using Arcript.Aspt;
using Arcript.Compose.Inspectors;
using Arcript.Data;
using Arcript.I18n;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Enhance.Unity.UI;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Arcript.Compose
{
	public class ArptScriptCmdItem : MonoBehaviour
	{
		public static readonly Color Transparent = new Color(1, 1, 1, 0);

		private static List<(CmdInspectExportAttribute, Type)> m_exportedInspectors;

		[Header("General")]
		public Text labelCmdText;
		public Toggle toggleChoosing;
		public Text labelIsBlockCmdSign;
		
		[SerializeField]
		private AsptCmdBase refCmd;

		private void Awake()
		{
			InitInspectors();
			toggleChoosing.onValueChanged.AddListener((bool isOn) =>
			{
				if (!isOn) return;
				SetInspectorInfo(this, refCmd);
			});
		}

		public void SetInfo(string cmdShortInfo, bool isBlock, ToggleGroup parentGroup)
		{
			labelCmdText.supportRichText = true; // 启用富文本，以确保<b>等标签能正常解析并显示
			labelCmdText.horizontalOverflow = HorizontalWrapMode.Overflow; // 指令文本超出时不换行(直接向右Overflow，反正有Mask)(因为指令文本只有一行)

			// 将cmdShortInfo中所有的换行都去掉，防止无法在一行上完全显示
			cmdShortInfo = cmdShortInfo.Replace("\r\n", string.Empty).Replace("\n", string.Empty);

			labelCmdText.text = cmdShortInfo;

			//labelIsBlockCmdSign.gameObject.SetActive(isBlock);
			// 这里直接SetActive会导致布局意义上直接消失了，所以这里改用alpha
			labelIsBlockCmdSign.color = isBlock ? Color.white : Transparent;

			parentGroup.RegisterToggle(toggleChoosing); // == toggleChoosing.group = parentGroup;
		}

		public void UpdateInfo(AsptCmdBase newCmd)
		{
			labelCmdText.text = newCmd.ToItemShortString();

			labelIsBlockCmdSign.color = newCmd.IsBlock ? Color.white : Transparent;
		}

		private void SetInspectorInfo(ArptScriptCmdItem parentItem, AsptCmdBase cmd, bool ignoreArptPlusCheck = false)
		{
			try
			{
				var asptCmdAttr = cmd.GetType().GetCustomAttribute<AsptCmdAttribute>();
				if (!ignoreArptPlusCheck)
				{
					bool isArptPlusCmd = asptCmdAttr.IsArptPlusCmd;
					if (!ArptProjectManager.Instance.CurrentProject.EnableArcriptPlusFeatures)
					{
						var diagR = MsgBoxDialog.Show(I.S["compose.sideright.inspector.arpt+onlyLimited"].value, I.S["compose.dialogs.arpt+onlylimited.title"].value,
							MsgBoxDialog.MsgBoxType.IconWarning | MsgBoxDialog.MsgBoxType.YesNo);
						if (diagR == MsgBoxDialog.MsgBoxResult.No)
						{
							return;
						}
						else
						{
							ArptProjectManager.Instance.CurrentProject.EnableArcriptPlusFeatures = true;
							UniTask.Create(async () =>
							{
								await ArptProjectManager.Instance.SaveProjectAsync();
								SetInspectorInfo(parentItem, cmd, ignoreArptPlusCheck: true);
							});
						}
					}
				}
				var inspPair = m_exportedInspectors.FirstOrDefault(i => i.Item1.CmdModelType == cmd.GetType());
				if (inspPair == default)
				{
					Debug.LogError($"[ArptScriptCmdItem] Cannot find inspector for cmd {cmd.GetType().FullName}");
					throw new ArcriptRuntimeException(string.Format(I.S["compose.dialogs.error.cmdInspNotFound"].value, cmd.GetType().FullName));
				}

				string prefabPath = inspPair.Item1.CmdInspectPrefabPath;

				var goInspector = ArptItemInspector.Instance.SetCmdInfo(prefabPath, inspPair.Item1);

				var insp = goInspector.GetComponent(inspPair.Item2) as InspectCmdPanelBase<AsptCmdBase>;

				insp.SetInfo(type: inspPair.Item2, command: refCmd, parentItem: this);
			}
			catch (ArcriptException)
			{
				throw;
			}
		}

		private void InitInspectors()
		{
			if (m_exportedInspectors != null) return;

			var assembiles = AppDomain.CurrentDomain.GetAssemblies();

			// 查找所有继承自InspectCmdPanelBase<T>且拥有CmdInspectExport特性的类
			var inspectors = new List<(CmdInspectExportAttribute, Type)>();

			foreach (var asm in assembiles)
			{
				var found = asm.GetTypes().Where(t =>
				{
					if (t.IsAbstract) return false;
					if (!t.IsSubclassOf(typeof(InspectCmdPanelBase<>))) return false;
					var attr = t.GetCustomAttribute<CmdInspectExportAttribute>();
					if (attr == null) return false;
					return true;
				}).Select(t => (t.GetCustomAttribute<CmdInspectExportAttribute>(), t));
				inspectors.AddRange(found);
			}

			m_exportedInspectors = inspectors;
		}
	}
}
