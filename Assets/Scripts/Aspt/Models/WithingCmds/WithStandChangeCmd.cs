using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	[YamlSerializable]
	public class WithStandChangeCmd : IWithingCmd
	{
		[YamlMember(Alias = "ImagePath")]
		public string StandImagePath { get; set; }

		[YamlMember(Alias = "Duration")]
		public float SwitchDuration { get; set; }

		[YamlMember(Alias = "Delay", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public float Delay { get; set; } = 0f;
	}
}
