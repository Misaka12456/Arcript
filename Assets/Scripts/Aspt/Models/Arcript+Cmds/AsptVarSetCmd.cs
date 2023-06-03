using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	[AsptCmd(AsptCmdType.VarSet, isArptPlusCmd: true)]
	public class AsptVarSetCmd : AsptCmdBase
	{
		[YamlMember(Alias = "Type")]
		public override AsptCmdType Type => AsptCmdType.VarSet;

		[YamlMember(Alias = "Block")]
		public override bool IsBlock => false;

		[YamlMember(Alias = "VarName")]
		public string VarName { get; set; }

		[YamlMember(Alias = "TargetValue")]
		public int TargetValue { get; set; }
	}
}
