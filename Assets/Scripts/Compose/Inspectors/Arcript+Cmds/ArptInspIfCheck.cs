using Arcript.Aspt;
using System;
using System.Enhance.Unity;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Arcript.Compose.Inspectors
{
	[CmdInspectExport(typeof(AsptIfCheckCmd), "if", "Arcript+ Script If Check")]
	public class ArptInspIfCheck : InspectCmdPanelBase<AsptIfCheckCmd>
	{
		[Header("General")]
		public VerticalLayoutGroup groupListConditions;
		public VerticalLayoutGroup groupListTrueCmds, groupListFalseCmds;
		public Button btnAddCondition, btnAddTrueCmd, btnAddFalseCmd;

		[SerializeField] private GameObject m_prefabConditionItem;
		[SerializeField] private GameObject m_prefabCmdItem;

		protected override void InspectorAwake()
		{
			btnAddCondition.onClick.AddListener(() =>
			{
				var condGObj = Instantiate(m_prefabConditionItem, groupListConditions.transform);
				var condInst = condGObj.GetComponent<ArptInspIfCheckCondition>();
				condInst.SetAsNewInfo(this);
			});

			btnAddTrueCmd.onClick.AddListener(() =>
			{
				var tcGObj = Instantiate(m_prefabCmdItem, groupListTrueCmds.transform);
				var tcInst = tcGObj.GetComponent<ArptInspIfCheckAction>();
				tcInst.SetAsNewInfo(this, isTrue: true);
			});

			btnAddFalseCmd.onClick.AddListener(() =>
			{
				var fcGObj = Instantiate(m_prefabCmdItem, groupListFalseCmds.transform);
				var fcInst = fcGObj.GetComponent<ArptInspIfCheckAction>();
				fcInst.SetAsNewInfo(this, isTrue: false);
			});
		}

		public override void SetInfo<C>(C command, ArptScriptCmdItem parentItem)
		{
			base.SetInfo(command, parentItem);

			groupListConditions.transform.DestroyAllChildren();
			groupListTrueCmds.transform.DestroyAllChildren();
			groupListFalseCmds.transform.DestroyAllChildren();
			foreach (var cond in cmd.Conditions)
			{
				var go = Instantiate(m_prefabConditionItem, groupListConditions.transform);
				var condItem = go.GetComponent<ArptInspIfCheckCondition>();
				condItem.SetInfo(this, cond);
			}
			foreach (var tc in cmd.TrueCmds)
			{
				var go = Instantiate(m_prefabCmdItem, groupListTrueCmds.transform);
				var cmdItem = go.GetComponent<ArptInspIfCheckAction>();
				cmdItem.SetInfo(this, tc, isTrue: true);
			}
			foreach (var fc in cmd.FalseCmds)
			{
				var go = Instantiate(m_prefabCmdItem, groupListFalseCmds.transform);
				var cmdItem = go.GetComponent<ArptInspIfCheckAction>();
				cmdItem.SetInfo(this, fc, isTrue: false);
			}
		}

		internal void OnConditionChanged(ArptInspIfCheckCondition source, bool isAdd = false, bool isRemove = false)
		{
			var conds = cmd.Conditions;
			if (isAdd)
			{
				conds = conds.Append(source.cmd).ToArray();
			}
			else if (isRemove)
			{
				int idx = source.transform.GetSiblingIndex();
				var newList = conds.ToList();
				newList.RemoveAt(idx);
				conds = newList.ToArray();
				newList.Clear();
			}
			else
			{
				var condBefore = conds[source.transform.GetSiblingIndex()];
				var condAfter = source.cmd;
				int idx = Array.IndexOf(conds, condBefore);
				conds[idx] = condAfter;
			}
			cmd.Conditions = conds;
			Apply();
			if (isRemove)
			{
				Destroy(source.gameObject);
			}
		}

		internal void OnActionChanged(ArptInspIfCheckAction source, bool isTrue, bool isAdd = false, bool isRemove = false)
		{
			if (isTrue)
			{
				var actions = cmd.TrueCmds;
				if (isAdd)
				{
					actions = actions.Append(source.cmd).ToArray();
				}
				else if (isRemove)
				{
					int idx = source.transform.GetSiblingIndex();
					var newList = actions.ToList();
					newList.RemoveAt(idx);
					actions = newList.ToArray();
					newList.Clear();
				}
				else
				{
					var actionBefore = actions[source.transform.GetSiblingIndex()];
					var actionAfter = source.cmd;
					int idx = Array.IndexOf(actions, actionBefore);
					actions[idx] = actionAfter;
				}
				cmd.TrueCmds = actions;
			}
			else
			{
				var actions = cmd.FalseCmds;
				if (isAdd)
				{
					actions = actions.Append(source.cmd).ToArray();
				}
				else if (isRemove)
				{
					int idx = source.transform.GetSiblingIndex();
					var newList = actions.ToList();
					newList.RemoveAt(idx);
					actions = newList.ToArray();
					newList.Clear();
				}
				else
				{
					var actionBefore = actions[source.transform.GetSiblingIndex()];
					var actionAfter = source.cmd;
					int idx = Array.IndexOf(actions, actionBefore);
					actions[idx] = actionAfter;
				}
				cmd.FalseCmds = actions;
			}
			Apply();
			if (isRemove)
			{
				Destroy(source.gameObject);
			}
		}

		public override void Apply(object tag = null)
		{
			parentItem.UpdateInfo(cmd);
		}

		public override void InitNewInfo()
		{
			groupListConditions.transform.DestroyAllChildren();
			groupListTrueCmds.transform.DestroyAllChildren();
			groupListFalseCmds.transform.DestroyAllChildren();
		}
	}
}