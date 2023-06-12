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
	[CmdInspectExport(typeof(AsptPictureHideCmd), "hide", "Arcript Picture Hide")]
	public class ArptInspPictureHide : InspectCmdPanelBase<AsptPictureHideCmd>
	{
		[Header("Picture")]
		public InputField inputPicturePath;
		public Button btnSelectPicture;

		[Header("Transition")]
		public Dropdown drplstTransType;
		public Dropdown drplstCurveType;
		public InputField inputTransDuration;

		protected override void InspectorAwake()
		{
			#region imagePath
			inputPicturePath.onValueChanged.AddListener((value) =>
			{
				cmd.ImagePath = value;
				Apply();
			});
			btnSelectPicture.onClick.AddListener(SelectPicture);
			#endregion

			#region transition
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
		}

		public override void Apply(object tag = null)
		{
			parentItem.UpdateInfo(cmd);
		}

		public override void InitNewInfo()
		{
			inputPicturePath.SetTextWithoutNotify(string.Empty);
			drplstTransType.SetValueWithoutNotify(0);
			drplstCurveType.SetValueWithoutNotify(0);
			inputTransDuration.SetTextWithoutNotify("0");
		}
	}
}