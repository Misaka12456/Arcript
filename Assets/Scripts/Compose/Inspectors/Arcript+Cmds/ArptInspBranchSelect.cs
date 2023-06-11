using Arcript.Aspt;
using Arcript.Compose.Dialogs;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Enhance.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Arcript.Compose.Inspectors
{
	[CmdInspectExport(typeof(AsptBranchSelectCmd), "selectSay")]
	public class ArptInspBranchSelect : InspectCmdPanelBase<AsptBranchSelectCmd>
	{
		public InputField inputSelectTip;
		public VerticalLayoutGroup grpListOptions;
		public Button btnAddOption; // 移除选项使用"Delete"键
		
		[SerializeField] private GameObject m_prefabOptionItem;
		private string m_lastSelectTip;

		protected override void InspectorAwake()
		{
			toggleIsBlock.interactable = false;
			toggleIsBlock.SetIsOnWithoutNotify(true); // 选择支必定会block
			inputSelectTip.onValueChanged.AddListener((value) =>
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					value = FallbackSelectTipTransaction();
				}
				else
				{
					SaveSelectTipTransaction(value);
				}
				cmd.SelectTip = value;
				Apply();
			});
		}

		public override void SetInfo<C>(C command, ArptScriptCmdItem parentItem)
		{
			base.SetInfo(command, parentItem);
			inputSelectTip.SetTextWithoutNotify(cmd.SelectTip);
			m_lastSelectTip = cmd.SelectTip;

			grpListOptions.transform.DestroyAllChildren();
			foreach (var opt in cmd.Options)
			{
				var go = Instantiate(m_prefabOptionItem, grpListOptions.transform);
				var item = go.GetComponent<ArptInspBranchSelectOption>();
				item.SetInfo(this, opt);
			}
		}

		private string FallbackSelectTipTransaction()
		{
			inputSelectTip.SetTextWithoutNotify(m_lastSelectTip);
			return m_lastSelectTip;
		}

		public void SaveSelectTipTransaction(string value)
		{
			m_lastSelectTip = value;
		}

		internal void OnSelectOptionChanged(ArptInspBranchSelectOption source)
		{
			var options = cmd.Options;
			var cmdBeforeEdit = options[source.transform.GetSiblingIndex()];
			var cmdAfterEdit = source.cmd;
			int idx = Array.IndexOf(options, cmdBeforeEdit);
			options[idx] = cmdAfterEdit;
			cmd.Options = options;
			Apply();
		}

		public override void Apply(object tag = null)
		{
			parentItem.UpdateInfo(cmd);
		}

		public override void InitNewInfo()
		{
			inputSelectTip.SetTextWithoutNotify("New Branch Select Tip");
			grpListOptions.transform.DestroyAllChildren();
			btnAddOption.interactable = true;
		}
	}
}