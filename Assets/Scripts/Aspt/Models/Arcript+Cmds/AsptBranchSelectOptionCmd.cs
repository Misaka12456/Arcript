using System;
using System.Text;
using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	[AsptCmd(AsptCmdType.SubBranchOptionItem, isArptPlusCmd: true, isSubCmd: true)]
	public class AsptBranchSelectOptionCmd : AsptCmdBase, ISubCmd
	{
		[YamlMember(Alias = "Type")]
		public override string TypeStr { get; set; } = "select";

		[YamlMember(Alias = "Block")]
		public override bool IsBlock { get; set; } = false; // 都不是完整的指令何来Block一说

		[YamlMember(Alias = "FriendlyText")]
		public string FriendlyText { get; set; }

		[YamlMember(Alias = "Id")]
		public string OptionId { get; set; }

		[YamlMember(Alias = "Actions")]
		public AsptCmdBase[] ChooseActions { get; set; } = Array.Empty<AsptCmdBase>();

		[YamlMember(Alias = "Goto", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public string GotoLabelOrAnchor { get; set; } = "<continue>";

		[YamlMember(Alias = "Requirements", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public AsptVarCheckCmd[] Requirements { get; set; } = Array.Empty<AsptVarCheckCmd>();

		public override string ToItemShortString()
		{
			var sb = new StringBuilder(FriendlyText).Append(" (").Append(OptionId).Append(")");
			return sb.ToString();
		}
	}
}
