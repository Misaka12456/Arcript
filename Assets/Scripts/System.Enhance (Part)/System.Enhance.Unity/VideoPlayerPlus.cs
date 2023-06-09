#pragma warning disable IDE1006
using UnityEngine;
using UnityEngine.UI;                                   // for ugui components
using VideoPlayerv1 = UnityEngine.Video.VideoPlayer;    // for video player
using Cysharp.Threading.Tasks;                          // for async/await (UniTask)
using UnityEngine.Video;
using System.Text;
using UnityEngine.Events;
using System.Threading;
using System.Collections;
using DG.Tweening;
#if UNITY_EDITOR
#endif

namespace System.Enhance.Unity
{
	/// <summary>
	/// A enhanced UGUI video player (Video Player+) that can play video from url or local path.
	/// </summary>
	[RequireComponent(typeof(VideoPlayerv1))] // main player
	[RequireComponent(typeof(RawImage))] // video target
	[RequireComponent(typeof(AudioSource))] // audio target
	public class VideoPlayerPlus : MonoBehaviour
	{
		/// <summary>
		/// Is the video playing?
		/// </summary>
		public bool isPlaying => m_isPlaying;

		/// <summary>
		/// The path or the url of the video.
		/// </summary>
		[Header("General")]
		public string urlVideoPath;

		/// <summary>
		/// Basic Unity <see cref="VideoPlayerv1"/> component.
		/// </summary>
		private VideoPlayerv1 m_playerParent;

		/// <summary>
		/// The <see cref="RawImage"/> component to display the video <see cref="RenderTexture"/>.
		/// </summary>
		private RawImage m_imgVideoTarget;

		/// <summary>
		/// The <see cref="AudioSource"/> component to play the video audio.
		/// </summary>
		private AudioSource m_sourceAudioTarget;

		/// <summary>
		/// The core <see cref="RenderTexture"/> of the video.<br />
		/// Video is not played directly to the UI; instead, it is played to this <see cref="RenderTexture"/>, and then displayed by the <see cref="RawImage"/> UGUI component.
		/// </summary>
		private RenderTexture m_rtVideoBaseTarget;

		/// <summary>
		/// The color of the background image before the video starts playing.<br />
		/// In 2nd development features enabled status, the whole display component will firstly show this color,
		/// and then fade to <see cref="beforeStartToColor" /> (which should has the same color as the 1st frame of the video).<br />
		/// For more information, see <seealso cref="beforeFadeInDuration"/>.
		/// </summary>
		[Header("Secondary Development")]
		[SerializeField] private Color beforeStartFromColor;

		/// <summary>
		/// The color that will be faded to before actually video playing.<br />
		/// Color should be the same as the 1st frame of the video for switching smoothness.<br />
		/// Video will started playing from the background with this color in the 2nd development features enabled status.<br />
		/// For more information, see <seealso cref="beforeFadeInDuration"/>.
		/// </summary>
		[SerializeField] private Color beforeStartToColor;

		/// <summary>
		/// The color that will be faded to after the whole video finished playing.<br />
		/// Color should be the same as the last frame of the video for switching smoothness.<br />
		/// Video will stopped playing from this background before it will be finished. <br />
		/// For more information, see <seealso cref="afterFadeOutDuration"/>.
		/// </summary>
		[SerializeField] private Color afterEndFromColor;

		/// <summary>
		/// The color of the background image after the video finished playing.<br />
		/// Usually this is the same color of <see cref="beforeStartFromColor" />.<br />
		/// After the fading action, the video displayer <see cref="RawImage"/> will be turning transparent, and then the background with this color will be automatically shown.<br />
		/// <br />
		/// For more information, see <seealso cref="afterFadeOutDuration"/>.
		/// </summary>
		[SerializeField] private Color afterEndToColor;

		/// <summary>
		/// The duration of the fading action before the video starts playing.<br />
		/// In 2nd development features enabled status, the whole display component will firstly show <see cref="beforeStartFromColor" />,
		/// and then fade to <see cref="beforeStartToColor" /> in this duration.<br />
		/// <para>
		/// Diagram: <br />
		/// c0: <see cref="beforeStartToColor"/>; c1: <see cref="beforeStartFromColor"/>; t: <see cref="beforeFadeInDuration"/>; f: the very first frame of the video<br />
		/// <br />
		/// <code>
		/// c0 ----------- c1<br />
		///       &lt;t&gt;       f --(play started)--&gt;
		/// </code>
		/// As you can see, the first frame of the video is very near to the color <see cref="beforeStartToColor"/>,
		/// so that's why <see cref="beforeStartToColor"/> should have same or at least similar color with the first frame of the video;<br />
		/// or the switching action will be sharp and obvious.
		/// </para>
		/// </summary>
		[SerializeField] private float beforeFadeInDuration;

