using System.Text;
using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	[AsptCmd(AsptCmdType.VarSet, isArptPlusCmd: true)]
	public class AsptVarSetCmd : AsptCmdBase
	{
		[YamlMember(Alias = "Type")]
		public override string TypeStr { get; set; } = "varSet";

		[YamlMember(Alias = "Block")]
		public override bool IsBlock { get; set; } = false;

		[YamlMember(Alias = "VarName")]
		public string VarName { get; set; }

		[YamlMember(Alias = "TargetValue")]
		public int TargetValue { get; set; }

		public override string ToItemShortString()
		{
			var sb = new StringBuilder("<b>varSet</b> ");
			sb.Append(VarName).Append(" = ");
			sb.Append(TargetValue);
			return sb.ToString();
		}
	}
}
