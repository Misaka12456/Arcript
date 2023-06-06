using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using YamlDotNet.Core;
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
		[YamlMember(Alias = "Type")]
		public string TypeStr { get; set; }
		
		[YamlIgnore]
		public RelativePosGroupType Type { get; set; } = RelativePosGroupType.Any;
		
		[YamlIgnore]
		public Dictionary<int, Vector2> RelativePos { get; set; }
	}

	public class QuickMaskImgTemplate
	{
		[YamlMember(Alias = "Size")]
		public Vector2 Size { get; set; }

		[YamlMember(Alias = "RStartPoint", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public Vector2 RelativeStartPoint { get; set; } = Vector2.zero;

		[YamlMember(Alias = "RStartPoints", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public Dictionary<string, object>[] RelativeStartPointsArray { get; set; } = Array.Empty<Dictionary<string, object>>();

		[YamlIgnore]
		public RelativePosGroup[] RelativeStartPoints
		{
			get
			{
				var r = new List<RelativePosGroup>();
				foreach (var rsp in RelativeStartPointsArray)
				{
					var rpg = new RelativePosGroup()
					{
						TypeStr = rsp["Type"] as string,
					};

					var rPos = rsp.Where(pair => int.TryParse(pair.Key, out _)).ToDictionary(pair => int.Parse(pair.Key), pair =>
					{
						var posArr = pair.Value as float[];
						return new Vector2(posArr[0], posArr[1]);
					});

					rpg.RelativePos = rPos;

					r.Add(rpg);
				}
				return r.ToArray();
			}
			set
			{
				var r = new List<Dictionary<string, object>>();
				foreach (var rpg in value)
				{
					var rsp = new Dictionary<string, object>()
					{
						{ "Type", rpg.TypeStr },
					};
					
					foreach (var rPos in rpg.RelativePos)
					{
						rsp.Add(rPos.Key.ToString(), new float[] { rPos.Value.x, rPos.Value.y });
					}

					r.Add(rsp);
				}

				RelativeStartPointsArray = r.ToArray();
			}
		}

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

		[YamlMember(Alias = "Size")]
		public Vector2 Size { get; set; }

		[YamlMember(Alias = "OptionalArgs", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public string[] OptionalArgs { get; set; } = Array.Empty<string>();

		[YamlMember(Alias = "ImageTemplate")]
		public QuickMaskImgTemplate ImageTemplate { get; set; }
	}
}