using System.Collections.Generic;
using System.ComponentModel;
using System.Enhance;
using System.Text;
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
		public string TypeStr { get; set; } = "fade";

		[YamlIgnore]
		public TransitionType Type
		{
			get
			{
				var descDict = new Dictionary<string, TransitionType>();
				foreach (TransitionType type in System.Enum.GetValues(typeof(TransitionType)))
				{
					string desc = type.GetDescription();
					descDict.Add(desc, type);
				}
				return descDict[CurveStr];
			}
			set
			{
				string desc = value.GetDescription();
				TypeStr = desc;
			}
		}

		[YamlMember(Alias = "Curve", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public string CurveStr { get; set; } = "linear";

		[YamlIgnore]
		public CurveType Curve
		{
			get
			{
				// 获取所有CurveType的DescriptionAttribute，然后根据字符串对应的DescriptionAttribute的值返回对应的CurveType
				var curveTypeDescDict = new Dictionary<string, CurveType>();
				foreach (CurveType curveType in System.Enum.GetValues(typeof(CurveType)))
				{
					string curveTypeDesc = curveType.GetDescription();
					curveTypeDescDict.Add(curveTypeDesc, curveType);
				}
				return curveTypeDescDict[CurveStr];
			}
			set
			{
				// 获取设置的Curve的Description，然后赋值给CurveStr
				string desc = value.GetDescription();
				CurveStr = desc;
			}
		}

		[YamlMember(Alias = "Duration")]
		public float Duration { get; set; } = 0f;

		public string ToItemShortString()
		{
			// 主指令参数用|分隔，子指令参数用;分隔
			var sb = new StringBuilder("type = ").Append(Type.ToString().ToLower()).Append("; ");
			sb.Append("curve = ").Append(Curve.ToString().ToLower()).Append("; ");
			sb.Append("duration = ").Append(Duration);
			return sb.ToString();
		}
	}

	[AsptCmd(AsptCmdType.ShowPicture, isArptPlusCmd: false)]
	public class AsptPictureShowLegacyCmd : AsptCmdBase
	{
		[YamlMember(Alias = "Type")]
		public override string TypeStr => "show";

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

		public override string ToItemShortString()
		{
			var sb = new StringBuilder("<b>show</b> ");
			sb.Append(ImagePath).Append(" | ");
			sb.Append("size = ").Append(Size).Append(" | ");
			sb.Append("startPoint = ").Append(StartPoint).Append(" | ");
			sb.Append("scale = ").Append(Scale).Append(" | ");
			sb.Append("transition = ").Append(Transition.ToItemShortString()).Append(" | ");
			sb.Append("scaleToWidth = ").Append(ScaleToWidth);
			return sb.ToString();
		}
	}
}
