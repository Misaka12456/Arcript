using System.Text;
using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	[AsptCmd(AsptCmdType.HidePicture)]
	public class AsptPictureHideCmd : AsptCmdBase
	{
		[YamlMember(Alias = "Type")]
		public override string TypeStr => "hide";

		[YamlMember(Alias = "Block")]
		public override bool IsBlock { get; set; }

		[YamlMember(Alias = "ImagePath")]
		public string ImagePath { get; set; }

		[YamlMember(Alias = "Transition")]
		public Transition Transition { get; set; }

		public override string ToItemShortString()
		{
			var sb = new StringBuilder("<b>hide</b> ");
			sb.Append(ImagePath).Append(" | ");
			sb.Append("transition = ").Append(Transition.ToItemShortString());
			return sb.ToString();
		}
	}
}
