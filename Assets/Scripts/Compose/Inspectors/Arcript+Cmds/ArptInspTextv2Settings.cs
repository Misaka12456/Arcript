using Arcript.Aspt;
using UnityEngine.UI;

namespace Arcript.Compose.Inspectors
{
	[CmdInspectExport(typeof(AsptTextv2SettingsCmd), "say+set")]
	public class ArptInspTextv2Settings : InspectCmdPanelBase<AsptTextv2SettingsCmd>
	{
		public InputField inputSpeakerBoxBgPath;
		public Button btnSelectSpeakerBoxBg;
		public InputField inputMessageBoxBgPath;
		public Button btnSelectMessageBoxBg;

		protected override void InspectorAwake()
		{
			inputSpeakerBoxBgPath.onValueChanged.AddListener((value) =>
			{
				cmd.SpeakerBoxImagePath = value;
				Apply();
			});
			inputMessageBoxBgPath.onValueChanged.AddListener((value) =>
			{
				cmd.MsgBoxImagePath = value;
				Apply();
			});
		}

		public override void SetInfo<C>(C command, ArptScriptCmdItem parentItem)
		{
			base.SetInfo(command, parentItem);

			#region speakerBoxBgPath
			inputSpeakerBoxBgPath.SetTextWithoutNotify(cmd.SpeakerBoxImagePath);
			#endregion

			#region messageBoxBgPath
			inputMessageBoxBgPath.SetTextWithoutNotify(cmd.MsgBoxImagePath);
			#endregion
		}

		public override void Apply(object tag = null)
		{
			parentItem.UpdateInfo(cmd);
		}

		public override void InitNewInfo()
		{
			inputSpeakerBoxBgPath.SetTextWithoutNotify(string.Empty);
			inputMessageBoxBgPath.SetTextWithoutNotify(string.Empty);
			btnSelectMessageBoxBg.interactable = btnSelectSpeakerBoxBg.interactable = true;
		}
	}
}