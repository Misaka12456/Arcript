using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	[AsptCmd(AsptCmdType.WaitOrSleep)]
	public class AsptWaitSleepCmd : AsptCmdBase
	{
		[YamlMember(Alias = "Type")]
		public override AsptCmdType Type => AsptCmdType.WaitOrSleep;

		[YamlMember(Alias = "Block")]
		public override bool IsBlock => true; // sleep指令必定会block之后的脚本执行，因此恒定为true

		[YamlMember(Alias = "Duration")]
		public float Duration { get; set; } = 1f;
	}
}
