using Arcript.Aspt;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Arcript.Compose.Inspectors
{
	[CmdInspectExport(typeof(AsptHSceneStartAnchorCmd), "anchorHSStart", "Arcript+ Script Anchor (HScene Start)")]
	public class ArptInspHSceneStartAnchor : InspectCmdPanelBase<AsptHSceneStartAnchorCmd>
	{
		[Header("General")]
		public InputField inputAnchorName;
		public InputField inputAnchorTags;
		public InputField inputHSceneName;

		private string m_lastAnchorName = Guid.NewGuid().ToString("N");
		private List<string> m_lastAnchorTags = new List<string>();
		private string m_lastHSceneName = $"HScene_Kano_06"; // example (will **NEVER** be shown in Arcript UI)
															 // 其实原游戏的Kano角色只有5个HScene(x

		protected override void InspectorAwake()
		{
			inputAnchorName.onValueChanged.AddListener((value) =>
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					value = FallbackAnchorNameTransaction();
				}
				else
				{
					SaveAnchorNameTransaction(value);
				}
				cmd.AnchorName = value;
				Apply();
			});

			inputAnchorTags.onValueChanged.AddListener((value) =>
			{
				List<string> list;
				try
				{
					var l = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
					SaveAnchorTagsTransaction(l);
					list = l;
				}
				catch
				{
					list = FallbackAnchorTagsTransaction();
				}
				cmd.AnchorTags = list.ToArray();
				Apply();
			});

			inputHSceneName.onValueChanged.AddListener((value) =>
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					value = FallbackHSceneNameTransaction();
				}
				else
				{
					SaveHSceneNameTransaction(value);
				}
				cmd.HSceneName = value;
				Apply();
			});
		}

		private string FallbackAnchorNameTransaction()
		{
			inputAnchorName.SetTextWithoutNotify(m_lastAnchorName);
			return m_lastAnchorName;
		}
		
		private void SaveAnchorNameTransaction(string value)
		{
			m_lastAnchorName = value;
		}

		private List<string> FallbackAnchorTagsTransaction()
		{
			inputAnchorTags.SetTextWithoutNotify(string.Join(",", m_lastAnchorTags));
			return m_lastAnchorTags;
		}

		private void SaveAnchorTagsTransaction(List<string> list)
		{
			m_lastAnchorTags = list;
		}

		private string FallbackHSceneNameTransaction()
		{
			inputHSceneName.SetTextWithoutNotify(m_lastHSceneName);
			return m_lastHSceneName;
		}

		private void SaveHSceneNameTransaction(string value)
		{
			m_lastHSceneName = value;
		}

		public override void SetInfo<C>(C command, ArptScriptCmdItem parentItem)
		{
			base.SetInfo(command, parentItem);
			#region anchorName (+ backup transaction)
			inputAnchorName.SetTextWithoutNotify(cmd.AnchorName);
			m_lastAnchorName = cmd.AnchorName;
			#endregion

			#region anchorTags (+ backup transaction)
			string tags = string.Join(",", cmd.AnchorTags);
			inputAnchorTags.SetTextWithoutNotify(tags);
			m_lastAnchorTags = cmd.AnchorTags.ToList();
			#endregion

			#region hSceneName (+ backup transaction)
			inputHSceneName.SetTextWithoutNotify(cmd.HSceneName);
			m_lastHSceneName = cmd.HSceneName;
			#endregion
		}

		public override void Apply(object tag = null)
		{
			parentItem.UpdateInfo(cmd);
		}

		public override void InitNewInfo()
		{
			inputAnchorName.SetTextWithoutNotify(Guid.NewGuid().ToString("N"));
			inputAnchorTags.SetTextWithoutNotify(string.Empty);
			inputHSceneName.SetTextWithoutNotify($"NewAnchorHScene");
		}
	}
}