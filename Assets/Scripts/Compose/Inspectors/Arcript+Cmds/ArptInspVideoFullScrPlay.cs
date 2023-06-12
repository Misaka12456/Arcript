using Arcript.Aspt;
using Arcript.I18n;
using SFB;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Arcript.Compose.Inspectors
{
	public enum ColorValueType
	{
		Red = 0, Green = 1, Blue = 2, Alpha = 3
	}

	[CmdInspectExport(typeof(AsptVideoFullScrPlayCmd), "videoFSPlay", "Arcript+ Video Full Screen Play")]
	public class ArptInspVideoFullScrPlay : InspectCmdPanelBase<AsptVideoFullScrPlayCmd>
	{
		public InputField inputVideoPath;
		public Button btnSelectVideo;

		public InputField inputStartFadeInFromR, inputStartFadeInFromG, inputStartFadeInFromB, inputStartFadeInFromA;
		public InputField inputStartFadeInToR, inputStartFadeInToG, inputStartFadeInToB, inputStartFadeInToA;
		public InputField inputStartFadeInDuration;

		public InputField inputEndFadeOutFromR, inputEndFadeOutFromG, inputEndFadeOutFromB, inputEndFadeOutFromA;
		public InputField inputEndFadeOutToR, inputEndFadeOutToG, inputEndFadeOutToB, inputEndFadeOutToA;
		public InputField inputEndFadeOutDuration;

		private Color m_lastStartFadeInFromColor, m_lastStartFadeInToColor;
		private Color m_lastEndFadeOutFromColor, m_lastEndFadeOutToColor;
		private float m_lastStartFadeInDuration, m_lastEndFadeOutDuration;

		protected override void InspectorAwake()
		{
			#region Fade Colors
			#region start fade-in (from)
			inputStartFadeInFromR.onValueChanged.AddListener((value) => OnStartFadeInFromColorEdited(ColorValueType.Red, value));
			inputStartFadeInFromG.onValueChanged.AddListener((value) => OnStartFadeInFromColorEdited(ColorValueType.Green, value));
			inputStartFadeInFromB.onValueChanged.AddListener((value) => OnStartFadeInFromColorEdited(ColorValueType.Blue, value));
			inputStartFadeInFromA.onValueChanged.AddListener((value) => OnStartFadeInFromColorEdited(ColorValueType.Alpha, value));
			#endregion

			#region start fade-in (to)
			inputStartFadeInToR.onValueChanged.AddListener((value) => OnStartFadeInToColorEdited(ColorValueType.Red, value));
			inputStartFadeInToG.onValueChanged.AddListener((value) => OnStartFadeInToColorEdited(ColorValueType.Green, value));
			inputStartFadeInToB.onValueChanged.AddListener((value) => OnStartFadeInToColorEdited(ColorValueType.Blue, value));
			inputStartFadeInToA.onValueChanged.AddListener((value) => OnStartFadeInToColorEdited(ColorValueType.Alpha, value));
			#endregion

			#region end fade-out (from)
			inputEndFadeOutFromR.onValueChanged.AddListener((value) => OnEndFadeOutFromColorEdited(ColorValueType.Red, value));
			inputEndFadeOutFromG.onValueChanged.AddListener((value) => OnEndFadeOutFromColorEdited(ColorValueType.Green, value));
			inputEndFadeOutFromB.onValueChanged.AddListener((value) => OnEndFadeOutFromColorEdited(ColorValueType.Blue, value));
			inputEndFadeOutFromA.onValueChanged.AddListener((value) => OnEndFadeOutFromColorEdited(ColorValueType.Alpha, value));
			#endregion

			#region end fade-out (to)
			inputEndFadeOutToR.onValueChanged.AddListener((value) => OnEndFadeOutToColorEdited(ColorValueType.Red, value));
			inputEndFadeOutToG.onValueChanged.AddListener((value) => OnEndFadeOutToColorEdited(ColorValueType.Green, value));
			inputEndFadeOutToB.onValueChanged.AddListener((value) => OnEndFadeOutToColorEdited(ColorValueType.Blue, value));
			inputEndFadeOutToA.onValueChanged.AddListener((value) => OnEndFadeOutToColorEdited(ColorValueType.Alpha, value));
			#endregion
			#endregion
			inputStartFadeInDuration.onValueChanged.AddListener((value) =>
			{
				if (!float.TryParse(value, out float r))
				{
					value = FallbackTransaction(inputStartFadeInDuration, m_lastStartFadeInDuration);
					r = float.Parse(value);
				}
				else
				{
					SaveTransaction(ref m_lastStartFadeInDuration, value);
				}
				cmd.StartFadeInDuration = r;
				Apply();
			});
			inputEndFadeOutDuration.onValueChanged.AddListener((value) =>
			{
				if (!float.TryParse(value, out float r))
				{
					value = FallbackTransaction(inputEndFadeOutDuration, m_lastEndFadeOutDuration);
					r = float.Parse(value);
				}
				else
				{
					SaveTransaction(ref m_lastEndFadeOutDuration, value);
				}
				cmd.StartFadeInDuration = r;
				Apply();
			});
			btnSelectVideo.onClick.AddListener(SelectVideo);
		}

		private void SelectVideo()
		{
			var filters = new List<ExtensionFilter>();
			var defFilter = new ExtensionFilter()
			{
				Name = I.S["compose.dialogs.fileSelect.video.filter"].value,
				Extensions = new string[] { "mp4", "wmv", "avi", "mkv", "mpg", "mpeg" } // ONLY support these formats (common video formats)
			};
			filters.Add(defFilter);

			string dialogResult = StandaloneFileBrowser.OpenFilePanel(title: I.S["compose.dialogs.fileSelect.video.title"].value,
				directory: string.Empty,
				extensions: filters.ToArray(),
				multiselect: false);
			if (string.IsNullOrWhiteSpace(dialogResult) || !File.Exists(dialogResult))
			{
				return;
			}

			dialogResult = dialogResult.TrimStart('"').TrimEnd('"'); // 去除双引号
			dialogResult = dialogResult.Replace(Path.PathSeparator, '/'); // 替换路径分隔符为正斜杠(无视平台)(这将同时替换Windows下的分区分隔符(":\\" => ":/"))

			inputVideoPath.text = dialogResult; // 这里不使用SetTextWithoutNotify以使其像用户编辑一样触发事件
		}

		#region color inputField edited bindings
		private void OnStartFadeInFromColorEdited(ColorValueType colorType, string valueData)
		{
			if (float.TryParse(valueData, out float r) && r >= 0 && r <= 1)
			{
				SaveStartFadeInFromColorTransaction(colorType, valueData);
			}
			else
			{
				valueData = FallbackStartFadeInFromColorTransaction(colorType);
			}
			var color = cmd.StartFadeInFrom;
			switch (colorType)
			{
				case ColorValueType.Red:
					color.r = float.Parse(valueData);
					break;
				case ColorValueType.Green:
					color.g = float.Parse(valueData);
					break;
				case ColorValueType.Blue:
					color.b = float.Parse(valueData);
					break;
				case ColorValueType.Alpha:
					color.a = float.Parse(valueData);
					break;
			}
			cmd.StartFadeInFrom = color;
			Apply();
		}

		private void OnStartFadeInToColorEdited(ColorValueType colorType, string valueData)
		{
			if (float.TryParse(valueData, out float r) && r >= 0 && r <= 1)
			{
				SaveStartFadeInToColorTransaction(colorType, valueData);
			}
			else
			{
				valueData = FallbackStartFadeInToColorTransaction(colorType);
			}
			var color = cmd.StartFadeInTo;
			switch (colorType)
			{
				case ColorValueType.Red:
					color.r = float.Parse(valueData);
					break;
				case ColorValueType.Green:
					color.g = float.Parse(valueData);
					break;
				case ColorValueType.Blue:
					color.b = float.Parse(valueData);
					break;
				case ColorValueType.Alpha:
					color.a = float.Parse(valueData);
					break;
			}
			cmd.StartFadeInTo = color;
			Apply();
		}

		private void OnEndFadeOutFromColorEdited(ColorValueType colorType, string valueData)
		{
			if (float.TryParse(valueData, out float r) && r >= 0 && r <= 1)
			{
				SaveEndFadeOutFromColorTransaction(colorType, valueData);
			}
			else
			{
				valueData = FallbackEndFadeOutFromColorTransaction(colorType);
			}
			var color = cmd.EndFadeOutFrom;
			switch (colorType)
			{
				case ColorValueType.Red:
					color.r = float.Parse(valueData);
					break;
				case ColorValueType.Green:
					color.g = float.Parse(valueData);
					break;
				case ColorValueType.Blue:
					color.b = float.Parse(valueData);
					break;
				case ColorValueType.Alpha:
					color.a = float.Parse(valueData);
					break;
			}
			cmd.EndFadeOutFrom = color;
			Apply();
		}

		private void OnEndFadeOutToColorEdited(ColorValueType colorType, string valueData)
		{
			if (float.TryParse(valueData, out float r) && r >= 0 && r <= 1)
			{
				SaveEndFadeOutToColorTransaction(colorType, valueData);
			}
			else
			{
				valueData = FallbackEndFadeOutToColorTransaction(colorType);
			}
			var color = cmd.EndFadeOutTo;
			switch (colorType)
			{
				case ColorValueType.Red:
					color.r = float.Parse(valueData);
					break;
				case ColorValueType.Green:
					color.g = float.Parse(valueData);
					break;
				case ColorValueType.Blue:
					color.b = float.Parse(valueData);
					break;
				case ColorValueType.Alpha:
					color.a = float.Parse(valueData);
					break;
			}
			cmd.EndFadeOutTo = color;
			Apply();
		}
		#endregion

		private string FallbackTransaction(InputField targetInput, float backup)
		{
			targetInput.SetTextWithoutNotify(backup.ToString());
			return backup.ToString();
		}

		private void SaveTransaction(ref float backup, string valueData)
		{
			backup = float.Parse(valueData);
		}

		#region color undo-support transactions
		private string FallbackStartFadeInFromColorTransaction(ColorValueType colorType)
		{
			var color = m_lastStartFadeInFromColor;
			switch (colorType)
			{
				case ColorValueType.Red:
					inputStartFadeInFromR.SetTextWithoutNotify(color.r.ToString());
					return color.r.ToString();
				case ColorValueType.Green:
					inputStartFadeInFromG.SetTextWithoutNotify(color.g.ToString());
					return color.g.ToString();
				case ColorValueType.Blue:
					inputStartFadeInFromB.SetTextWithoutNotify(color.b.ToString());
					return color.b.ToString();
				case ColorValueType.Alpha:
					inputStartFadeInFromA.SetTextWithoutNotify(color.a.ToString());
					return color.a.ToString();
				default:
					throw new ArgumentOutOfRangeException(nameof(colorType), colorType, null);
			}
		}

		private void SaveStartFadeInFromColorTransaction(ColorValueType colorType, string valueData)
		{
			var color = m_lastStartFadeInFromColor;
			switch (colorType)
			{
				case ColorValueType.Red:
					color.r = float.Parse(valueData);
					break;
				case ColorValueType.Green:
					color.g = float.Parse(valueData);
					break;
				case ColorValueType.Blue:
					color.b = float.Parse(valueData);
					break;
				case ColorValueType.Alpha:
					color.a = float.Parse(valueData);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(colorType), colorType, null);
			}
			m_lastStartFadeInFromColor = color;
		}

		private string FallbackStartFadeInToColorTransaction(ColorValueType colorType)
		{
			var color = m_lastStartFadeInToColor;
			switch (colorType)
			{
				case ColorValueType.Red:
					inputStartFadeInToR.SetTextWithoutNotify(color.r.ToString());
					return color.r.ToString();
				case ColorValueType.Green:
					inputStartFadeInToG.SetTextWithoutNotify(color.g.ToString());
					return color.g.ToString();
				case ColorValueType.Blue:
					inputStartFadeInToB.SetTextWithoutNotify(color.b.ToString());
					return color.b.ToString();
				case ColorValueType.Alpha:
					inputStartFadeInToA.SetTextWithoutNotify(color.a.ToString());
					return color.a.ToString();
				default:
					throw new ArgumentOutOfRangeException(nameof(colorType), colorType, null);
			}
		}

		private void SaveStartFadeInToColorTransaction(ColorValueType colorType, string valueData)
		{
			var color = m_lastStartFadeInToColor;
			switch (colorType)
			{
				case ColorValueType.Red:
					color.r = float.Parse(valueData);
					break;
				case ColorValueType.Green:
					color.g = float.Parse(valueData);
					break;
				case ColorValueType.Blue:
					color.b = float.Parse(valueData);
					break;
				case ColorValueType.Alpha:
					color.a = float.Parse(valueData);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(colorType), colorType, null);
			}
			m_lastStartFadeInToColor = color;
		}

		private string FallbackEndFadeOutFromColorTransaction(ColorValueType colorType)
		{
			var color = m_lastEndFadeOutFromColor;
			switch (colorType)
			{
				case ColorValueType.Red:
					inputEndFadeOutFromR.SetTextWithoutNotify(color.r.ToString());
					return color.r.ToString();
				case ColorValueType.Green:
					inputEndFadeOutFromG.SetTextWithoutNotify(color.g.ToString());
					return color.g.ToString();
				case ColorValueType.Blue:
					inputEndFadeOutFromB.SetTextWithoutNotify(color.b.ToString());
					return color.b.ToString();
				case ColorValueType.Alpha:
					inputEndFadeOutFromA.SetTextWithoutNotify(color.a.ToString());
					return color.a.ToString();
				default:
					throw new ArgumentOutOfRangeException(nameof(colorType), colorType, null);
			}
		}

		private void SaveEndFadeOutFromColorTransaction(ColorValueType colorType, string valueData)
		{
			var color = m_lastEndFadeOutFromColor;
			switch (colorType)
			{
				case ColorValueType.Red:
					color.r = float.Parse(valueData);
					break;
				case ColorValueType.Green:
					color.g = float.Parse(valueData);
					break;
				case ColorValueType.Blue:
					color.b = float.Parse(valueData);
					break;
				case ColorValueType.Alpha:
					color.a = float.Parse(valueData);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(colorType), colorType, null);
			}
			m_lastEndFadeOutFromColor = color;
		}

		private string FallbackEndFadeOutToColorTransaction(ColorValueType colorType)
		{
			var color = m_lastEndFadeOutToColor;
			switch (colorType)
			{
				case ColorValueType.Red:
					inputEndFadeOutToR.SetTextWithoutNotify(color.r.ToString());
					return color.r.ToString();
				case ColorValueType.Green:
					inputEndFadeOutToG.SetTextWithoutNotify(color.g.ToString());
					return color.g.ToString();
				case ColorValueType.Blue:
					inputEndFadeOutToB.SetTextWithoutNotify(color.b.ToString());
					return color.b.ToString();
				case ColorValueType.Alpha:
					inputEndFadeOutToA.SetTextWithoutNotify(color.a.ToString());
					return color.a.ToString();
				default:
					throw new ArgumentOutOfRangeException(nameof(colorType), colorType, null);
			}
		}

		private void SaveEndFadeOutToColorTransaction(ColorValueType colorType, string valueData)
		{
			var color = m_lastEndFadeOutToColor;
			switch (colorType)
			{
				case ColorValueType.Red:
					color.r = float.Parse(valueData);
					break;
				case ColorValueType.Green:
					color.g = float.Parse(valueData);
					break;
				case ColorValueType.Blue:
					color.b = float.Parse(valueData);
					break;
				case ColorValueType.Alpha:
					color.a = float.Parse(valueData);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(colorType), colorType, null);
			}
			m_lastEndFadeOutToColor = color;
		}
		#endregion
		public override void Apply(object tag = null)
		{
			parentItem.UpdateInfo(cmd); // cmd is base.cmd (not this.cmd), so that's why it didn't appear in declaration of this inherited class
										// another same base variable: base.parentItem
		}

		public override void InitNewInfo()
		{
			inputVideoPath.SetTextWithoutNotify(string.Empty);

			#region startFadeInFrom(0, 0, 0, 1)
			inputStartFadeInFromR.SetTextWithoutNotify("0");
			inputStartFadeInFromG.SetTextWithoutNotify("0");
			inputStartFadeInFromB.SetTextWithoutNotify("0");
			inputStartFadeInFromA.SetTextWithoutNotify("1");
			#endregion

			#region startFadeInTo(1, 1, 1, 1)
			inputStartFadeInToR.SetTextWithoutNotify("1");
			inputStartFadeInToG.SetTextWithoutNotify("1");
			inputStartFadeInToB.SetTextWithoutNotify("1");
			inputStartFadeInToA.SetTextWithoutNotify("1");
			#endregion

			#region endFadeOutFrom(1, 1, 1, 1)
			inputEndFadeOutFromR.SetTextWithoutNotify("1");
			inputEndFadeOutFromG.SetTextWithoutNotify("1");
			inputEndFadeOutFromB.SetTextWithoutNotify("1");
			inputEndFadeOutFromA.SetTextWithoutNotify("1");
			#endregion

			#region endFadeOutTo(0, 0, 0, 1)
			inputEndFadeOutToR.SetTextWithoutNotify("0");
			inputEndFadeOutToG.SetTextWithoutNotify("0");
			inputEndFadeOutToB.SetTextWithoutNotify("0");
			inputEndFadeOutToA.SetTextWithoutNotify("1");
			#endregion

			inputStartFadeInDuration.SetTextWithoutNotify("2.5");
			inputEndFadeOutDuration.SetTextWithoutNotify("2.5");
			
			m_lastStartFadeInFromColor = m_lastEndFadeOutToColor = Color.black;
			m_lastStartFadeInToColor = m_lastEndFadeOutFromColor = Color.white;
			m_lastStartFadeInDuration = m_lastEndFadeOutDuration = 2.5f;
		}
	}
}