		/// <summary>
		/// The duration of the fading action after the video finished playing.<br />
		/// In 2nd development features enabled status, the whole display component will fadly show <see cref="afterEndFromColor" />,
		/// and then fade to <see cref="afterEndToColor" /> in this duration (by fading the video raw image to transparent).<br />
		/// The core video render texture will be cleared by the same color and the video displayer raw image will be instantly shown after the actions above.<br />
		/// <para>
		/// Diagram: <br />
		/// f: the last frame of the video;<br />
		/// c8: <see cref="afterEndFromColor"/>;<br />
		/// c9: <see cref="afterEndToColor"/>;<br />
		/// cn: the normal color of the render texture (usually black);<br />
		/// ct: the transparent color;<br />
		/// |: the end of the whole operation<br />
		/// <br />
		/// <code>
		/// Bg Layer:    ---- c8 ---------------- c9 - ct ------------- |<br />
		/// Video Layer: ---- f ----------------- ct - cn ------------- |<br />
		/// Video Color: ----&lt;as normal&gt;--------- ct - c9 ------------- |<br />
		/// </code>
		/// Finally the background will be hidden (<c>ct</c>) and the video displayer show as normal, which is totally the prepared state before the video starts playing.
		/// </para>
		/// </summary>
		[SerializeField] private float afterFadeOutDuration;
		[SerializeField] private Image m_imgBackground;

		/// <summary>
		/// Occurs when the video is ready to play.<br />
		/// Params: <br />
		/// <list type="number">
		/// <item>videoPath: the path of the video file</item>
		/// <item>videoSize: the size of the video (width, height)</item>
		/// <item>videoFps: the fps of the video</item> 
		/// <item>videoLength: the length of the video (in ms)</item>
		/// </list>
		/// </summary>
		[Header("Events")]
		public UnityEvent<string, Vector2, float, long> onVideoReady; // params[0] = videoPath, [1] = videoSize, [2] = videoFps, [3] = videoLength (in ms)
		
		public UnityEvent<string, long> onVideoPlayFinish; // params[0] = videoPath, [1] = videoLength (in ms)
		private bool m_isReady = false;
		private bool m_isPlaying = false;

		private void Awake()
		{
			m_playerParent = GetComponent<VideoPlayerv1>();
			m_imgVideoTarget = GetComponent<RawImage>();
			m_sourceAudioTarget = GetComponent<AudioSource>();

			onVideoReady ??= new UnityEvent<string, Vector2, float, long>();
			onVideoPlayFinish ??= new UnityEvent<string, long>();
		}

		public bool Load(string videoPath)
		{
			bool success = true;
			try
			{
				urlVideoPath = videoPath;
				#region Player Settings
				// Parent Player Settings
				m_playerParent.playOnAwake = false;
				m_playerParent.isLooping = false;
				if (m_playerParent.canSetSkipOnDrop)
				{
					m_playerParent.skipOnDrop = true; // 允许通过跳帧来同步时间(即"音频同步")
				}
				m_playerParent.playbackSpeed = 1f; // 播放速度
				m_playerParent.renderMode = VideoRenderMode.RenderTexture;
				m_playerParent.aspectRatio = VideoAspectRatio.FitInside; // "内部适应"(即"等比例缩放填充")
				m_playerParent.audioOutputMode = VideoAudioOutputMode.AudioSource;
				
				m_playerParent.source = VideoSource.Url;
				m_playerParent.url = videoPath;

				// Video
				m_rtVideoBaseTarget = new RenderTexture(Screen.width, Screen.height, 0)
				{
					name = $"VideoPlayer+_BaseRT_{videoPath}"
				};
				m_rtVideoBaseTarget.Create();
				m_playerParent.targetTexture = m_rtVideoBaseTarget;

				// Audio
				m_playerParent.controlledAudioTrackCount = 1;
				m_playerParent.SetTargetAudioSource(0, m_sourceAudioTarget);
				m_playerParent.Prepare();
				#endregion
				#region UGUI Components Inits
				// Video (RawImage)
				m_imgVideoTarget.texture = m_rtVideoBaseTarget;
				m_playerParent.seekCompleted += (player) =>
				{
					onVideoPlayFinish?.Invoke(urlVideoPath, (long)(m_playerParent.length * 1000));
				};

				// Audio (AudioSource)
				m_sourceAudioTarget.volume = 1f;
				m_sourceAudioTarget.loop = false;
				#endregion
				#region Asyncly Wait Until Prepared
				UniTask.Create(async () =>
				{
					await UniTask.WaitUntil(() => m_playerParent.isPrepared);
					m_isReady = true;
					var sb = new StringBuilder($"[VideoPlayer+] Video is ready (").Append(m_playerParent.width).Append("x").Append(m_playerParent.height)
						.Append('@').Append(m_playerParent.frameRate).Append("fps, ").Append(m_playerParent.length).Append("s");
					if (m_playerParent.audioTrackCount > 0)
					{
						sb.Append(" with audio@").Append(m_playerParent.GetAudioSampleRate(0)).Append("Hz");
					}
					Debug.Log(sb.ToString());
					onVideoReady?.Invoke(urlVideoPath, new Vector2(m_playerParent.width, m_playerParent.height), m_playerParent.frameRate, (long)(m_playerParent.length * 1000));
				});
				#endregion
				return true;
			}
			catch (Exception ex)
			{
				success = false;
				Debug.LogError($"[VideoPlayer+] An error occurred while loading video: {ex}");
				return false;
			}
			finally
			{
				if (!success) CloseInternal();
			}
		}

