using YamlDotNet.Serialization;

namespace Arcript.Aspt.QuickFuncs
{
	public class QuickStand
	{
		[YamlMember(Alias = "FilePath")]
		public string StandImagePath { get; set; }

		[YamlMember(Alias = "ShortCut")]
		public string ShortCutName { get; set; }
	}
}