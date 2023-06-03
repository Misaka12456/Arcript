
using System;
using System.IO;
using NAudio.Wave;
using NVorbis;
using NLayer;
using UnityEngine;
using UnityEngine.Video;
using System.Linq;

namespace Arcript.Utility.Loader
{
	public static class Loader
	{
		private class AudioDataHost
		{
			public float[] audioData;
			public int position = 0;

			public int channels = 2;
			public AudioDataHost(float[] audioData, int channels)
			{
				this.audioData = audioData;
				this.channels = channels;
			}
			public void PCMReaderCallback(float[] buffer)
			{
				if (position <= audioData.Length)
				{
					if (position + buffer.Length <= audioData.Length)
					{
						Array.Copy(audioData, position, buffer, 0, buffer.Length);
					}
					else
					{
						Array.Copy(audioData, position, buffer, 0, audioData.Length - position);
					}
				}
				position += buffer.Length;
			}
			public void PCMSetPositionCallback(int newPosition)
			{
				position = newPosition * channels;
			}
		}
		// This is used to load audio files that supported by NLayer like mp3
		public static AudioClip LoadMp3AudioFile(string path)
		{
			float[] audioData = null;
			int channels = 0;
			int sampleRate = 0;
			try
			{
				using (var file = new MpegFile(path))
				{
					//Note: to be simple, we do not load large file with samples count larger than int limit
					//This will be enough for most file, especially the sound effects in the skin folder
					if (file.Length > 0x7FFFFFFFL * sizeof(float))
					{
						return null;
					}
					float[] data = new float[file.Length / sizeof(float)];
					file.ReadSamples(data, 0, (int)(file.Length / sizeof(float)));
					channels = file.Channels;
					sampleRate = file.SampleRate;
					audioData = data;
				}
			}
			catch
			{
				return null;
			}
			if (audioData == null)
			{
				return null;
			}
			var dataHost = new AudioDataHost(audioData, channels);
			var clip = AudioClip.Create(path, audioData.Length / channels, channels, sampleRate, true, dataHost.PCMReaderCallback, dataHost.PCMSetPositionCallback);
			return clip;
		}
		// This is used to load audio files that supported by NAudio like wav/mp3(on windows)
		public static AudioClip LoadWavOrMp3AudioFile(string path)
		{
			float[] audioData = null;
			int channels = 0;
			int sampleRate = 0;
			try
			{
				using (var reader = new AudioFileReader(path))
				{
					//Note: to be simple, we do not load large file with samples count larger than int limit
					//This will be enough for most file, especially the sound effects in the skin folder
					if (reader.Length > 0x7FFFFFFFL * sizeof(float))
					{
						return null;
					}
					float[] data = new float[reader.Length / sizeof(float)];
					reader.Read(data, 0, (int)(reader.Length / sizeof(float)));
					channels = reader.WaveFormat.Channels;
					sampleRate = reader.WaveFormat.SampleRate;
					audioData = data;
				}
			}
			catch
			{
				return null;
			}
			if (audioData == null)
			{
				return null;
			}
			var dataHost = new AudioDataHost(audioData, channels);
			var clip = AudioClip.Create(path, audioData.Length / channels, channels, sampleRate, true, dataHost.PCMReaderCallback, dataHost.PCMSetPositionCallback);
			return clip;
		}
		// This is used to load audio files that supported by NVorbis like ogg
		public static AudioClip LoadOggAudioFile(string path)
		{
			float[] audioData = null;
			int channels = 0;
			int sampleRate = 0; try
			{
				using (var reader = new VorbisReader(path))
				{
					//Note: Same here
					if (reader.TotalSamples * reader.Channels > 0x7FFFFFFFL)
					{
						return null;
					}
					float[] data = new float[reader.TotalSamples * reader.Channels];
					reader.ReadSamples(data, 0, (int)(reader.TotalSamples * reader.Channels));
					channels = reader.Channels;
					sampleRate = reader.SampleRate;
					audioData = data;
				}
			}
			catch
			{
				return null;
			}
			if (audioData == null)
			{
				return null;
			}
			var dataHost = new AudioDataHost(audioData, channels);
			var clip = AudioClip.Create(path, audioData.Length / channels, channels, sampleRate, true, dataHost.PCMReaderCallback, dataHost.PCMSetPositionCallback);
			return clip;
		}
		// This try all decoders used in arcade

		public static VideoPlayer LoadBGA(string bgaPath)
		{
			if (File.Exists(bgaPath))
			{
				GameObject.Find("ArcaeaPlayer/Playground/Background/BGA").SetActive(true);
				var bgaPlayer = GameObject.Find("ArcaeaPlayer/Playground/Background/BGA").GetComponent<VideoPlayer>();
				bgaPlayer.source = VideoSource.Url;
				bgaPlayer.url = bgaPath;
				bgaPlayer.enabled = true;
				bgaPlayer.Prepare();
				return bgaPlayer;
			}
			else
			{
				return null;
			}
		}
		public static AudioClip LoadAudioFile(string path)
		{
			AudioClip clip = null;
			clip = LoadOggAudioFile(path);
			if (clip != null)
			{
				return clip;
			}
			clip = LoadMp3AudioFile(path);
			if (clip != null)
			{
				return clip;
			}
			clip = LoadWavOrMp3AudioFile(path);
			if (clip != null)
			{
				return clip;
			}
			return clip;
		}

		public static Texture2D LoadTexture2D(string path)
		{
			byte[] file;
			try
			{
				file = File.ReadAllBytes(path);
			}
			catch
			{
				return null;
			}
			//TODO: Completly remove mipmap after GPU optimize
			var texture = new Texture2D(1, 1);
			bool success = ImageConversion.LoadImage(texture, file, true);
			if (success)
			{
				texture.wrapMode = TextureWrapMode.Clamp;
				texture.name = path;
				texture.mipMapBias = -4;
				return texture;
			}
			else
			{
				UnityEngine.Object.Destroy(texture);
				return null;
			}
		}

		public static Texture2D LoadTexture2D(string textureName, byte[] data)
		{
			var texture = new Texture2D(1, 1);
			bool success = ImageConversion.LoadImage(texture, data, true);
			if (success)
			{
				texture.wrapMode = TextureWrapMode.Clamp;
				texture.name = textureName;
				texture.mipMapBias = -4;
				return texture;
			}
			else
			{
				UnityEngine.Object.Destroy(texture);
				return null;
			}
		}

		public static Sprite LoadSprite(string path)
		{
			var texture = LoadTexture2D(path);
			if (texture == null)
			{
				return null;
			}
			var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
			return sprite;
		}
	}
}