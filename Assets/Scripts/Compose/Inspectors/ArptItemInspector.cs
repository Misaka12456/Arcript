using Arcript.Aspt.RawArcVNScripts;
using System;
using System.Enhance.Unity;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

namespace Arcript.Compose.Inspectors
{
	public class ArptItemInspector : Singleton<ArptItemInspector>
	{
		[Header("File Info")]
		public Text labelFileName;
		public InputField inputFileTypeName;

		protected override void SingletonAwake()
		{
			AllowRepeatInit = true;
		}

		public void SetFileInfo(string fileName, string typeName)
		{
			labelFileName.text = fileName;
			inputFileTypeName.text = typeName;
		}

		public void SetCmdInfo(ArptScriptCmdItem item, ArcVNScriptCmdBase cmd)
		{
			
		}
	}

	public abstract class InpectInfoPanelBase : MonoBehaviour
	{
		public abstract GameObject TemplatePrefab { get; }

		public abstract void SetInfo(string fileName, string typeName);
	}

	public abstract class InspectCmdPanelBase : InpectInfoPanelBase
	{
		public override GameObject TemplatePrefab { get; }

		internal ArptScriptCmdItem parentItem;

		public override void SetInfo(string fileName, string typeName) => throw new NotSupportedException("Can't set file info to a command panel.");

		public abstract void SetInfo<T>(T command) where T : ArcVNScriptCmdBase;

		public abstract void Apply(object tag = null);
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class CmdInspectExportAttribute : Attribute
	{
		public RawArcVNScriptCmdType CmdType { get; private set; }

		/// <summary>
		/// 表示该命令的指令代码名称(在脚本文件中的指令代码)<br />
		/// 如 <see cref="TextShowCmd"/> 在脚本文件中格式为<c>"say+ 参数..."</c>，则若 <see cref="CmdType"/> 为 <see cref="RawArcVNScriptCmdType.ShowTextv2"/>，则该属性应为 <c>"say+"</c>
		/// </summary>
		public string CmdCodeName { get; private set; }

		[Preserve]
		public CmdInspectExportAttribute(RawArcVNScriptCmdType type, string codePrefix)
		{
			CmdType = type;
			CmdCodeName = codePrefix;
		}
	}

	[CmdInspectExport(RawArcVNScriptCmdType.ShowTextv2, "say+")]
	public class ArptInspectCSayPlusPnl : InspectCmdPanelBase
	{
		// C = Command, "SayPlus" is "say+" command.
		public override GameObject TemplatePrefab => prefabPanel;

		[Header("General")]
		public GameObject prefabPanel;

		[Header("UI")]
		public InputField inputSpeakerName;
		public InputField inputSayText;
		public Toggle toggleFallbackToLegacyText;

		public override void SetInfo<T>(T command)
		{
			if (typeof(T) != typeof(TextShowCmd)) throw new ArgumentException($"Mismatched command type. Expected: TextShowCmd, Actual: {typeof(T)}");
			if (parentItem == null) throw new NullReferenceException("Please set parent item first to indicate changes when appling (changes).");
			var cmd = ((ArcVNScriptCmdBase)command) as TextShowCmd;
			if (cmd == null) throw new NullReferenceException("Command is null. This may indicate that the command type mismatched.");
			inputSpeakerName.SetTextWithoutNotify(cmd.Speaker);
			inputSayText.SetTextWithoutNotify(cmd.Text);
			toggleFallbackToLegacyText.SetIsOnWithoutNotify(cmd.TreatAsFallbackLegacyText);
			inputSpeakerName.onValueChanged.RemoveAllListeners();
			inputSayText.onValueChanged.RemoveAllListeners();
			toggleFallbackToLegacyText.onValueChanged.RemoveAllListeners();
		}

		public override void Apply(object tag = null)
		{
			if (tag == null) throw new ArgumentNullException(nameof(tag));
			var tuple = tag as (int, string)?; // == tag as ValueTuple<int, string>?
			if (!tuple.HasValue) throw new ArgumentException($"Tag must be a tuple of (int, string). Actual: {tag.GetType()}");
			var (typeName, changesText) = tuple.Value;
			ApplyChanges(typeName, changesText);
		}

		private void ApplyChanges(int typeName, string changesText)
		{
			
		}
	}
}
