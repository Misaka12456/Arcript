using Arcript.Aspt;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Arcript.Compose.Inspectors
{
	[CmdInspectExport(typeof(AsptAnchorCmd), "anchor")]
	public class ArptInspAnchor : InspectCmdPanelBase<AsptAnchorCmd>
	{
		[Header("General")]
		public InputField inputAnchorName;
		public InputField inputAnchorTags;

		private string m_lastAnchorName = Guid.NewGuid().ToString("N");
		private List<string> m_lastAnchorTags = new List<string>();

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

		public override void SetInfo<C>(C command, ArptScriptCmdItem parentItem)
		{
			base.SetInfo(command, parentItem);

			#region anchorName
			inputAnchorName.SetTextWithoutNotify(cmd.AnchorName);
			#endregion

			#region anchorTags (+ backup transaction)
			string tags = string.Join(",", cmd.AnchorTags);
			inputAnchorTags.SetTextWithoutNotify(tags);
			m_lastAnchorTags = cmd.AnchorTags.ToList();
			#endregion
		}

		public override void Apply(object tag = null)
		{
			parentItem.UpdateInfo(cmd);
		}
	}
}