using System.Linq;
using System.Text;
using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	[AsptCmd(AsptCmdType.HSceneStartAnchorTag, isArptPlusCmd: true)]
	public class AsptHSceneStartAnchorCmd : AsptAnchorCmd
	{
		[YamlMember(Alias = "Type")]
		public override string TypeStr => "anchorHSStart";

		[YamlMember(Alias = "HSceneName")]
		public string HSceneName { get; set; }
		public override string ToItemShortString()
		{
			var sb = new StringBuilder("<b>anchorHSStart</b> ");
			sb.Append(AnchorName).Append(" | hscene = ").Append(HSceneName);
			if (AnchorTags.Any())
			{
				sb.Append($" | tags = {string.Join(", ", AnchorTags)}");
			}
			else
			{
				sb.Append($" | tags = <none>");
			}
			return sb.ToString();
		}
	}
}
