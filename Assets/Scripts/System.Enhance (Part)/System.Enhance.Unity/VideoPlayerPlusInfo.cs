using UnityEngine;
using FFmpeg.AutoGen;                                   // for video info (FFmpeg)
using System.IO;
using System.Runtime.InteropServices;

namespace System.Enhance.Unity
{
	public struct VideoPlayerPlusInfo
	{
		#region Video Info
		public Vector2Int VideoSize;
		public RenderTextureFormat VideoFormat;
		public float VideoFPS;
		#endregion
		#region Audio Info
		public int AudioSampleRate;
		public int AudioChannels;
		#endregion

		public unsafe VideoPlayerPlusInfo(AVStream* pVideoStream, AVStream* pAudioStream)
		{
			var r = ReadInfo(pVideoStream, pAudioStream);
			VideoSize = r.Item1;
			VideoFormat = r.Item2;
			VideoFPS = r.Item3;
			AudioSampleRate = r.Item4;
			AudioChannels = r.Item5;
		}

		public unsafe VideoPlayerPlusInfo(AVFormatContext* pFormatCtx, bool closeAfterInit = false)
		{
			try
			{
				if (TryFindStreams(pFormatCtx, out var pStreamVideo, out bool hasAudio, out var pStreamAudio))
				{
					var r = ReadInfo(pStreamVideo, hasAudio ? pStreamAudio : null);
					VideoSize = r.Item1;
					VideoFormat = r.Item2;
					VideoFPS = r.Item3;
					AudioSampleRate = r.Item4;
					AudioChannels = r.Item5;
				}
				else
				{
					closeAfterInit = true; // 因为下方会throw一个异常，所以这里要确保finally里面能够关闭
										   // (否则一旦调用方没有catch或finally将会导致永远无法关闭，从而导致内存泄漏)
					throw new TypeInitializationException(nameof(VideoPlayerPlusInfo), new ApplicationException("Cannot find video stream."));
				}
			}
			finally
			{
				if (closeAfterInit)
				{
					ffmpeg.avformat_close_input(&pFormatCtx);
				}
			}
		}

		public VideoPlayerPlusInfo(string videoFilePath)
		{
			if (!File.Exists(videoFilePath))
			{
				throw new FileNotFoundException($"Cannot find video file: {videoFilePath}");
			}

			try
			{
				unsafe
				{
					AVFormatContext* pFormatCtx = null;
					int ret = ffmpeg.avformat_open_input(&pFormatCtx, videoFilePath, null, null);
					try
					{
						if (ret < 0)
						{
							throw new SEHException($"Cannot open input file: {videoFilePath} ({ret})");
						}

						if (TryFindStreams(pFormatCtx, out var pStreamVideo, out bool hasAudio, out var pStreamAudio))
						{
							var r = ReadInfo(pStreamVideo, hasAudio ? pStreamAudio : null);
							VideoSize = r.Item1;
							VideoFormat = r.Item2;
							VideoFPS = r.Item3;
							AudioSampleRate = r.Item4;
							AudioChannels = r.Item5;
						}
						else
						{
							throw new TypeInitializationException(nameof(VideoPlayerPlusInfo), new ApplicationException("Cannot find video stream."));
						}
					}
					finally
					{
						ffmpeg.avformat_close_input(&pFormatCtx);
					}
				}
			}
			catch (SEHException ex)
			{
				throw new TypeInitializationException(nameof(VideoPlayerPlusInfo), ex);
			}
			catch (TypeInitializationException)
			{
				throw;
			}
		}

		private static unsafe (Vector2Int, RenderTextureFormat, float, int, int) ReadInfo(AVStream* pVideoStream, AVStream* pAudioStream = null)
		{
			#region Video Info
			float fps = (float)ffmpeg.av_q2d(pVideoStream->avg_frame_rate);
			int width = pVideoStream->codecpar->width;
			int height = pVideoStream->codecpar->height;
			var ffVideoFmt = (AVPixelFormat)pVideoStream->codecpar->format;

			var videoSize = new Vector2Int(width, height);
			var videoFormat = ffVideoFmt.ToRTexFormat();
			var videoFPS = fps;
			#endregion
			#region Audio Info
			var audioSampleRate = pAudioStream != null ? pAudioStream->codecpar->sample_rate : -1;
			var audioChannels = pAudioStream != null ? pAudioStream->codecpar->ch_layout.nb_channels : -1;
			#endregion
			return (videoSize, videoFormat, videoFPS, audioSampleRate, audioChannels);
		}

		private static unsafe bool TryFindStreams(AVFormatContext* pFormatCtx, out AVStream* pStreamVideo, out bool audioExists, out AVStream* pStreamAudio)
		{
			pStreamVideo = null;
			pStreamAudio = null;
			audioExists = false;

			int ret = ffmpeg.avformat_find_stream_info(pFormatCtx, null);
			if (ret < 0)
			{
				Debug.LogError($"[VideoPlayer+][VideoPlayerPlusInfo] Cannot find stream information");
				ffmpeg.avformat_close_input(&pFormatCtx);
				return false;
			}

			for (int i = 0; i < pFormatCtx->nb_streams; i++)
			{
				AVStream* pStream = pFormatCtx->streams[i];
				if (pStream->codecpar->codec_type == AVMediaType.AVMEDIA_TYPE_VIDEO)
				{
					pStreamVideo = pStream;
				}
				else if (pStream->codecpar->codec_type == AVMediaType.AVMEDIA_TYPE_AUDIO)
				{
					audioExists = true;
					pStreamAudio = pStream;
				}
			}
			
			if (pStreamVideo == null)
			{
				ffmpeg.avformat_close_input(&pFormatCtx);
				Debug.LogError($"[VideoPlayer+][VideoPlayerPlusInfo] Cannot find video stream");
				return false;
			}

			return true;
		}
	}
}