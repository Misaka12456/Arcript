using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	[AsptCmd(AsptCmdType.HSceneStartAnchorTag, isArptPlusCmd: true)]
	public class AsptHSceneStartAnchorCmd : AsptAnchorCmd
	{
		[YamlMember(Alias = "Type")]
		public override AsptCmdType Type => AsptCmdType.HSceneStartAnchorTag;

		[YamlMember(Alias = "HSceneName")]
		public string HSceneName { get; set; }
	}
}
