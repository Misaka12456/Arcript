using Cysharp.Threading.Tasks;
using System.IO;
using System.Text;
using System.Threading;
using Team123it.ProcessForUnity;
using UnityEngine;
using UnityEngine.UI;

namespace System.Enhance.Unity
{
	[RequireComponent(typeof(RawImage))]
	[RequireComponent(typeof(AudioSource))]
	[AddComponentMenu("UI/System Enhance/FFmpeg Video Player (uGUI)")]
	public class FFmpegVideoPlayer : MonoBehaviour
	{
		[Header("General")]
		public RawImage imgVideoTarget;
		public AudioSource sourceAudioTarget;

#if UNITY_EDITOR
		[Header("Integrited Loading")]
		public string VideoPath;
		public string FFmpegPath;
		public bool PlayOnStart = false;
		public float PlayDelay = 0f;
#endif

		private Process ffmpegProcess;
		private string videoFilePath;
		private bool isPlaying = false;

		private Texture2D texVideoMain;
		private MemoryStreamPlus msFFmpegOutput;
		private BinaryReader brFFmpegAudioOutput;
		private long lastFramePos = 0;
		private float lastFrameTime = 0f;
		private byte[] buffer;

		private UniTask taskVideoRender;
		private CancellationTokenSource ctsVideoRender = new CancellationTokenSource();

#if UNITY_EDITOR
		private void Start()
		{
			if (!string.IsNullOrWhiteSpace(VideoPath) && !string.IsNullOrWhiteSpace(FFmpegPath) && PlayOnStart)
			{
				Init(VideoPath, FFmpegPath);
			}
			UniTask.Create(async () =>
			{
				if (PlayDelay > 0f)
				{
					await UniTask.Delay(TimeSpan.FromSeconds(PlayDelay));
				}

				Play();
			});
		}
#endif

		public void Load(string videoPath)
		{
			Init(videoPath);
		}

		private void Init(string videoPath, string ffmpegExecPath = null)
		{
			try
			{
				// 如果ffmpegExecPath，则在{当前目录}/Arcript_Data/Plugins下寻找ffmpeg.exe
				if (!File.Exists(videoPath))
				{
					throw new FileNotFoundException("Video file not found.", videoPath);
				}
				if (string.IsNullOrWhiteSpace(ffmpegExecPath))
				{
					ffmpegExecPath = Path.Combine(Application.dataPath, "Plugins", "ffmpeg.exe");
				}

				if (!File.Exists(ffmpegExecPath))
				{
					throw new UnityException($"Failed to initialize VideoPlayer: ffmpeg.exe not found at {ffmpegExecPath}.");
				}

				videoFilePath = videoPath;

				texVideoMain = new Texture2D(2, 2);
				buffer = new byte[1024 * 1024];

				var cmdBuilder = new StringBuilder($"\"{ffmpegExecPath}\" ");
				cmdBuilder.Append("-i ").Append($"\"{videoPath}\" ");
				cmdBuilder.Append("-f image2pipe ");
				cmdBuilder.Append("-pix_fmt rgba ");
				cmdBuilder.Append("-vcodec png -");

				ffmpegProcess = new Process(cmdBuilder.ToString());
				msFFmpegOutput = new MemoryStreamPlus(buffer);
				ffmpegProcess.RedirectOutput(msFFmpegOutput);

				sourceAudioTarget.clip = GetAudioClip(ffmpegExecPath);

				Debug.Log($"Prepared for playing video {videoPath}.");
			}
			catch (Exception ex)
			{
				Debug.LogError($"An error occurred when initializing VideoPlayer: {ex}");
			}
		}

		public void Play()
		{
			if (ffmpegProcess == null)
			{
				throw new NullReferenceException("VideoPlayer not initialized. Please call Load() first.");
			}

			if (isPlaying)
			{
				Debug.LogWarning("Cannot play video: video is already playing.");
				return;
			}

			taskVideoRender = UniTask.Create(RenderVideo).AttachExternalCancellation(ctsVideoRender.Token);
			sourceAudioTarget.Play();
		}

		private async UniTask RenderVideo()
		{
			for (; ; )
			{
				if (!isPlaying)
				{
					await UniTask.Yield();
					continue;
				}

				float currentTime = Time.realtimeSinceStartup;

				if (currentTime != lastFrameTime)
				{
					int framePos = FindLastFramePosition();
					
					if (framePos < 0 || framePos == lastFramePos)
					{
						await UniTask.Yield();
						continue;
					}

					lastFramePos = framePos;
					lastFrameTime = currentTime;

					byte[] data = (byte[])buffer.PickRange((int)lastFramePos, (int)(buffer.Length - lastFramePos));
					texVideoMain.LoadImage(data);
					imgVideoTarget.texture = texVideoMain;
				}

				await UniTask.Yield();
			}
		}

		private AudioClip GetAudioClip(string ffmpegExecPath)
		{
			var cmdBuilder = new StringBuilder($"\"{ffmpegExecPath}\" ");
			cmdBuilder.Append("-i ").Append($"\"{videoFilePath}\" ");
			cmdBuilder.Append("-f s16le -acodec pcm_s16le -ar 44100 -ac 2 -");

			var audioProc = new Process(cmdBuilder.ToString());
			audioProc.RedirectOutput(new MemoryStreamPlus(buffer));
			audioProc.Start();

			brFFmpegAudioOutput = new BinaryReader(new MemoryStream(buffer, false));
			brFFmpegAudioOutput.ReadBytes(44); // 44 is the header size of wav file
											   // (whatever the audio format is, ffmpeg will always output as wav)

			float[] data = new float[1024];
			int numSamples = 0;

			while (audioProc.IsRunning)
			{
				// Read PCM data from ffmpeg output and convert to float format
				for (int i = 0; i < data.Length; i++)
				{
					short sample = brFFmpegAudioOutput.ReadInt16();
					data[i] = sample / 32768f;
				}

				// Create a new AudioClip with PCM data
				var clip = AudioClip.Create("clip", numSamples + data.Length, 2, 44100, false);
				clip.SetData(data, numSamples);
				sourceAudioTarget.clip = clip;

				numSamples += data.Length;
			}

			return sourceAudioTarget.clip;
		}

		private int FindLastFramePosition()
		{
			// find the last PNG image frame position
			for (int i = buffer.Length - 8; i >= 0; i--)
			{
				if (buffer[i] == 0x89 && buffer[i + 1] == 0x50 && buffer[i + 2] == 0x4E && buffer[i + 3] == 0x47
					&& buffer[i + 4] == 0x0D && buffer[i + 5] == 0x0A && buffer[i + 6] == 0x1A && buffer[i + 7] == 0x0A)
				{
					// {0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A} is the PNG image header
					// (plain text: '\x89PNG\x0D\x0A\x1A\x0A')
					return i;
				}
			}

			return -1;
		}

		private void OnDestroy()
		{
			// clear resources
			ffmpegProcess?.Kill();
			ctsVideoRender?.Cancel();
			ctsVideoRender = new CancellationTokenSource();

			brFFmpegAudioOutput?.Close();
			msFFmpegOutput?.Close();
			Destroy(texVideoMain);

			ffmpegProcess = null;
			taskVideoRender = default;
			brFFmpegAudioOutput = null;
			msFFmpegOutput = null;
			texVideoMain = null;
			lastFramePos = 0;
			lastFrameTime = 0f;
			buffer = Array.Empty<byte>();

			isPlaying = false;
		}
	}
}
