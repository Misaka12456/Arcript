using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using YamlDotNet.Serialization;

namespace Arcript.Aspt.QuickFuncs
{

	public enum RelativePosGroupType
	{
		Any = 0,
		[Description("x")] X = 1,
		[Description("y")] Y = 2,
	}

	[Description("position")]
	public class RelativePosGroup
	{
		public RelativePosGroupType Type { get; set; } = RelativePosGroupType.Any;
		public Dictionary<int, Vector2> RelativePos { get; set; }
	}

	public class QuickMaskImgTemplate
	{
		[YamlMember(Alias = "Size")]
		public Vector2 Size { get; set; }

		[YamlMember(Alias = "RStartPoint", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public Vector2 RelativeStartPoint { get; set; } = Vector2.zero;

		[YamlMember(Alias = "RStartPoints", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public RelativePosGroup[] RelativeStartPoints { get; set; } = Array.Empty<RelativePosGroup>();

		[YamlMember(Alias = "Scale")]
		public Vector2 Scale { get; set; }

		[YamlMember(Alias = "IgnoreColor")]
		public Color IgnoreColor { get; set; } = new Color(1, 1, 1, 0);
	}
	public class QuickMask
	{
		[YamlMember(Alias = "FilePath")]
		public string MaskImagePath { get; set; }

		[YamlMember(Alias = "UnderlayerMaskPath")]
		public string AlphaOnlyUnderlayerMaskPath { get; set; }

		[YamlMember(Alias = "MaskName")]
		public string Name { get; set; }

		[YamlMember(Alias = "MaskSize")]
		public Vector2 Size { get; set; }

		[YamlMember(Alias = "OptionalArgs", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public string[] OptionalArgs { get; set; } = Array.Empty<string>();

		[YamlMember(Alias = "ImageTemplate")]
		public QuickMaskImgTemplate ImageTemplate { get; set; }
	}
}