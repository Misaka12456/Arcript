using Arcript.Aspt;
using Arcript.Compose.UI;
using Arcript.I18n;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SFB; // Standalone File Browser (gkngkc/UnityStandaloneFileBrowser [-> GitHub])
using System.IO;

namespace Arcript.Compose.Inspectors
{
	[CmdInspectExport(typeof(AsptPictureShowCmd), "show", "Arcript Picture Show (Legacy)", "Resources/Prefabs/Editor/Inspectors/show")]
	public class ArptInspPictureShowLegacy : InspectCmdPanelBase<AsptPictureShowLegacyCmd>
	{
		public const int MaxLayerNameLength = 32;

		[Header("Picture")]
		public InputField inputPicturePath;
		public Button btnSelectPicture;
		public InputField inputSizeWidth, inputSizeHeight;
		public InputField inputStartPointX, inputStartPointY;
		public InputField inputScaleX, inputScaleY;

		[Header("Transition")]
		public Dropdown drplstTransType;
		public Dropdown drplstCurveType;
		public InputField inputTransDuration;

		[Header("Settings")]
		public Toggle toggleScaleToWidth;

		protected override void InspectorAwake()
		{
			#region imagePath
			inputPicturePath.onValueChanged.AddListener((value) =>
			{
				cmd.ImagePath = value;
				Apply();
			});
			#endregion

			#region size
			inputSizeWidth.onValueChanged.AddListener((value) =>
			{
				if (float.TryParse(value, out float result))
				{
					OnSizeChanged(false, result);
				}
				else
				{
					inputSizeWidth.SetTextWithoutNotify("0");
					OnSizeChanged(false, 0);
				}
			});
			inputSizeHeight.onValueChanged.AddListener((value) =>
			{
				if (float.TryParse(value, out float result))
				{
					OnSizeChanged(true, result);
				}
				else
				{
					inputSizeHeight.SetTextWithoutNotify("0");
					OnSizeChanged(true, 0);
				}
			});
			#endregion

			#region startPoint
			inputStartPointX.onValueChanged.AddListener((value) =>
			{
				if (float.TryParse(value, out float result))
				{
					OnStartPointChanged(false, result);
				}
				else
				{
					inputStartPointX.SetTextWithoutNotify("0");
					OnStartPointChanged(false, 0);
				}
			});
			inputStartPointY.onValueChanged.AddListener((value) =>
			{
				if (float.TryParse(value, out float result))
				{
					OnStartPointChanged(true, result);
				}
				else
				{
					inputStartPointY.SetTextWithoutNotify("0");
					OnStartPointChanged(true, 0);
				}
			});
			#endregion

			#region scale
			inputScaleX.onValueChanged.AddListener((value) =>
			{
				if (float.TryParse(value, out float result))
				{
					OnScaleChanged(false, result);
				}
				else
				{
					inputScaleX.SetTextWithoutNotify("1");
					OnScaleChanged(false, 1);
				}
			});
			inputScaleY.onValueChanged.AddListener((value) =>
			{
				if (float.TryParse(value, out float result))
				{
					OnScaleChanged(true, result);
				}
				else
				{
					inputScaleY.SetTextWithoutNotify("1");
					OnScaleChanged(true, 1);
				}
			});
			#endregion

			#region transition(type + curveType + duration)
			var transOptions = new List<Dropdown.OptionData>();
			// 循环遍历TransitionType枚举
			foreach (var item in Enum.GetValues(typeof(TransitionType)))
			{
				transOptions.Add(new Dropdown.OptionData(item.ToString()));
			}

			var curveOptions = new List<Dropdown.OptionData>();
			// 循环遍历CurveType枚举
			foreach (var item in Enum.GetValues(typeof(CurveType)))
			{
				curveOptions.Add(new Dropdown.OptionData(item.ToString()));
			}

			drplstTransType.AddOptions(transOptions);
			drplstCurveType.AddOptions(curveOptions);
			
			drplstTransType.onValueChanged.AddListener((idx) =>
			{
				string str = drplstTransType.options[idx].text;
				var type = (TransitionType)Enum.Parse(typeof(TransitionType), str);
				cmd.Transition.Type = type;
				Apply();
			});

			drplstCurveType.onValueChanged.AddListener((idx) =>
			{
				string str = drplstTransType.options[idx].text;
				var type = (CurveType)Enum.Parse(typeof(CurveType), str);
				cmd.Transition.Curve = type;
				Apply();
			});
			
			inputTransDuration.SetTextWithoutNotify("0.5");
			inputTransDuration.onValueChanged.AddListener((value) =>
			{
				if (float.TryParse(value, out float result))
				{
					cmd.Transition.Duration = result;
				}
				else
				{
					inputTransDuration.SetTextWithoutNotify("0");
					cmd.Transition.Duration = 0;
				}
				Apply();
			});
			#endregion

			#region scaleToWidth
			toggleScaleToWidth.onValueChanged.AddListener((value) =>
			{
				cmd.ScaleToWidth = value;
				Apply();
			});
			#endregion

			btnSelectPicture.onClick.AddListener(() => SelectPicture(null, ref inputPicturePath));
		}

