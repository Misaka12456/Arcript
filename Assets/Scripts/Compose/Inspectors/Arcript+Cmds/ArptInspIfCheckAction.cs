using Arcript.Aspt;
using Arcript.Compose.Dialogs;
using UnityEngine;
using UnityEngine.UI;

namespace Arcript.Compose.Inspectors
{
	public class ArptInspIfCheckAction : MonoBehaviour
	{
		public Text labelActionCmdShortInfo;
		public Button btnShowEditDialog;

		[HideInInspector] public AsptCmdBase cmd;
		[HideInInspector] public bool isTrueCmd;
		private ArptInspIfCheck m_parentPanel;

		private void Awake()
		{
			btnShowEditDialog.onClick.AddListener(() =>
			{
				// 以下代码其实只实现了下面一行伪代码:
				// ArptRawCmdEditDialog.Instance.ShowDialog<cmd.GetType()>(cmd, new Action<cmd.GetType()>(SaveCallback));
				// 但众所周知，泛型类的类型参数是不能直接传System.Type的，所以……
				// --------------
				// 获取cmd的实际继承类Type
				var cmdActualType = cmd.GetType();

				var showDialogMethod = typeof(ArptRawCmdEditDialog).GetMethod("ShowDialog");
				var genericMethod = showDialogMethod.MakeGenericMethod(new[] { cmdActualType });

				var saveCallback = (object obj) =>
				{
					SaveCallback(obj, isTrueCmd);
				}; // saveCallback.GetType() == Action<object>
				genericMethod.Invoke(ArptRawCmdEditDialog.Instance, new object[] { cmd, saveCallback });
				// --------------
			});
		}

		public void SetInfo(ArptInspIfCheck parent, AsptCmdBase cmd, bool isTrue)
		{
			m_parentPanel = parent;
			this.cmd = cmd;
			isTrueCmd = isTrue;
			labelActionCmdShortInfo.text = cmd.ToItemShortString();
		}

		public void SetAsNewInfo(ArptInspIfCheck parent, bool isTrue)
		{
			cmd = null;
			m_parentPanel = parent;
			labelActionCmdShortInfo.text = "<new>";
			isTrueCmd = isTrue;

			var saveCallback = (object obj) =>
			{
				SaveCallback(obj, isTrue);
			};

			var discardCallback = () =>
			{
				m_parentPanel.OnActionChanged(this, isTrue: isTrueCmd, isRemove: true);
			};

			ArptRawCmdEditDialog.Instance.ShowDialogAsNewCmd(saveCallback, discardCallback);
		}

		private void SaveCallback(object cmd, bool @new = false)
		{
			this.cmd = cmd as AsptCmdBase;
			labelActionCmdShortInfo.text = this.cmd.ToItemShortString();
			Apply(@new);
		}

		private void Apply(bool @new = false)
		{
			m_parentPanel.OnActionChanged(this, isTrue: isTrueCmd, isAdd: @new);
		}
	}
}