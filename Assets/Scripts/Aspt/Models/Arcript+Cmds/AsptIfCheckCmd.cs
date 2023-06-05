using System.Linq;
using System.Text;
using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	[AsptCmd(AsptCmdType.IfCheck, isArptPlusCmd: true)]
	public class AsptIfCheckCmd : AsptCmdBase
	{
		[YamlMember(Alias = "Type")]
		public override string TypeStr => "if";

		[YamlMember(Alias = "Block")]
		public override bool IsBlock => false; // if判断必定不会block

		[YamlMember(Alias = "Conditions")]
		public AsptVarCheckCmd[] Conditions { get; set; }

		[YamlMember(Alias = "TrueCmds")] // 判断为真时执行的命令
		public AsptCmdBase[] TrueCmds { get; set; }

		[YamlMember(Alias = "FalseCmds")] // 判断为假时执行的命令
		public AsptCmdBase[] FalseCmds { get; set; }

		public override string ToItemShortString()
		{
			var sb = new StringBuilder("<b>if</b> ");
			sb.Append('(').Append(string.Join(" && ", Conditions.Select(c => c.Expression))).Append(')');
			sb.Append("{ ").Append("<").Append(TrueCmds.Length).Append(" cmds> } else { ").Append("<").Append(FalseCmds.Length).Append(" cmds> }");
			return sb.ToString();
		}
	}
}
