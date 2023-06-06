using System;
using System.Text;
using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	[AsptCmd(AsptCmdType.ShowTextv2, isArptPlusCmd: true)]
	public class AsptTextShowCmd : AsptCmdBase
	{
		[YamlMember(Alias = "Type")]
		public override string TypeStr { get; set; } = "say+";

		[YamlMember(Alias = "Block")]
		public override bool IsBlock { get; set; }

		[YamlMember(Alias = "SpeakerName")]
		public string Speaker { get; set; }

		[YamlMember(Alias = "Content")]
		public string Content { get; set; }

		[YamlMember(Alias = "ShowAsCSText", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public bool ShowAsCenterScreenText { get; set; } = false;

		[YamlMember(Alias = "Ruby", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public bool IsRubyIncluded { get; set; } = false;

		[YamlMember(Alias = "RichText", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public bool IsRichText { get; set; } = true;

		[YamlMember(Alias = "WithStandChange", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public WithStandChangeCmd WithStandChange { get; set; } = null;

		[YamlMember(Alias = "WithStandChanges", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public WithStandChangeCmd[] WithStandChanges { get; set; } = Array.Empty<WithStandChangeCmd>();

		[YamlMember(Alias = "WithVoice", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public string WithVoicePath { get; set; } = string.Empty;

		[YamlMember(Alias = "Fallback", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public bool FallbackToLegacy { get; set; } = false;

		public override string ToItemShortString()
		{
			var sb = new StringBuilder("<b>say+</b> ");
			if (string.IsNullOrWhiteSpace(Speaker) && string.IsNullOrWhiteSpace(Content))
			{
				sb.Append("<i><clearScreen></i>");
			}
			else
			{
				sb.Append(Speaker).Append(' ').Append(Content.Replace("\n", string.Empty));
			}
			return sb.ToString();
		}
	}
}
