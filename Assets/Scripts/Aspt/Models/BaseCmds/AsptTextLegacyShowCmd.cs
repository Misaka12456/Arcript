using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	[AsptCmd(AsptCmdType.ShowTextLegacy)]
	public class AsptTextLegacyShowCmd : AsptCmdBase
	{
		[YamlMember(Alias = "Type")]
		public override AsptCmdType Type => AsptCmdType.ShowTextLegacy;

		[YamlMember(Alias = "Block")]
		public override bool IsBlock => !AutoNextText;

		[YamlMember(Alias = "Content")]
		public string Content { get; set; }

		[YamlMember(Alias = "AutoNextText", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public bool AutoNextText { get; set; } = false;
	}
}
