using System.ComponentModel;
using System.Text;
using UnityEngine.Scripting;
using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	[AsptCmd(AsptCmdType.PlayAudio)]
	public class AsptAudioPlayCmd : AsptCmdBase
	{
		[YamlMember(Alias = "Type")]
		public override string TypeStr => "play";

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

		public override string ToItemShortString()
		{
			var sb = new StringBuilder("<b>play</b> ");
			sb.Append(AudioPath).Append(" | ");
			sb.Append("volume = ").Append(Volume).Append(" | ");
			sb.Append("loop = ").Append(IsLoop);
			return sb.ToString();
		}
	}
}