		private void OnSizeChanged(bool isHeight, float value)
		{
			if (value < 0) value = 0;
			if (!isHeight)
			{
				cmd.Size = new Vector2(value, cmd.Size.y);
			}
			else
			{
				cmd.Size = new Vector2(cmd.Size.x, value);
			}
			Apply();
		}

		private void OnStartPointChanged(bool isY, float value)
		{
			if (!isY)
			{
				cmd.StartPoint = new Vector2(value, cmd.StartPoint.y);
			}
			else
			{
				cmd.StartPoint = new Vector2(cmd.StartPoint.x, value);
			}
			Apply();
		}

		private void OnScaleChanged(bool isY, float value)
		{
			if (value < 0) value = 1;
			if (!isY)
			{
				cmd.Scale = new Vector2(value, cmd.Scale.y);
			}
			else
			{
				cmd.Scale = new Vector2(cmd.Scale.x, value);
			}
			Apply();
		}

		private void SelectPicture(string titleI18nId, ref InputField target)
		{
			if (string.IsNullOrWhiteSpace(titleI18nId))
			{
				titleI18nId = "compose.dialogs.fileSelect.img.title"; // fallback到默认值
			}
			var filters = new List<ExtensionFilter>();
			var defFilter = new ExtensionFilter()
			{
				Name = I.S["compose.dialogs.fileSelect.img.filter"].value,
				Extensions = FileItem.ImageFileExt
			};
			filters.Add(defFilter);

			// 自定义的第三方图片格式(由包含继承ICustomImageFormat的格式类的Plugin程序集(Assembly)提供)
			foreach (var imgExtPlugin in FileItem.CustomImageFileExt)
			{
				var filter = new ExtensionFilter()
				{
					Name = imgExtPlugin.Value.FormatName,
					Extensions = new[] { imgExtPlugin.Value.FormatExtension.TrimStart('.') }
				};
				filters.Add(filter);
			}

			string dialogResult = StandaloneFileBrowser.OpenFilePanel(title: I.S[titleI18nId].value,
				directory: string.Empty,
				extensions: filters.ToArray(),
				multiselect: false);
			if (string.IsNullOrWhiteSpace(dialogResult) || !File.Exists(dialogResult))
			{
				return;
			}

			dialogResult = dialogResult.TrimStart('"').TrimEnd('"'); // 去除双引号
			dialogResult = dialogResult.Replace(Path.PathSeparator, '/'); // 替换路径分隔符为正斜杠(无视平台)(这将同时替换Windows下的分区分隔符(":\\" => ":/"))

			target.text = dialogResult; // 这里不使用SetTextWithoutNotify以使其像用户编辑一样触发事件
		}

		public override void SetInfo<C>(C command, ArptScriptCmdItem parentItem)
		{
			base.SetInfo(command, parentItem); // check type validation and: parentItem -> parent; command as C -> cmd

			#region imagePath
			inputPicturePath.SetTextWithoutNotify(cmd.ImagePath);
			#endregion

			#region size
			inputSizeWidth.SetTextWithoutNotify(cmd.Size.x.ToString());
			inputSizeHeight.SetTextWithoutNotify(cmd.Size.y.ToString());
			#endregion

			#region startPoint
			inputStartPointX.SetTextWithoutNotify(cmd.StartPoint.x.ToString());
			inputStartPointY.SetTextWithoutNotify(cmd.StartPoint.y.ToString());
			#endregion

			#region scale
			inputScaleX.SetTextWithoutNotify(cmd.Scale.x.ToString());
			inputScaleY.SetTextWithoutNotify(cmd.Scale.y.ToString());
			#endregion

			#region transition
			#region transition - type
			string transTypeStr = Enum.GetName(typeof(TransitionType), cmd.Transition.Type);
			int transTypeIdx = drplstTransType.options.FindIndex((item) => item.text == transTypeStr);
			drplstTransType.SetValueWithoutNotify(transTypeIdx);
			#endregion

			#region transition - curveType
			string curveTypeStr = Enum.GetName(typeof(CurveType), cmd.Transition.Curve);
			int curveTypeIdx = drplstCurveType.options.FindIndex((item) => item.text == curveTypeStr);
			drplstCurveType.SetValueWithoutNotify(curveTypeIdx);
			#endregion

			#region transition - duration
			inputTransDuration.SetTextWithoutNotify(cmd.Transition.Duration.ToString());
			#endregion
			#endregion
			
			#region scaleToWidth
			toggleScaleToWidth.SetIsOnWithoutNotify(cmd.ScaleToWidth);
			#endregion
		}

		public override void Apply(object tag = null)
		{
			parentItem.UpdateInfo(cmd);
		}

		public override void InitNewInfo()
		{
			inputPicturePath.SetTextWithoutNotify(string.Empty);
			btnSelectPicture.interactable = true;
			inputSizeWidth.SetTextWithoutNotify("100");
			inputSizeHeight.SetTextWithoutNotify("100");
			inputStartPointX.SetTextWithoutNotify("0");
			inputStartPointY.SetTextWithoutNotify("0");
			inputScaleX.SetTextWithoutNotify("1");
			inputScaleY.SetTextWithoutNotify("1");
			drplstTransType.SetValueWithoutNotify(0);
			drplstCurveType.SetValueWithoutNotify(0);
			inputTransDuration.SetTextWithoutNotify("0.5");
			toggleScaleToWidth.SetIsOnWithoutNotify(false);
		}
	}
}