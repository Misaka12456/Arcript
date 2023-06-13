using Arcript.Aspt;
using Arcript.Compose.UI;
using Arcript.I18n;
using SFB;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

namespace Arcript.Compose.Inspectors
{
	[CmdInspectExport(typeof(AsptTextv2SettingsCmd), "say+set", "Arcript+ Text Show v2 Settings", "Resources/Prefabs/Editor/Inspectors/say+set")]
	public class ArptInspTextv2Settings : InspectCmdPanelBase<AsptTextv2SettingsCmd>
	{
		public InputField inputSpeakerBoxBgPath;
		public Button btnSelectSpeakerBoxBg;
		public InputField inputMessageBoxBgPath;
		public Button btnSelectMessageBoxBg;

		protected override void InspectorAwake()
		{
			inputSpeakerBoxBgPath.onValueChanged.AddListener((value) =>
			{
				cmd.SpeakerBoxImagePath = value;
				Apply();
			});
			inputMessageBoxBgPath.onValueChanged.AddListener((value) =>
			{
				cmd.MsgBoxImagePath = value;
				Apply();
			});
			btnSelectSpeakerBoxBg.onClick.AddListener(() => SelectPicture("compose.dialogs.fileSelect.img.title.speakerBoxBg", ref inputSpeakerBoxBgPath));
			btnSelectMessageBoxBg.onClick.AddListener(() => SelectPicture("compose.dialogs.fileSelect.img.title.msgBoxBg", ref inputMessageBoxBgPath));
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
			base.SetInfo(command, parentItem);

			#region speakerBoxBgPath
			inputSpeakerBoxBgPath.SetTextWithoutNotify(cmd.SpeakerBoxImagePath);
			#endregion

			#region messageBoxBgPath
			inputMessageBoxBgPath.SetTextWithoutNotify(cmd.MsgBoxImagePath);
			#endregion
		}

		public override void Apply(object tag = null)
		{
			parentItem.UpdateInfo(cmd);
		}

		public override void InitNewInfo()
		{
			inputSpeakerBoxBgPath.SetTextWithoutNotify(string.Empty);
			inputMessageBoxBgPath.SetTextWithoutNotify(string.Empty);
			btnSelectMessageBoxBg.interactable = btnSelectSpeakerBoxBg.interactable = true;
		}
	}
}