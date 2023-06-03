using System;
using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	[AsptCmd(AsptCmdType.ScriptAnchor, isArptPlusCmd: true)]
	public class AsptAnchorCmd : AsptCmdBase
	{
		[YamlMember(Alias = "Type")]
		public override AsptCmdType Type => AsptCmdType.ScriptAnchor;

		[YamlMember(Alias = "Block")]
		public override bool IsBlock => false; // 所有的"定义"都不会block(包括标签和锚点)

		[YamlMember(Alias = "AnchorName")]
		public string AnchorName { get; set; }

		[YamlMember(Alias = "AnchorTags")]
		public string[] AnchorTags { get; set; } = Array.Empty<string>();
	}
}
