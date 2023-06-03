using UnityEngine;
using UnityEngine.UI;

namespace Arcript.I18n
{
	/// <summary>
	/// 可自适应本地化文本id的文本框组件(依赖于 <see cref="Text"/> (UGUI的文本框组件))。
	/// <para>
	/// 本地化文本id与值对照JSON文件位于 <b><c>Assets/Resources/jsons/tl/</c></b> 目录下。(<b>tl = Translation</b>)<br />
	/// JSON文件以<c>{区域代码}.json</c>命名，如 <c>zh-Hans.json</c> 为简体中文。
	/// </para>
	/// <para>
	/// I18n主管理类为 <see cref="I" />。
	/// </para>
	/// </summary>
	[RequireComponent(typeof(Text))]
	public class IText : MonoBehaviour
	{
		[SerializeField]
		private string _id;
		public string Id
		{
			get
			{
				return _id;
			}
			set
			{
#if UNITY_EDITOR
				if (!Application.isPlaying)
				{
					_id = value;
					return;
				}
#endif
				if (_id == value) return;
				_id = value;
				ApplyLocale();
			}
		}

		public bool fontOverride;
		public bool useSpecFont;
		[SerializeField]
		private I.FontType _fontType;
		public I.FontType FontType
		{
			get
			{
				return _fontType;
			}
			set
			{
#if !UNITY_EDITOR
				if (_fontType == value) return;
#else
				if (!Application.isPlaying)
				{
					_fontType = value;
					return;
				}
#endif
				_fontType = value;
				ApplyLocale();
			}
		}

		private Text _text;

		public Text Text => _text ??= GetComponent<Text>();

		public string[] replaceTexts;

		private void Awake()
		{
			_text = GetComponent<Text>();
		}

		private void Start()
		{
			ApplyLocale();
		}

		public void ApplyLocale()
		{
			var value = I.S?[Id];
			var str = value?.value;
			if (str != null && replaceTexts != null)
			{
				try
				{
					str = string.Format(str, replaceTexts);
				}
				catch
				{
					// ignored
				}
			}
			Text.text = str;
			if (!fontOverride) return;
			if (value?.overrideFontData)
			{
				Text.font = value.overrideFontData;
			}
			else
			{
				var font = I.S?[FontType];
				if (font != null)
				{
					Text.font = font;
				}
			}
		}

		public static string FastFormat(string id, params string[] replaceTexts)
		{
			var value = I.S?[id];
			var str = value?.value;
			if (str != null && replaceTexts != null)
			{
				try
				{
					str = string.Format(str, replaceTexts);
				}
				catch
				{
					// ignored
				}
			}
			return str;
		}
	}
}