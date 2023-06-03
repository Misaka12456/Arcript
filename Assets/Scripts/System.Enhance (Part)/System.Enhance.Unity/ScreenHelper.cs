using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace System.Enhance.Unity
{
	public static class ScreenHelper
	{
		/// <summary>
		/// 将当前 <see cref="Resolution"/> 实例所表示的分辨率转换为常见的分辨率字符串。<br />
		/// Convert the resolution represented by current <see cref="Resolution"/> instance to common resolution string.
		/// </summary>
		/// <param name="resolution">
		/// 当前 <see cref="Resolution"/> 实例。<br />
		/// Current <see cref="Resolution"/> instance.
		/// </param>
		/// <returns>
		/// 格式为"宽 x 高"的分辨率字符串(单位:像素(px))。<br />
		/// Resolution string in "Width x Height" format (in pixels(px)).
		/// </returns>
		public static string ToCommonString(this Resolution resolution)
		{
			var sb = new StringBuilder();
			sb.Append(resolution.width).Append('x').Append(resolution.height);
			return sb.ToString();
		}

		/// <summary>
		/// 判断当前 <see cref="Resolution"/> 实例所表示的分辨率是否为给定的常用分辨率之一。<br />
		/// Judge whether the resolution represented by current <see cref="Resolution"/> instance is one of the given resolutions.
		/// </summary>
		/// <param name="resolution">
		/// 当前 <see cref="Resolution"/> 实例。<br />
		/// Current <see cref="Resolution"/> instance.
		/// </param>
		/// <param name="commonResolutions">
		/// 格式为"宽 x 高"并以像素(px)为单位的常用分辨率字符串列表。<br />
		/// List of Common Resolution strings in "Width x Height" format (in pixels(px)).
		/// </param>
		/// <param name="matched">
		/// 在当前方法返回时, 若判断结果为真则返回符合的常用分辨率 <see cref="Resolution"/> 实例; 否则返回 <see cref="Resolution" /> 类型的默认值。此参数未经初始化即经传递。<br />
		/// When the current method returns, if the judgment result is true, the matching common resolution <see cref="Resolution"/> instance is returned;<br />
		/// Otherwise, the default value of type <see cref="Resolution"/> is returned.<br />
		/// This parameter is passed uninitialized.
		/// </param>
		/// <returns>
		/// 判断结果。<br />
		/// Judgment result.
		/// </returns>
		/// <exception cref="FormatException" />
		public static bool IsCommonResolution(this Resolution resolution, IEnumerable<string> commonResolutions, out Resolution matched)
		{
			var commonResos = new List<Resolution>();
			foreach (string commonReso in commonResolutions)
			{
				var regex = new Regex(@"(?<width>^[0-9]\d+)x(?<height>[0-9]\d+)$"); // 正则表达式(匹配"整数 x 整数"的字符串, 前者整数记为命名组"width", 后者记为命名组"height")
				var match = regex.Match(commonReso);
				if (match.Success)
				{
					commonResos.Add(new Resolution()
					{
						width = int.Parse(match.Groups["width"].Value),
						height = int.Parse(match.Groups["height"].Value),
						refreshRate = Screen.currentResolution.refreshRate
					});
				}
				else
				{
					throw new FormatException($"Common Resolution String \"{commonReso}\" is not in a correct format. Correct format: \"Width x Height\"");
				}
			}
			var r = from commonReso in commonResos
					where commonReso.width == resolution.width && commonReso.height == resolution.height
					select commonReso;
			matched = r.FirstOrDefault();
			return r.Any();
		}

		public static Vector2 GUIToScreenCoordinate(this Vector2 guiCoordinate)
		{
			float y = Screen.height - guiCoordinate.y;
			return new Vector2(guiCoordinate.x, y);
		}
		public static Rect ResizeRect(this Rect original, Vector2 descResolution, ResizeType originalType = ResizeType.From1280720)
		{
			float scaleRatio = originalType switch
			{
				ResizeType.From1280720 => descResolution.x / 1280f,
				ResizeType.From19201080 => descResolution.x / 1920f,
				_ => 1f
			};
			var rect = new Rect(original.x * scaleRatio, original.y * scaleRatio, original.width * scaleRatio, original.height * scaleRatio);
			DebugChan.Log($"original=" + original.ToString());
			DebugChan.Log($"rect=" + rect.ToString());
			return rect;
		}
	}

	public enum ResizeType
	{
		From1280720,
		From19201080
	}
}