		/// <summary>
		/// Play the video.
		/// </summary>
		public void Play()
		{
			if (!m_playerParent.isPrepared)
			{
				Debug.LogError("[VideoPlayer+] Please load a video first.");
				return;
			}
			if (!m_isReady)
			{
				Debug.LogError("[VideoPlayer+] Video is still loading, please wait patiently.");
				return;
			}
			if (m_playerParent.audioTrackCount == 0)
			{
				Debug.LogWarning("[VideoPlayer+] The video does not contain audio stream, so any audio settings will be ignored.");
			}
			m_playerParent.Play();
			m_isPlaying = true;
		}

		/// <summary>
		/// Play the video with secondary development config enabled.
		/// </summary>
		public async UniTask PlayPlusAsync()
		{
			if (!m_playerParent.isPrepared)
			{
				Debug.LogError("[VideoPlayer+] Please load a video first.");
				return;
			}
			if (!m_isReady)
			{
				Debug.LogError("[VideoPlayer+] Video is still loading, please wait patiently.");
				return;
			}
			if (m_playerParent.audioTrackCount == 0)
			{
				Debug.LogWarning("[VideoPlayer+] The video does not contain audio stream, so any audio settings will be ignored.");
			}

			if (afterEndToColor == null) afterEndToColor = beforeStartFromColor;

			// 设置视频目标RawImage为Transparent
			m_imgVideoTarget.color = new Color(1, 1, 1, 0);

			if (beforeFadeInDuration < 0)
			{
				m_playerParent.Play();
			}
			
			// 从黑色淡入背景
			m_imgBackground.color = beforeStartFromColor;
			m_imgBackground.DOColor(beforeStartToColor, Mathf.Abs(beforeFadeInDuration));
			await UniTask.Delay((int)(Mathf.Abs(beforeFadeInDuration) * 1000));

			if (beforeFadeInDuration >= 0)
			{
				m_playerParent.Play();
			}
			
			await UniTask.DelayFrame(1);
			m_imgVideoTarget.color = Color.white;
			
			if (afterFadeOutDuration < 0)
			{
				await UniTask.WaitUntil(() => m_playerParent.time >= m_playerParent.length);
				m_playerParent.Stop();
				await UniTask.Delay(TimeSpan.FromSeconds(Mathf.Abs(afterFadeOutDuration)));
			}
			else
			{
				await UniTask.WaitUntil(() => m_playerParent.time >= (m_playerParent.length - afterFadeOutDuration));
			}

			// 从视频淡出到背景(结束颜色)
			m_imgBackground.color = afterEndFromColor;
			m_imgVideoTarget.DOColor(new Color(1, 1, 1, 0), afterFadeOutDuration / 2f);
			await UniTask.Delay((int)(afterFadeOutDuration * 500));

			// 设置视频目标RawImage为Transparent
			await UniTask.DelayFrame(1);
			if (afterFadeOutDuration > 0)
			{
				m_playerParent.Stop();
			}
			m_imgVideoTarget.color = new Color(1, 1, 1, 0);

			// 从结束颜色的背景淡入到黑色
			m_imgBackground.DOColor(afterEndToColor, afterFadeOutDuration / 2f);
			await UniTask.Delay((int)(afterFadeOutDuration * 500));

			// 清空视频目标RawImage并将颜色复原，然后隐藏掉背景
			m_rtVideoBaseTarget.Clear(afterEndToColor);
			m_imgVideoTarget.color = Color.white;
			await UniTask.DelayFrame(1);
			
			m_imgBackground.gameObject.SetActive(false);
		}

		/// <summary>
		/// Set the secondary development config.
		/// </summary>
		public void Set2ndDevConfig(Color startFrom, Color startTo, Color endFrom, Color? endTo = null, float fadeInDuration = 0.5f, float fadeOutDuration = 0.5f)
		{
			beforeStartFromColor = startFrom;
			beforeStartToColor = startTo;
			afterEndFromColor = endFrom;
			afterEndToColor = endTo ?? startFrom;
			beforeFadeInDuration = fadeInDuration;
			afterFadeOutDuration = fadeOutDuration;
		}

		/// <summary>
		/// Stop the video playing.
		/// </summary>
		public void Stop()
		{
			if (!m_playerParent.isPrepared) return;
			if (m_playerParent.isPlaying)
			{
				m_playerParent.Stop();
			}
			m_isPlaying = false;
		}
		
		private void CloseInternal()
		{
			if (m_playerParent.isPrepared && m_playerParent.isPlaying)
			{
				m_playerParent.Stop();
			}
			m_isPlaying = false;
			m_playerParent.url = null;
			m_playerParent.seekCompleted -= delegate { }; // Clear all delegates
			m_isPlaying = false;
			urlVideoPath = null;
		}

		/// <summary>
		/// Close the video media.
		/// </summary>
		public void Close()
		{
			if (m_playerParent.isPlaying)
			{
				m_playerParent.Stop();
			}
			CloseInternal();
		}

		private void OnDestroy()
		{
			CloseInternal();
		}
	}
}