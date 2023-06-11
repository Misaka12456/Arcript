using Arcript.Aspt;
using Arcript.I18n;
using Arcript.Utility;
using System;
using System.Collections.Generic;
using System.Enhance.Unity;
using System.Enhance.Unity.UI;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Arcript.Compose.Dialogs
{
	public class ArptRawCmdEditDialog : Singleton<ArptRawCmdEditDialog>
	{
		[Header("General")]
		public GameObject View;
		public Text labelDialogTitle;
		public GameObject panelValidateErrors;
		public Text labelValidateErrors;
		public InputField inputRawCmdCode;
		public Button btnSave, btnDiscard;

		private Type m_cmdType; // for yaml grammar & format validation (m_cmdType is the model type of the cmd)
		private Action<object> m_saveCallBack;
		private Action m_discardCallBack;
		private bool m_isNewCmd;

		private ISerializer m_asptSerializer;
		private IDeserializer m_asptDeserializer;

		protected override void SingletonAwake()
		{
			AllowRepeatInit = true;
			m_asptSerializer = CreateNewSerializer();
			m_asptDeserializer = CreateNewDeserializer();
			btnSave.onClick.AddListener(Save);
			btnDiscard.onClick.AddListener(() => Discard());
		}

		private ISerializer CreateNewSerializer()
		{
			var builder = new SerializerBuilder();
			var typeTags = new Dictionary<string, Type>();

			// 定义几个转换器
			var converters = ArptProjectManager.Converters;

			// 查找所有的AsptCmdBase的继承类
			var cmdTypes = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(a => a.GetTypes())
				.Where(t => t.IsSubclassOf(typeof(AsptCmdBase)) && !t.IsAbstract);

			// 提取所有继承类的名称以$"!{name}"的形式作为Yaml的Tag
			foreach (var t in cmdTypes)
			{
				string name = t.Name;
				var type = t;
				typeTags.Add($"!{name}", type);
			}

			// 注册所有的Tag
			foreach (var kv in typeTags)
			{
				builder = builder.WithTagMapping(kv.Key, kv.Value);
			}

			// 注册所有的转换器
			foreach (var c in converters)
			{
				builder = builder.WithTypeConverter(c);
			}

			// 构建Serializer
			var serializer = builder.Build();

			return serializer;
		}

		private IDeserializer CreateNewDeserializer()
		{
			var builder = new DeserializerBuilder();
			var typeTags = new Dictionary<string, Type>();

			// 定义几个转换器
			var converters = ArptProjectManager.Converters;

			// 查找所有的AsptCmdBase的继承类
			var cmdTypes = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(a => a.GetTypes())
				.Where(t => t.IsSubclassOf(typeof(AsptCmdBase)) && !t.IsAbstract);

			// 提取所有继承类的名称以$"!{name}"的形式作为Yaml的Tag
			foreach (var t in cmdTypes)
			{
				string name = t.Name;
				var type = t;
				typeTags.Add($"!{name}", type);
			}

			// 注册所有的Tag
			foreach (var kv in typeTags)
			{
				builder = builder.WithTagMapping(kv.Key, kv.Value);
			}

			// 注册所有的转换器
			foreach (var c in converters)
			{
				builder = builder.WithTypeConverter(c);
			}

			// 构建Deserializer
			var deserializer = builder.Build();

			return deserializer;
		}

		public void ShowDialog<T>(T cmd, Action<T> saveCallBack) where T : AsptCmdBase
		{
			#region Serialize the cmd into plain yaml code
			m_cmdType = typeof(T);
			m_isNewCmd = false;

			using var ms = new MemoryStream();
			using var sw = new StreamWriter(ms, Encoding.UTF8, 1024, leaveOpen: true);
			m_asptSerializer.Serialize(sw, cmd);
			sw.Flush();
			sw.Close();
			ms.Seek(0, SeekOrigin.Begin);
			string rawPlainCmd = Encoding.UTF8.GetString(ms.ToArray());
			ms.Close();
			#endregion
			// set the plain yaml code to the input field for further editing
			inputRawCmdCode.SetTextWithoutNotify(rawPlainCmd);

			#region Dialog prepares (title + validation error tips)
			// set the title of the dialog
			string template = I.S["compose.dialogs.rawCodeEdit.title"].value;
			string title = string.Format(template, typeof(T).Name);
			labelDialogTitle.text = title;
			panelValidateErrors.SetActive(false);
			#endregion

			View.SetActive(true);

			m_saveCallBack = (obj) =>
			{
				saveCallBack?.Invoke((T)obj);
			};
		}

		public void ShowDialogAsNewCmd(Action<object> saveCallBack, Action discardCallBack)
		{
			m_cmdType = typeof(AsptCmdBase);
			m_isNewCmd = true;

			string template = I.S["compose.dialogs.rawCodeCreate.title"].value;
			string title = string.Format(template, "Aspt Command");
			labelDialogTitle.text = title;
			panelValidateErrors.SetActive(false);
			inputRawCmdCode.SetTextWithoutNotify(string.Empty);
			inputRawCmdCode.placeholder.GetComponent<Text>().text = I.S["compose.dialogs.rawCodeCreate.placeholder"].value;

			View.SetActive(true);

			m_saveCallBack = saveCallBack;
			m_discardCallBack = discardCallBack;
		}

		private void Save()
		{
			panelValidateErrors.SetActive(false);
			btnSave.interactable = btnDiscard.interactable = false;
			if (!TryDeserializeCmd(out string error, out var rawCmdObj))
			{
				labelValidateErrors.text = error;
				labelValidateErrors.color = Color.red;
				panelValidateErrors.SetActive(true);
				btnSave.interactable = btnDiscard.interactable = true;
				return;
			}

			bool save;

			if (!m_isNewCmd)
			{
				save = true;
			}
			else
			{
				var cmdAttr = rawCmdObj.GetType().GetCustomAttribute<AsptCmdAttribute>();
				string cmdActualTypeStr = $"AsptCmdType.{Enum.GetName(typeof(AsptCmdType), cmdAttr.CmdType)}";
				string createConfirmStr = I.S["compose.dialogs.rawCodeCreate.confirm"].value;
				createConfirmStr = string.Format(createConfirmStr, cmdActualTypeStr);

				var r = MsgBoxDialog.Show(createConfirmStr, I.S["compose.dialogs.rawCodeCreate.title"].value, MsgBoxDialog.MsgBoxType.YesNo);

				save = r == MsgBoxDialog.MsgBoxResult.Yes;
			}

			if (save)
			{
				var cmd = Convert.ChangeType(rawCmdObj, m_cmdType);
				m_saveCallBack?.Invoke(cmd);
				Discard(true);
			}
			else
			{
				btnSave.interactable = btnDiscard.interactable = true;
			}
		}

		private void Discard(bool isSave = false)
		{
			m_saveCallBack = null;
			
			if (!isSave)
			{
				m_discardCallBack?.Invoke();
			}
			m_discardCallBack = null;
			
			View.SetActive(false);
			btnSave.interactable = btnDiscard.interactable = true;
			inputRawCmdCode.SetTextWithoutNotify(string.Empty);
		}

		private bool TryDeserializeCmd(out string error, out object resultCmd)
		{
			try
			{
				// Try to deserialize the plain yaml code into the cmd object
				using var sr = new StringReader(inputRawCmdCode.text);
				var cmd = m_asptDeserializer.Deserialize(sr, m_cmdType);
				if (cmd != null && cmd.GetType() == m_cmdType)
				{
					error = null;
					resultCmd = cmd;
					return true;
				}
				else
				{
					throw new YamlException("Deserialize failed.");
				}
			}
			catch (YamlException ex)
			{
				string template = I.S["compose.dialogs.rawCodeEdit.save.checkFailed"].value;
				string errFullStr = ex.ToString();
				errFullStr = errFullStr.Replace(ex.StackTrace, string.Empty);
				error = string.Format(template, errFullStr);
				resultCmd = null;
				return false;
			}
		}
	}
}
