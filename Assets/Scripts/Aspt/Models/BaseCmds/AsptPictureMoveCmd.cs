using System;
using System.Collections.Generic;
using System.Enhance;
using System.Text;
using UnityEngine;
using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	[AsptCmd(AsptCmdType.MovePicture)]
	public class AsptPictureMoveCmd : AsptCmdBase
	{
		[YamlMember(Alias = "Type")]
		public override string TypeStr { get; set; } = "move";

		[YamlMember(Alias = "Block")]
		public override bool IsBlock { get; set; }

		[YamlMember(Alias = "ImagePath")]
		public string ImagePath { get; set; }

		[YamlMember(Alias = "MoveDelta")]
		public Vector2 MoveDelta { get; set; }

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
		public float Duration { get; set; } = 0f; // 0s == immediately move (looks like 'flash')

		#region Events (Optional)
		[YamlIgnore]
		public override AsptCmdBase[] EventBeforeExec { get => EventBeforeMove; set => EventBeforeMove = value; }

		[YamlIgnore]
		public override AsptCmdBase[] EventAfterExec { get => EventAfterMove; set => EventAfterMove = value; }

		[YamlMember(Alias = "EBeforeMove", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public AsptCmdBase[] EventBeforeMove { get; set; } = Array.Empty<AsptCmdBase>();

		[YamlMember(Alias = "EAfterMove", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public AsptCmdBase[] EventAfterMove { get; set; } = Array.Empty<AsptCmdBase>();
		#endregion

		public override string ToItemShortString()
		{
			var sb = new StringBuilder("<b>move</b> ");
			sb.Append(ImagePath).Append(" | ");
			sb.Append("delta = ").Append(MoveDelta).Append(" | ");
			sb.Append("curve = ").Append(Curve.ToString().ToLower()).Append(" | ");
			sb.Append("duration = ").Append(Duration);
			if (EventBeforeMove.Length > 0)
			{
				sb.Append(" | ").Append("[Event] BeforeMove = ").Append(EventBeforeMove.Length);
			}
			if (EventAfterMove.Length > 0)
			{
				sb.Append(" | ").Append("[Event] AfterMove = ").Append(EventAfterMove.Length);
			}
			return sb.ToString();
		}
	}
}
