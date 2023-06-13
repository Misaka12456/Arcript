using Arcript.Aspt;
using UnityEngine.UI;

namespace Arcript.Compose.Inspectors
{
	[CmdInspectExport(typeof(AsptTextLegacyShowCmd), "say", "Arcript+ Text Show (Legacy)", "Resources/Prefabs/Editor/Inspectors/say")]
	public class ArptInspTextShowLegacy : InspectCmdPanelBase<AsptTextLegacyShowCmd>
	{
		public InputField inputSayText;

		protected override void InspectorAwake()
		{
			inputSayText.onValueChanged.AddListener((value) =>
			{
				cmd.Content = value;
				Apply();
			});
		}

		public override void SetInfo<T>(T command, ArptScriptCmdItem parent)
		{
			base.SetInfo(command, parent); // 检查类型是否匹配(之后parent会存到parentItem, command会转为AsptTextShowCmd后存到cmd)
			inputSayText.SetTextWithoutNotify(cmd.Content);
		}

		public override void Apply(object tag = null)
		{
			parentItem.UpdateInfo(cmd);
		}

		public override void InitNewInfo()
		{
			inputSayText.SetTextWithoutNotify("That's true. Who wouldn't want to fall in love sweetly in the summer?");
		}
	}

}
