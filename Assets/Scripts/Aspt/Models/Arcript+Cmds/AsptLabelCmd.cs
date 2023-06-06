using System.Text;
using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	[AsptCmd(AsptCmdType.LabelDefine, isArptPlusCmd: true)]
	public class AsptLabelCmd : AsptCmdBase
	{
		[YamlMember(Alias = "Type")]
		public override string TypeStr { get; set; } = "label";

		[YamlMember(Alias = "Block")]
		public override bool IsBlock { get; set; } = false; // 所有的"定义"都不会block(包括标签和锚点)

		[YamlMember(Alias = "LabelName")]
		public string LabelName { get; set; }

		public override string ToItemShortString()
		{
			var sb = new StringBuilder("<b>label</b> ");
			sb.Append(LabelName);
			return sb.ToString();
		}
	}
}
