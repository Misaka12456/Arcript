using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	[AsptCmd(AsptCmdType.IfCheck, isArptPlusCmd: true)]
	public class AsptIfCheckCmd : AsptCmdBase
	{
		[YamlMember(Alias = "Type")]
		public override AsptCmdType Type => AsptCmdType.IfCheck;

		[YamlMember(Alias = "Block")]
		public override bool IsBlock => false; // if判断必定不会block

		[YamlMember(Alias = "Conditions")]
		public AsptVarCheckCmd[] Conditions { get; set; }

		[YamlMember(Alias = "TrueCmds")] // 判断为真时执行的命令
		public AsptCmdBase[] TrueCmds { get; set; }

		[YamlMember(Alias = "FalseCmds")] // 判断为假时执行的命令
		public AsptCmdBase[] FalseCmds { get; set; }
	}
}
