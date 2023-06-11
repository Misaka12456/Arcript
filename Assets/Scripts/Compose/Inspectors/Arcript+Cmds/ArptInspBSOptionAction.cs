using Arcript.Aspt;
using Arcript.Compose.Dialogs;
using UnityEngine;
using UnityEngine.UI;

namespace Arcript.Compose.Inspectors
{
	public class ArptInspBSOptionAction : MonoBehaviour
	{
		public Text labelActionCmdShortInfo;
		public Button btnShowEditDialog;

		[HideInInspector] public AsptCmdBase cmd;
		private ArptInspBranchSelectOption m_parentOption;

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

				var saveCallback = (object cmd) =>
				{
					SaveCallback(cmd);
				};
				genericMethod.Invoke(ArptRawCmdEditDialog.Instance, new object[] { cmd, saveCallback });
				// --------------
			});
		}

		public void SetInfo(ArptInspBranchSelectOption parentOption, AsptCmdBase cmd)
		{
			m_parentOption = parentOption;
			this.cmd = cmd;
			labelActionCmdShortInfo.text = cmd.ToItemShortString();
		}

		public void SetAsNewInfo(ArptInspBranchSelectOption parentOption)
		{
			cmd = null;
			m_parentOption = parentOption;
			labelActionCmdShortInfo.text = "<new>";

			var discardCallback = () =>
			{
				m_parentOption.OnChooseActionChanged(this, isRemove: true);
			};

			ArptRawCmdEditDialog.Instance.ShowDialogAsNewCmd((obj) =>
			{
				SaveCallback(obj, true);
			}, discardCallback);
		}

		private void SaveCallback(object cmd, bool @new = false)
		{
			this.cmd = cmd as AsptCmdBase;
			labelActionCmdShortInfo.text = this.cmd.ToItemShortString();
			Apply(@new);
		}

		private void Apply(bool @new = false)
		{
			m_parentOption.OnChooseActionChanged(this, isAdd: @new);
		}
	}
}