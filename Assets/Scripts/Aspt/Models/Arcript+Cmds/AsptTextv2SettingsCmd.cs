using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	[AsptCmd(AsptCmdType.SetTextv2Settings, isArptPlusCmd: true)]
	public class AsptTextv2SettingsCmd : AsptCmdBase
	{
		[YamlMember(Alias = "Type")]
		public override AsptCmdType Type => AsptCmdType.SetTextv2Settings;

		[YamlMember(Alias = "Block")]
		public override bool IsBlock { get; set; }

		[YamlMember(Alias = "SpeakerBgBoxPath", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public string SpeakerBoxImagePath { get; set; } = string.Empty;

		[YamlMember(Alias = "MsgBgBoxPath")]
		public string MsgBoxImagePath { get; set; }
	}
}
