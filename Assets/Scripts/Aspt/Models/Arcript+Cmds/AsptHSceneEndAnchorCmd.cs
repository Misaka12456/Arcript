using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	[AsptCmd(AsptCmdType.HSceneEndAnchorTag, isArptPlusCmd: true)]
	public class AsptHSceneEndAnchorCmd : AsptAnchorCmd
	{
		[YamlMember(Alias = "Type")]
		public override AsptCmdType Type => AsptCmdType.HSceneEndAnchorTag;

		[YamlMember(Alias = "HSceneName")]
		public string HSceneName { get; set; }
	}
}
