using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	[AsptCmd(AsptCmdType.StopAudio)]
	public class AsptAudioStopCmd : AsptCmdBase
	{
		[YamlMember(Alias = "Type")]
		public override AsptCmdType Type => AsptCmdType.StopAudio;

		[YamlMember(Alias = "Block")]
		public override bool IsBlock { get; set; }

		[YamlMember(Alias = "AudioPath")]
		public string AudioPath { get; set; }

		[YamlMember(Alias = "Curve", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public CurveType Curve { get; set; } = CurveType.Linear;

		[YamlMember(Alias = "Duration", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public float Duration { get; set; } = 0f;
	}
}
