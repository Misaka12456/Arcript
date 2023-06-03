using System;
using System.Collections.Generic;
using System.Enhance.Unity;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace Arcript.I18n
{
	/// <summary>
	/// 提供国际化支持(多语言支持)的单例 <see cref="MonoBehaviour"/> 类。<br />
	/// <b>I = Internationalization</b>(or I18n)
	/// <para>
	/// 可自适应本地化文本id的文本框组件为 <see cref="IText" /> (依赖于 <see cref="UILabel"/> (NGUI的文本框组件))。
	/// </para>
	/// </summary>
	public class I : Singleton<I>
	{
		/// <summary>
		/// 游戏的默认语言(首次启动时使用)。<br />
		/// 默认语言跟随系统语言设置。
		/// </summary>
		public static string DefaultLocale
		{
			get
			{
				// 读取操作系统的语言，并返回为区域代码
				// 支持的语言（其它语言均返回英文区域代码(en))
				// 英文 -> en
				// 日文 -> ja
				// 韩文 -> ko
				// 中文(简体) -> zh-Hans
				// 中文(繁体) -> zh-Hant
				// 通过Application.systemLanguage获取
				return Application.systemLanguage switch
				{
					SystemLanguage.English => "en",
					SystemLanguage.Japanese => "ja",
					SystemLanguage.Korean => "ko",
					SystemLanguage.ChineseSimplified => "zh-Hans",
					SystemLanguage.ChineseTraditional => "zh-Hant",
					_ => "en",
				};
			}
		}

		/// <summary>
		/// 在任何情况下未找到字体时使用的默认字体。
		/// </summary>
		public static readonly string FallbackFont = "NotoSansCJKsc-Regular";

		/// <summary>
		/// 当前语言的区域代码。
		/// </summary>
		public string CurrentLocale
		{
			get => PlayerPrefs.GetString("locale", DefaultLocale);
			set => PlayerPrefs.SetString("locale", value);
		}

		/// <summary>
		/// 表示一个字体的类型。
		/// </summary>
		[Serializable]
		public enum FontType
		{
			/// <summary>
			/// 常规字体(Regular)
			/// </summary>
			Regular,

			/// <summary>
			/// 粗体(Bold)
			/// </summary>
			Bold,

			/// <summary>
			/// 细体(Light)
			/// </summary>
			Light,
		}
		[Serializable]
		public class IData
		{
			[Serializable]
			public class IValue
			{
				public string value;
				[JsonProperty("override_font")]
				public string overrideFont;
				[JsonIgnore]
				public Font overrideFontData;
			}

			[JsonProperty("override_fonts")]
			public Dictionary<string, string> OverrideFonts;
			[JsonIgnore]
			public Dictionary<string, Font> OverrideFontDatas;
			public Dictionary<string, IValue> Texts;
		}
		private readonly Dictionary<string, IData> stringsDict = new Dictionary<string, IData>();
		[HideInInspector]
		public List<string> stringsNotFound = new List<string>();

		/// <summary>
		/// 当前 <see cref="I"/> 类的单例实例(S = In<b>s</b>tance, 也可理解为<b>S</b>ingleton)。
		/// </summary>
		public static I S => Instance;

		private readonly IData.IValue loadingPlaceholder = new IData.IValue { value = "<LOADING>" };

		/// <summary>
		/// 获取当前语言下指定本地化文本id的文本。
		/// </summary>
		/// <param name="id">本地化文本id。</param>
		/// <returns>
		/// 成功返回对应的本地化文本，失败返回请求的本地化文本id(<b>并非<c>null</c></b>)。
		/// </returns>
		public IData.IValue this[string id]
		{
			get
			{
				IData.IValue value;
				if (!stringsDict.ContainsKey(CurrentLocale)) return loadingPlaceholder;
				bool isLastNull = false;
				if (stringsDict[CurrentLocale].Texts.ContainsKey(id))
				{
					value = stringsDict[CurrentLocale].Texts[id];
					if (value != null) return value;
					isLastNull = true;
				}
				if (stringsDict["en"].Texts.ContainsKey(id))
				{
					if (!isLastNull) LogI18nKeyNotFound(CurrentLocale, id);
					value = stringsDict["en"].Texts[id];
					if (value != null) return value;
					isLastNull = true;
				}
				else isLastNull = false;
				if (stringsDict["zh-Hans"].Texts.ContainsKey(id))
				{
					if (!isLastNull) LogI18nKeyNotFound("en", id);
					value = stringsDict["zh-Hans"].Texts[id];
					if (value != null) return value;
					isLastNull = true;
				}
				else isLastNull = false;
				if (!isLastNull) LogI18nKeyNotFound("zh-Hans", id);
				value = new IData.IValue
				{
					value = id
				};
				return value;
			}
		}

		/// <summary>
		/// 获取当前语言下指定类型的字体。<br />
		/// 该项返回的字体与语言存在相关性，如英文状态下<c>Regular</c>字体为<c>Kazesawa-Regular</c>，而中文状态下<c>Regular</c>字体为<c>NotoSansCJKsc-Regular</c>。
		/// </summary>
		/// <param name="typeEnum">字体类型(Regular/Bold/Light等)。</param>
		/// <returns>
		/// 成功返回对应的字体，失败返回 <see langword="null"/> 。
		/// </returns>
		public Font this[FontType typeEnum]
		{
			get
			{
				var type = typeEnum.ToString();
				if (!stringsDict.ContainsKey(CurrentLocale))
				{
					return null;
				}
				if (stringsDict[CurrentLocale].OverrideFontDatas.ContainsKey(type))
				{
					return stringsDict[CurrentLocale].OverrideFontDatas[type];
				}
				return null;
			}
		}

		private void LogI18nKeyNotFound(string loc, string id)
		{
			if (!stringsNotFound.Contains(id))
			{
				stringsNotFound.Add(id);
				Debug.LogWarningFormat("{0} I18n Key Not found: {1}", loc, id);
			}
		}

		private void LoadLocale(string locale)
		{
			byte[] rawData = Resources.Load<TextAsset>($"Languages/{locale}").bytes;
			try
			{
				stringsDict[locale] = JsonConvert.DeserializeObject<IData>(Encoding.UTF8.GetString(rawData));
				stringsDict[locale].OverrideFontDatas = new Dictionary<string, Font>();
				foreach (var font in stringsDict[locale].OverrideFonts)
				{
					stringsDict[locale].OverrideFontDatas[font.Key] = Resources.Load<Font>($"Fonts/{font.Value}");
				}
				stringsDict[locale].OverrideFonts = null;
				foreach (var value in stringsDict[locale].Texts)
				{
					if (value.Value == null)
					{
						stringsDict.Remove(value.Key);
						continue;
					}
					value.Value.overrideFontData = Resources.Load<Font>($"Fonts/{value.Value.overrideFont}");
					value.Value.overrideFont = null;
				}
			}
			catch (Exception ex)
			{
				stringsDict.Remove(locale);
				Debug.LogError($"本地化资源文件读取发生错误\nLocale file loading error\n\n{ex}");
			}
			GC.Collect();
		}

		public void OnLocaleChanges(string locale)
		{
			if (CurrentLocale != "en" && CurrentLocale != "zh-Hans") stringsDict[CurrentLocale] = null;
			CurrentLocale = locale;
			LoadLocale(locale);
		}

		protected override void SingletonAwake()
		{
			AllowRepeatInit = false;
			if (string.IsNullOrEmpty(CurrentLocale)) CurrentLocale = DefaultLocale;
			LoadLocale("zh-Hans");
			LoadLocale("en");
			if (CurrentLocale != "zh-Hans" && CurrentLocale != "en")
			{
				LoadLocale(CurrentLocale);
			}
		}
	}
}