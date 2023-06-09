using FFmpeg.AutoGen;
using UnityEngine;

namespace System.Enhance.Unity
{
	public static class VideoPlayPlusExtension
	{
		public static RenderTextureFormat ToRTexFormat(this AVPixelFormat ffVideoFmt)
		{
			string ffVideoFmtStr = ffmpeg.av_get_pix_fmt_name(ffVideoFmt);
			if (string.IsNullOrWhiteSpace(ffVideoFmtStr))
			{
				return RenderTextureFormat.Default; // == RenderTextureFormat.Unknown
			}
			return ffVideoFmtStr switch
			{
				"rgba" => RenderTextureFormat.ARGB32,
				"bgra" => RenderTextureFormat.BGRA32,
				"rgb24" => RenderTextureFormat.ARGB32,
				"bgr24" => RenderTextureFormat.BGRA32,
				_ => RenderTextureFormat.Default
			};
		}

		public static bool Clear(this RenderTexture rTex, Color clearColor, bool avoidBlink = false)
		{
			if (rTex == null) return false;

			var prevRt = RenderTexture.active; // 保存先前的活动RenderTexture，防止影响到其它逻辑
			
			RenderTexture.active = rTex;
			if (!avoidBlink)
			{
				GL.Clear(true, true, clearColor);
			}
			else
			{
				GL.Clear(false, true, clearColor);
			}

			RenderTexture.active = prevRt; // 恢复先前的活动RenderTexture

			return true;
		}
	}
}
