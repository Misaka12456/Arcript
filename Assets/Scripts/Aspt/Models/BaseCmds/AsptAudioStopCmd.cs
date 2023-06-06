using System.Collections.Generic;
using System.Enhance;
using System.Text;
using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	[AsptCmd(AsptCmdType.StopAudio)]
	public class AsptAudioStopCmd : AsptCmdBase
	{
		[YamlMember(Alias = "Type")]
		public override string TypeStr { get; set; } = "stop";

		[YamlMember(Alias = "Block")]
		public override bool IsBlock { get; set; }

		[YamlMember(Alias = "AudioPath")]
		public string AudioPath { get; set; }

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
		[YamlMember(Alias = "Duration", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public float Duration { get; set; } = 0f;

		public override string ToItemShortString()
		{
			var sb = new StringBuilder("<b>stop</b> ");
			sb.Append(AudioPath).Append(" | ");
			// curve导出为不包含"CurveType"的字符串
			sb.Append("curve = ").Append(Curve.ToString().ToLower()).Append(" | ");
			sb.Append("duration = ").Append(Duration);
			return sb.ToString();
		}
	}
}
