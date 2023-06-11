using Arcript.Aspt;
using Arcript.Compose.Dialogs;
using UnityEngine;
using UnityEngine.UI;

namespace Arcript.Compose.Inspectors
{
	public class ArptInspBSOptionRequirement : MonoBehaviour
	{
		public Text labelReqExpression;
		public Button btnShowEditDialog;

		[HideInInspector] public AsptVarCheckCmd cmd;
		private ArptInspBranchSelectOption m_parentOption;

		private void Awake()
		{
			btnShowEditDialog.onClick.AddListener(() =>
			{
				// 都确定是AsptVarCheckCmd了还反射什么，直接调用喽！
				ArptRawCmdEditDialog.Instance?.ShowDialog(cmd, (obj) =>
				{
					SaveCallback(obj);
				});
			});
		}

		public void SetInfo(ArptInspBranchSelectOption parentOption, AsptVarCheckCmd cmd)
		{
			m_parentOption = parentOption;
			this.cmd = cmd;
			labelReqExpression.text = cmd.Expression.Substring(0, Mathf.Min(20, cmd.Expression.Length));
		}

		public void SetAsNewInfo(ArptInspBranchSelectOption parentOption)
		{
			cmd = null;
			m_parentOption = parentOption;
			labelReqExpression.text = "<new>";

			var saveCallback = (object result) =>
			{
				var cmd = result as AsptVarCheckCmd;
				SaveCallback(cmd, true);
			};

			var discardCallback = () =>
			{
				m_parentOption.OnRequirementChanged(this, isRemove: true);
			};

			ArptRawCmdEditDialog.Instance.ShowDialogAsNewCmd(saveCallback, discardCallback);
		}

		private void SaveCallback(AsptVarCheckCmd cmd, bool @new = false)
		{
			this.cmd = cmd;
			labelReqExpression.text = cmd.Expression.Substring(0, Mathf.Min(20, cmd.Expression.Length));
			Apply(@new);
		}

		private void Apply(bool @new = false)
		{
			m_parentOption.OnRequirementChanged(this, isAdd: @new);
		}
	}
}