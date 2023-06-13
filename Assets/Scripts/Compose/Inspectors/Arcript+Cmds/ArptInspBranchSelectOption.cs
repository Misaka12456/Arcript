using Arcript.Aspt;
using System;
using System.Enhance.Unity;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Arcript.Compose.Inspectors
{

	[CmdInspectExport(typeof(AsptBranchSelectOptionCmd), "select", "Arcript+ Branch Select Option", "Resources/Prefabs/Editor/Inspectors/select")]
	public class ArptInspBranchSelectOption : MonoBehaviour
	{
		public InputField inputOptionId;
		public InputField inputFriendlyText;
		public InputField inputGotoLabelOrAnchor;
		public VerticalLayoutGroup grpListChooseActions;
		public VerticalLayoutGroup grpListRequirements; // 允许选择该选项的前置条件
		public Button btnAddChooseAction, btnAddRequirement; // 移除选项使用"Delete"键

		[HideInInspector] public AsptBranchSelectOptionCmd cmd;
		[SerializeField] private GameObject m_prefabOptionActionItem;
		[SerializeField] private GameObject m_prefabOptionRequirementItem;
		private ArptInspBranchSelect m_parentPanel;

		private string m_lastOptionId, m_lastFriendlyText;

		private void Awake()
		{
			inputOptionId.onValueChanged.AddListener((value) =>
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					value = FallbackLastOptIdTransaction();
				}
				else
				{
					SaveLastOptIdTransaction(value);
				}
				Apply();
			});
			inputFriendlyText.onValueChanged.AddListener((value) =>
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					value = FallbackLastFriendlyTextTransaction();
				}
				else
				{
					SaveLastFriendlyTextTransaction(value);
				}
				Apply();
			});
			inputGotoLabelOrAnchor.onValueChanged.AddListener((value) =>
			{
				Apply();
			});
			btnAddChooseAction.onClick.AddListener(() =>
			{
				var actionGObj = Instantiate(m_prefabOptionActionItem, grpListChooseActions.transform);
				var actionInst = actionGObj.GetComponent<ArptInspBSOptionAction>();
				actionInst.SetAsNewInfo(this);
			});
			btnAddRequirement.onClick.AddListener(() =>
			{
				var reqGObj = Instantiate(m_prefabOptionRequirementItem, grpListRequirements.transform);
				var reqInst = reqGObj.GetComponent<ArptInspBSOptionRequirement>();
				reqInst.SetAsNewInfo(this);
			});
		}

		public void SetInfo(ArptInspBranchSelect parent, AsptBranchSelectOptionCmd cmd)
		{
			m_parentPanel = parent;
			this.cmd = cmd;

			inputOptionId.SetTextWithoutNotify(cmd.OptionId);
			m_lastOptionId = cmd.OptionId;

			inputFriendlyText.SetTextWithoutNotify(cmd.FriendlyText);
			m_lastFriendlyText = cmd.FriendlyText;

			inputGotoLabelOrAnchor.SetTextWithoutNotify(cmd.GotoLabelOrAnchor);
			
			grpListChooseActions.transform.DestroyAllChildren();
			foreach (var action in cmd.ChooseActions)
			{
				var item = Instantiate(m_prefabOptionActionItem, grpListChooseActions.transform);
				item.GetComponent<ArptInspBSOptionAction>().SetInfo(this, action);
			}

			grpListRequirements.transform.DestroyAllChildren();
			foreach (var requirement in cmd.Requirements)
			{
				var item = Instantiate(m_prefabOptionRequirementItem, grpListRequirements.transform);
				item.GetComponent<ArptInspBSOptionRequirement>().SetInfo(this, requirement);
			}
		}

		private string FallbackLastOptIdTransaction()
		{
			inputOptionId.SetTextWithoutNotify(m_lastOptionId);
			return m_lastOptionId;
		}

		private void SaveLastOptIdTransaction(string value)
		{
			m_lastOptionId = value;
		}

		private string FallbackLastFriendlyTextTransaction()
		{
			inputFriendlyText.SetTextWithoutNotify(m_lastFriendlyText);
			return m_lastFriendlyText;
		}

		private void SaveLastFriendlyTextTransaction(string value)
		{
			m_lastFriendlyText = value;
		}

		private void Apply()
		{
			m_parentPanel?.OnSelectOptionChanged(this);
		}

		internal void OnChooseActionChanged(ArptInspBSOptionAction source, bool isAdd = false, bool isRemove = false)
		{
			var actions = cmd.ChooseActions;
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
				newList.Clear(); // same as newList.Dispose();
			}
			else
			{
				var actionBefore = actions[source.transform.GetSiblingIndex()];
				var actionAfter = source.cmd;
				int idx = Array.IndexOf(actions, actionBefore);
				actions[idx] = actionAfter;
			}
			cmd.ChooseActions = actions;
			Apply();
			if (isRemove)
			{
				Destroy(source.gameObject);
			}
		}

		internal void OnRequirementChanged(ArptInspBSOptionRequirement source, bool isAdd = false, bool isRemove = false)
		{
			var reqs = cmd.Requirements;
			if (isAdd)
			{
				reqs = reqs.Append(source.cmd).ToArray();
			}
			else if (isRemove)
			{
				int idx = source.transform.GetSiblingIndex();
				var newList = reqs.ToList();
				newList.RemoveAt(idx);
				reqs = newList.ToArray();
				newList.Clear(); // same as newList.Dispose();
			}
			else
			{
				var reqBefore = reqs[source.transform.GetSiblingIndex()];
				var reqAfter = source.cmd;
				int idx = Array.IndexOf(reqs, reqBefore);
				reqs[idx] = reqAfter;
			}
			cmd.Requirements = reqs;
			Apply();
			if (isRemove)
			{
				Destroy(source.gameObject);
			}
		}

		public void InitNewInfo()
		{
			inputOptionId.SetTextWithoutNotify("NewOption");
			inputFriendlyText.SetTextWithoutNotify("New Option Tip");
			inputGotoLabelOrAnchor.SetTextWithoutNotify(string.Empty); // no jump target; just continue as normal
			grpListChooseActions.transform.DestroyAllChildren();
			grpListRequirements.transform.DestroyAllChildren();
			btnAddChooseAction.interactable = btnAddRequirement.interactable = true;
		}
	}
}