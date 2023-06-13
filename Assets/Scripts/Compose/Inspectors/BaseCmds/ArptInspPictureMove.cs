using Arcript.Aspt;
using Arcript.I18n;
using UnityEngine;
using UnityEngine.UI;
using SFB; // Standalone File Browser (gkngkc/UnityStandaloneFileBrowser [-> GitHub])
using System.IO;
using System.Collections.Generic;
using System;
using Arcript.Compose.UI;

namespace Arcript.Compose.Inspectors
{
	[CmdInspectExport(typeof(AsptPictureMoveCmd), "move", "Arcript Picture Move", "Resources/Prefabs/Editor/Inspectors/move")]
	public class ArptInspPictureMove : InspectCmdPanelBase<AsptPictureMoveCmd>
	{
		[Header("Picture")]
		public InputField inputPicturePath;
		public Button btnSelectPicture;
		public InputField inputMoveDeltaX, inputMoveDeltaY;

		[Header("Transition")]
		public Dropdown dropListCurveType;
		public InputField inputDuration;

		protected override void InspectorAwake()
		{
			#region picturePath (+selectBtn)
			inputPicturePath.onValueChanged.AddListener((value) =>
			{
				cmd.ImagePath = value;
				Apply();
			});
			btnSelectPicture.onClick.AddListener(SelectPicture);
			#endregion

			#region moveDeltaX
			inputMoveDeltaX.onValueChanged.AddListener((value) =>
			{
				if (!float.TryParse(value, out float r))
				{
					value = "0";
					inputMoveDeltaX.SetTextWithoutNotify("0");
					r = float.Parse(value);
				}
				cmd.MoveDelta = new Vector2(r, cmd.MoveDelta.y);
				Apply();
			});
			#endregion

			#region moveDeltaY
			inputMoveDeltaY.onValueChanged.AddListener((value) =>
			{
				if (!float.TryParse(value, out float r))
				{
					value = "0";
					inputMoveDeltaY.SetTextWithoutNotify("0");
					r = float.Parse(value);
				}
				cmd.MoveDelta = new Vector2(cmd.MoveDelta.x, r);
				Apply();
			});
			#endregion

			#region curveType
			dropListCurveType.ClearOptions();

			var curveOptions = new List<Dropdown.OptionData>();
			foreach (var item in Enum.GetValues(typeof(CurveType)))
			{
				curveOptions.Add(new Dropdown.OptionData(item.ToString()));
			}
			dropListCurveType.AddOptions(curveOptions);

			dropListCurveType.onValueChanged.AddListener((idx) =>
			{
				string str = dropListCurveType.options[idx].text;
				var type = (CurveType)Enum.Parse(typeof(CurveType), str);
				cmd.Curve = type;
				Apply();
			});
			#endregion

			#region duration
			inputDuration.onValueChanged.AddListener((value) =>
			{
				if (!float.TryParse(value, out float r) || r < 0)
				{
					value = "0";
					inputDuration.SetTextWithoutNotify("0");
					r = float.Parse(value);
				}
				cmd.Duration = r;
				Apply();
			});
			#endregion
		}

		private void SelectPicture()
		{
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

			string dialogResult = StandaloneFileBrowser.OpenFilePanel(title: I.S["compose.dialogs.fileSelect.img.title"].value,
				directory: string.Empty,
				extensions: filters.ToArray(),
				multiselect: false);
			if (string.IsNullOrWhiteSpace(dialogResult) || !File.Exists(dialogResult))
			{
				return;
			}

			dialogResult = dialogResult.TrimStart('"').TrimEnd('"'); // 去除双引号
			dialogResult = dialogResult.Replace(Path.PathSeparator, '/'); // 替换路径分隔符为正斜杠(无视平台)(这将同时替换Windows下的分区分隔符(":\\" => ":/"))

			inputPicturePath.text = dialogResult; // 这里不使用SetTextWithoutNotify以使其像用户编辑一样触发事件
		}

		public override void SetInfo<C>(C command, ArptScriptCmdItem parentItem)
		{
			base.SetInfo(command, parentItem);

			#region imagePath
			inputPicturePath.SetTextWithoutNotify(cmd.ImagePath);
			#endregion

			#region moveDelta
			inputMoveDeltaX.SetTextWithoutNotify(cmd.MoveDelta.x.ToString());
			inputMoveDeltaY.SetTextWithoutNotify(cmd.MoveDelta.y.ToString());
			#endregion

			#region curveType
			int idx = dropListCurveType.options.FindIndex((item) => item.text == cmd.Curve.ToString());
			dropListCurveType.SetValueWithoutNotify(idx);
			#endregion

			#region duration
			inputDuration.SetTextWithoutNotify(cmd.Duration.ToString());
			#endregion

		}

		public override void Apply(object tag = null)
		{
			parentItem.UpdateInfo(cmd);
		}

		public override void InitNewInfo()
		{
			inputPicturePath.SetTextWithoutNotify(string.Empty);
			inputMoveDeltaX.SetTextWithoutNotify("0");
			inputMoveDeltaY.SetTextWithoutNotify("0");
			dropListCurveType.SetValueWithoutNotify(0);
			inputDuration.SetTextWithoutNotify("0");
		}
	}
}