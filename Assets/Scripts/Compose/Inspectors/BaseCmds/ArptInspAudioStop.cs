using Arcript.Aspt;
using Arcript.I18n;
using UnityEngine;
using UnityEngine.UI;
using SFB; // Standalone File Browser (gkngkc/UnityStandaloneFileBrowser [-> GitHub])
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Arcript.Compose.Inspectors
{
	[CmdInspectExport(typeof(AsptAudioStopCmd), "stop", "Arcript Audio Stop", "Resources/Prefabs/Editor/Inspectors/stop")]
	public class ArptInspAudioStop : InspectCmdPanelBase<AsptAudioStopCmd>
	{
		[Header("Audio")]
		public InputField inputAudioPath;
		public Button btnSelectAudio;

		[Header("Fade Transition")]
		public Dropdown dropListCurveType;
		public InputField inputDuration;

		protected override void InspectorAwake()
		{
			#region audioPath (+selectBtn)
			inputAudioPath.onValueChanged.AddListener((value) =>
			{
				cmd.AudioPath = value;
				Apply();
			});
			btnSelectAudio.onClick.AddListener(SelectAudio);
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

		private void SelectAudio()
		{
			bool enableArptPlus = ArptProjectManager.Instance.CurrentProject.EnableArcriptPlusFeatures;

			var defFilter = new ExtensionFilter()
			{
				Name = I.S["compose.dialogs.fileSelect.audio.filter"].value,
				Extensions = new[] { "ogg", "wav" }
			};

			if (enableArptPlus)
			{
				defFilter.Extensions = defFilter.Extensions.Append("mp3").Append("flac").ToArray();
			}

			string titleI18nId = enableArptPlus ? "compose.dialogs.fileSelect.audio.title" : "compose.dialogs.fileSelect.audio.title.limited";

			string dialogResult = StandaloneFileBrowser.OpenFilePanel(title: I.S[titleI18nId].value,
				directory: string.Empty,
				extensions: new[] { defFilter },
				multiselect: false);
			if (string.IsNullOrWhiteSpace(dialogResult) || !File.Exists(dialogResult))
			{
				return;
			}

			dialogResult = dialogResult.TrimStart('"').TrimEnd('"'); // 去除双引号
			dialogResult = dialogResult.Replace(Path.PathSeparator, '/'); // 替换路径分隔符为正斜杠(无视平台)(这将同时替换Windows下的分区分隔符(":\\" => ":/"))

			inputAudioPath.text = dialogResult; // 这里不使用SetTextWithoutNotify以使其像用户编辑一样触发事件
		}

		public override void SetInfo<C>(C command, ArptScriptCmdItem parentItem)
		{
			base.SetInfo(command, parentItem);

			#region audioPath
			inputAudioPath.SetTextWithoutNotify(cmd.AudioPath);
			#endregion

			#region curveType
			int idx = dropListCurveType.options.FindIndex((option) => option.text == cmd.Curve.ToString());
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
			inputAudioPath.SetTextWithoutNotify(string.Empty);
			dropListCurveType.SetValueWithoutNotify(0);
			inputDuration.SetTextWithoutNotify("0");
		}
	}
}