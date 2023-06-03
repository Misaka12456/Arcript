using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	[AsptCmd(AsptCmdType.LabelDefine, isArptPlusCmd: true)]
	public class AsptLabelCmd : AsptCmdBase
	{
		[YamlMember(Alias = "Type")]
		public override AsptCmdType Type => AsptCmdType.LabelDefine;

		[YamlMember(Alias = "Block")]
		public override bool IsBlock => false; // 所有的"定义"都不会block(包括标签和锚点)

		[YamlMember(Alias = "LabelName")]
		public string LabelName { get; set; }
	}
}
