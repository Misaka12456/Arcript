using System;
using YamlDotNet.Serialization;
using System.Text;
using System.Linq;

namespace Arcript.Aspt
{
	[AsptCmd(AsptCmdType.ScriptAnchor, isArptPlusCmd: true)]
	public class AsptAnchorCmd : AsptCmdBase
	{
		[YamlMember(Alias = "Type")]
		public override string TypeStr => "anchor";

		[YamlMember(Alias = "Block")]
		public override bool IsBlock => false; // 所有的"定义"都不会block(包括标签和锚点)

		[YamlMember(Alias = "AnchorName")]
		public string AnchorName { get; set; }

		[YamlMember(Alias = "AnchorTags")]
		public string[] AnchorTags { get; set; } = Array.Empty<string>();

		public override string ToItemShortString()
		{
			var sb = new StringBuilder("<b>anchor</b> ");
			sb.Append(AnchorName);
			if (AnchorTags.Any())
			{
				sb.Append($" | tags = {string.Join(", ", AnchorTags)}");
			}
			else
			{
				sb.Append($" | tags = <none>");
			}
			return sb.ToString();
		}
	}
}
