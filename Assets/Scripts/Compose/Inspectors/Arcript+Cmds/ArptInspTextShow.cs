using Arcript.Aspt;
using UnityEngine;
using UnityEngine.UI;

namespace Arcript.Compose.Inspectors
{
	[CmdInspectExport(typeof(AsptTextShowCmd), "say+", "Arcript+ Text Show (v2)")]
	public class ArptInspTextShow : InspectCmdPanelBase<AsptTextShowCmd>
	{
		[Header("UI")]
		public InputField inputSpeakerName;
		public InputField inputSayText;
		public Toggle toggleFallbackToLegacyText;

		protected override void InspectorAwake()
		{
			inputSpeakerName.onValueChanged.AddListener((value) =>
			{
				cmd.Speaker = value;
				Apply();
			});
			inputSayText.onValueChanged.AddListener((value) =>
			{
				cmd.Content = value;
				Apply();
			});
			toggleFallbackToLegacyText.onValueChanged.AddListener((value) =>
			{
				cmd.FallbackToLegacy = value;
				Apply();
			});
		}

		public override void SetInfo<T>(T command, ArptScriptCmdItem parent)
		{
			base.SetInfo(command, parent); // 检查类型是否匹配(之后parent会存到parentItem, command会转为AsptTextShowCmd后存到cmd)
			inputSpeakerName.SetTextWithoutNotify(cmd.Speaker);

			inputSayText.SetTextWithoutNotify(cmd.Content);
			toggleFallbackToLegacyText.SetIsOnWithoutNotify(cmd.FallbackToLegacy);
		}

		public override void Apply(object tag = null)
		{
			parentItem.UpdateInfo(cmd);
		}

		public override void InitNewInfo()
		{
			inputSpeakerName.SetTextWithoutNotify(string.Empty);
			inputSayText.SetTextWithoutNotify("「おはようございます」");
			toggleFallbackToLegacyText.SetIsOnWithoutNotify(false);
		}
	}
}
