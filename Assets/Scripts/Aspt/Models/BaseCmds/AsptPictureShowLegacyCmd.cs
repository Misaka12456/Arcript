using System.ComponentModel;
using UnityEngine;
using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	public enum TransitionType
	{
		[Description("fade")] Fade = 0
	}

	public class Transition : ISubCmd
	{
		[YamlMember(Alias = "Type", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public TransitionType Type { get; set; } = TransitionType.Fade;

		[YamlMember(Alias = "Curve", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public CurveType Curve { get; set; } = CurveType.Linear;

		[YamlMember(Alias = "Duration")]
		public float Duration { get; set; } = 0f;
	}

	[AsptCmd(AsptCmdType.ShowPicture, isArptPlusCmd: false)]
	public class AsptPictureShowLegacyCmd : AsptCmdBase
	{
		[YamlMember(Alias = "Type")]
		public override AsptCmdType Type => AsptCmdType.ShowPicture;

		[YamlMember(Alias = "Block")]
		public override bool IsBlock { get; set; }

		[YamlMember(Alias = "ImagePath")]
		public string ImagePath { get; set; }

		[YamlMember(Alias = "Size")]
		public Vector2 Size { get; set; }

		[YamlMember(Alias = "StartPoint")]
		public Vector2 StartPoint { get; set; }

		[YamlMember(Alias = "Scale", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public Vector2 Scale { get; set; } = Vector2.one;

		[YamlMember(Alias = "Transition")]
		public Transition Transition { get; set; }

		[YamlMember(Alias = "ScaleToWidth", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public bool ScaleToWidth { get; set; } = false;
	}
}
