using System;
using System.Text;
using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	[AsptCmd(AsptCmdType.ShowBranchSelection, isArptPlusCmd: true)]
	public class AsptBranchSelectCmd : AsptCmdBase
	{
		[YamlMember(Alias = "Type")]
		public override string TypeStr => "selectSay";

		[YamlMember(Alias = "Block")]
		public override bool IsBlock => true; // 选择支必定会block

		[YamlMember(Alias = "SelectTip")]
		public string SelectTip { get; set; }

		[YamlMember(Alias = "Options")]
		public AsptBranchSelectOptionCmd[] Options { get; set; } = Array.Empty<AsptBranchSelectOptionCmd>();

		public override string ToItemShortString()
		{
			var sb = new StringBuilder("<b>selectSay</b> ");
			sb.Append(SelectTip);
			sb.Append(" +").Append(Options.Length).Append(" options");
			return sb.ToString();
		}
	}
}
