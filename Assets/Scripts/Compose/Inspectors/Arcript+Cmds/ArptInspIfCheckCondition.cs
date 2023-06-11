using Arcript.Aspt;
using Arcript.Compose.Dialogs;
using UnityEngine;
using UnityEngine.UI;

namespace Arcript.Compose.Inspectors
{
	public class ArptInspIfCheckCondition : MonoBehaviour
	{
		public Text labelReqExpression;
		public Button btnShowEditDialog;

		[HideInInspector] public AsptVarCheckCmd cmd;
		private ArptInspIfCheck m_parentPanel;

		private void Awake()
		{
			btnShowEditDialog.onClick.AddListener(() =>
			{
				// 都确定是AsptVarCheckCmd了还反射什么，直接调用喽！
				ArptRawCmdEditDialog.Instance?.ShowDialog(cmd, (obj) =>
				{
					SaveCallback(cmd);
				});
			});
		}

		public void SetInfo(ArptInspIfCheck parent, AsptVarCheckCmd cmd)
		{
			m_parentPanel = parent;
			this.cmd = cmd;
			labelReqExpression.text = cmd.Expression.Substring(0, Mathf.Min(20, cmd.Expression.Length));
		}

		public void SetAsNewInfo(ArptInspIfCheck parent)
		{
			cmd = null;
			m_parentPanel = parent;
			labelReqExpression.text = "<new>";

			var saveCallback = (object result) =>
			{
				var cmd = result as AsptVarCheckCmd;
				SaveCallback(cmd, true);
			};

			var discardCallback = () =>
			{
				m_parentPanel.OnConditionChanged(this, isRemove: true);
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
			m_parentPanel.OnConditionChanged(this, isAdd: @new);
		}
	}
}