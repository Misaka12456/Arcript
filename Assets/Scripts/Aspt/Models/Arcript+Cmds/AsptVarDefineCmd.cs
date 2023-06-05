using System.Text;
using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	[AsptCmd(AsptCmdType.VarDefine, isArptPlusCmd: true)]
	public class AsptVarDefineCmd : AsptCmdBase
	{
		[YamlMember(Alias = "Type")]
		public override string TypeStr => "var";

		[YamlMember(Alias = "Block")]
		public override bool IsBlock => false;

		[YamlMember(Alias = "VarName")]
		public string VarName { get; set; }

		[YamlMember(Alias = "InitValue")]
		public int InitValue { get; set; }

		public override string ToItemShortString()
		{
			var sb = new StringBuilder("<b>var</b> ");
			sb.Append(VarName).Append(" | ");
			sb.Append("initValue = ").Append(InitValue);
			return sb.ToString();
		}
	}
}
