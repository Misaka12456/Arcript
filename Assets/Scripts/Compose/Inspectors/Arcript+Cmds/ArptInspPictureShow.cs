using Arcript.Aspt;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Arcript.Compose.Inspectors
{
	[CmdInspectExport(typeof(AsptPictureShowCmd), "show")]
	public class ArptInspPictureShow : InspectCmdPanelBase<AsptPictureShowCmd>
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
		public InputField inputLayer;
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

			#region layer
			inputLayer.SetTextWithoutNotify("default");
			inputLayer.onValueChanged.AddListener((value) =>
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					value = "default";
				}
				else if (value.Length > MaxLayerNameLength)
				{
					value = value.Substring(0, MaxLayerNameLength);
				}
				cmd.Layer = value;
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

			#region layer
			inputLayer.SetTextWithoutNotify(cmd.Layer);
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
			inputLayer.SetTextWithoutNotify("default");
			toggleScaleToWidth.SetIsOnWithoutNotify(false);
		}
	}
}