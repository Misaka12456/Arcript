using System.Text;
using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	[AsptCmd(AsptCmdType.SubVarCheck, isArptPlusCmd: true, isSubCmd: true)]
	public class AsptVarCheckCmd : AsptCmdBase, ISubCmd
	{
		[YamlMember(Alias = "Type")]
		public override string TypeStr => "varCheck";

		[YamlMember(Alias = "Block")]
		public override bool IsBlock => false; // 还是那句话，都不是完整的指令何来Block一说

		[YamlMember(Alias = "Expr")]
		public string Expression { get; set; }

		public override string ToItemShortString()
		{
			var sb = new StringBuilder("varCheck(").Append(Expression).Append(")");
			return sb.ToString();
		}
	}
}
