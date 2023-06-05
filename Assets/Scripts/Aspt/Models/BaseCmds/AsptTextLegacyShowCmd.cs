using System.Text;
using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	[AsptCmd(AsptCmdType.ShowTextLegacy)]
	public class AsptTextLegacyShowCmd : AsptCmdBase
	{
		[YamlMember(Alias = "Type")]
		public override string TypeStr => "say";

		[YamlMember(Alias = "Block")]
		public override bool IsBlock => !AutoNextText;

		[YamlMember(Alias = "Content")]
		public string Content { get; set; }

		[YamlMember(Alias = "AutoNextText", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public bool AutoNextText { get; set; } = false;

		public override string ToItemShortString()
		{
			var sb = new StringBuilder("<b>say</b> ");
			sb.Append(Content).Append(" | ");
			sb.Append("autoNextText = ").Append(AutoNextText);
			return sb.ToString();
		}
	}
}
