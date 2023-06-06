using System.Text;
using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	[AsptCmd(AsptCmdType.SetTextv2Settings, isArptPlusCmd: true)]
	public class AsptTextv2SettingsCmd : AsptCmdBase
	{
		[YamlMember(Alias = "Type")]
		public override string TypeStr { get; set; } = "say+set";

		[YamlMember(Alias = "Block")]
		public override bool IsBlock { get; set; }

		[YamlMember(Alias = "SpeakerBoxImg", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public string SpeakerBoxImagePath { get; set; } = string.Empty;

		[YamlMember(Alias = "MsgBoxImg")]
		public string MsgBoxImagePath { get; set; }

		public override string ToItemShortString()
		{
			var sb = new StringBuilder("<b>say+set</b> ");
			sb.Append("speakerBgBox = ").Append(string.IsNullOrWhiteSpace(SpeakerBoxImagePath) ? "<none>" : SpeakerBoxImagePath).Append(" | ");
			sb.Append("msgBgBox = ").Append(MsgBoxImagePath);
			return sb.ToString();
		}
	}
}
