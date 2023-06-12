using Arcript.Aspt;
using Arcript.I18n;
using UnityEngine;
using UnityEngine.UI;
using SFB; // Standalone File Browser (gkngkc/UnityStandaloneFileBrowser [-> GitHub])
using System.IO;
using System.Linq;

namespace Arcript.Compose.Inspectors
{
	[CmdInspectExport(typeof(AsptAudioPlayCmd), "play", "Arcript Audio Play")]
	public class ArptInspAudioPlay : InspectCmdPanelBase<AsptAudioPlayCmd>
	{
		[Header("Audio")]
		public InputField inputAudioPath;
		public Button btnSelectAudio;
		public InputField inputVolume;
		public Toggle toggleLoop;

		private float m_lastVolume = 0.85f;

		protected override void InspectorAwake()
		{
			inputAudioPath.onValueChanged.AddListener((value) =>
			{
				cmd.AudioPath = value;
				Apply();
			});
			btnSelectAudio.onClick.AddListener(SelectAudio);
			inputVolume.onValueChanged.AddListener((value) =>
			{
				if (!float.TryParse(value, out float r))
				{
					value = FallbackVolumeTransaction();
					r = float.Parse(value);
				}
				else
				{
					SaveVolumeTransaction(value);
				}
				cmd.Volume = r;
				Apply();
			});
			toggleLoop.onValueChanged.AddListener((value) =>
			{
				cmd.IsLoop = value;
				Apply();
			});
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
		
		private string FallbackVolumeTransaction()
		{
			inputVolume.SetTextWithoutNotify(m_lastVolume.ToString());
			return m_lastVolume.ToString();
		}

		private void SaveVolumeTransaction(string checkedValue)
		{
			m_lastVolume = float.Parse(checkedValue);
		}

		public override void SetInfo<C>(C command, ArptScriptCmdItem parentItem)
		{
			base.SetInfo(command, parentItem);
			
			#region audioPath
			inputAudioPath.SetTextWithoutNotify(cmd.AudioPath);
			#endregion
			
			#region volume
			inputVolume.SetTextWithoutNotify(cmd.Volume.ToString());
			m_lastVolume = cmd.Volume;
			#endregion

			#region isLoop
			toggleLoop.SetIsOnWithoutNotify(cmd.IsLoop);
			#endregion
		}

		public override void Apply(object tag = null)
		{
			parentItem.UpdateInfo(cmd);
		}

		public override void InitNewInfo()
		{
			inputAudioPath.SetTextWithoutNotify(string.Empty);
			inputVolume.SetTextWithoutNotify("0.85");
			m_lastVolume = 0.85f;
			
			if (ArptProjectManager.Instance.CurrentProject.EnableArcriptPlusFeatures)
			{
				// 如果启用了Arcript+特性，那么play基本就变成专门用于bgm的了，所以默认循环
				toggleLoop.SetIsOnWithoutNotify(true);
			}
			else
			{
				// 如果未启用Arcript+特性，因为任何音频都可以用play播放(包括音效和角色语音)，所以默认不循环
				toggleLoop.SetIsOnWithoutNotify(false);
			}
		}
	}
}