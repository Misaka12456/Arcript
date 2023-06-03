using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	[AsptCmd(AsptCmdType.VarDefine, isArptPlusCmd: true)]
	public class AsptVarDefineCmd : AsptCmdBase
	{
		[YamlMember(Alias = "Type")]
		public override AsptCmdType Type => AsptCmdType.VarDefine;

		[YamlMember(Alias = "Block")]
		public override bool IsBlock => false;

		[YamlMember(Alias = "VarName")]
		public string VarName { get; set; }

		[YamlMember(Alias = "InitValue")]
		public int InitValue { get; set; }
	}
}
