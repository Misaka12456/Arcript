using System.Text;
using UnityEngine;
using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	[AsptCmd(AsptCmdType.VideoFullScreenPlay, isArptPlusCmd: true)]
	public class AsptVideoFullScrPlayCmd : AsptCmdBase
	{
		[YamlMember(Alias = "Type")]
		public override string TypeStr { get; set; } = "videoFSPlay";

		[YamlMember(Alias = "Block")]
		public override bool IsBlock { get; set; }

		[YamlMember(Alias = "VideoPath")]
		public string VideoPath { get; set; }

		[YamlMember(Alias = "StartFadeFrom", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public Color StartFadeInFrom { get; set; } = Color.white;

		[YamlMember(Alias = "StartFadeTo", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public Color StartFadeInTo { get; set; } = Color.black;

		[YamlMember(Alias = "StartFadeDuration", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public float StartFadeInDuration { get; set; } = 7.5f;

		[YamlMember(Alias = "EndFadeFrom", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public Color EndFadeOutFrom { get; set; } = Color.black;

		[YamlMember(Alias = "EndFadeTo", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public Color EndFadeOutTo { get; set; } = Color.white;

		[YamlMember(Alias = "EndFadeDuration", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public float EndFadeOutDuration { get; set; } = 7.5f;

		public override string ToItemShortString()
		{
			var sb = new StringBuilder("<b>videoFSPlay</b> ");
			sb.Append(VideoPath).Append(" | ");
			sb.Append("startFrom = ").Append(StartFadeInFrom).Append(" | ");
			sb.Append("startTo = ").Append(StartFadeInTo).Append(" | ");
			sb.Append("startDuration = ").Append(StartFadeInDuration).Append(" | ");
			sb.Append("endFrom = ").Append(EndFadeOutFrom).Append(" | ");
			sb.Append("endTo = ").Append(EndFadeOutTo).Append(" | ");
			sb.Append("endDuration = ").Append(EndFadeOutDuration);
			return sb.ToString();
		}
	}
}
