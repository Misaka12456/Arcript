using System.Text;
using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	[AsptCmd(AsptCmdType.WaitOrSleep)]
	public class AsptWaitSleepCmd : AsptCmdBase
	{
		[YamlMember(Alias = "Type")]
		public override string TypeStr { get; set; } = "wait";

		[YamlMember(Alias = "Block")]
		public override bool IsBlock { get; set; } = true; // sleep指令必定会block之后的脚本执行，因此恒定为true

		[YamlMember(Alias = "Duration")]
		public float Duration { get; set; } = 1f;

		public override string ToItemShortString()
		{
			var sb = new StringBuilder("<b>wait</b> ");
			sb.Append("duration = ").Append(Duration);
			return sb.ToString();
		}
	}
}
