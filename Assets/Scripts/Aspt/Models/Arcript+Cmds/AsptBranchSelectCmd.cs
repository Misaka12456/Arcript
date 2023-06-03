using System;
using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	[AsptCmd(AsptCmdType.ShowBranchSelection, isArptPlusCmd: true)]
	public class AsptBranchSelectCmd : AsptCmdBase
	{
		[YamlMember(Alias = "Type")]
		public override AsptCmdType Type => AsptCmdType.ShowBranchSelection;

		[YamlMember(Alias = "Block")]
		public override bool IsBlock => true; // 选择支必定会block

		[YamlMember(Alias = "SelectTip")]
		public string SelectTip { get; set; }

		[YamlMember(Alias = "Options")]
		public AsptBranchSelectOptionCmd[] Options { get; set; } = Array.Empty<AsptBranchSelectOptionCmd>();
	}
}
