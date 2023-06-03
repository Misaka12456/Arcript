using System.ComponentModel;
using UnityEngine.Scripting;
using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	[AsptCmd(AsptCmdType.PlayAudio)]
	public class AsptAudioPlayCmd : AsptCmdBase
	{
		[YamlMember(Alias = "Type")]
		public override AsptCmdType Type => AsptCmdType.PlayAudio;

		[YamlMember(Alias = "Block")]
		public override bool IsBlock { get; set; }

		[YamlMember(Alias = "AudioPath")]
		public string AudioPath { get; set; }

		[YamlMember(Alias = "Volume", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public float Volume { get; set; } = 1f;

		[YamlMember(Alias = "Loop", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public bool IsLoop { get; set; } = false;

		[Preserve]
		public AsptAudioPlayCmd() { }

		[Preserve]
		public AsptAudioPlayCmd(bool block, string audioPath, float volume = 1f, bool isLoop = false)
		{
			IsBlock = block;
			AudioPath = audioPath;
			Volume = volume;
			IsLoop = isLoop;
		}
	}
}
