using Arcript.Aspt;
using Arcript.Compose.Dialogs;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Arcript.Compose.Inspectors
{
	[CmdInspectExport(typeof(AsptVarCheckCmd), "varCheck", "Arcript+ Script Variable Check", "Resources/Prefabs/Editor/Inspectors/varCheck")]
	public class ArptInspVarCheck : InspectCmdPanelBase<AsptVarCheckCmd>
	{
		public Text labelReqExpression;
		public Button btnShowEditDialog;

		protected override void InspectorAwake()
		{
			btnShowEditDialog.onClick.AddListener(() =>
			{
				ArptRawCmdEditDialog.Instance?.ShowDialog(cmd, (obj) =>
				{
					SaveCallback(obj);
				});
			});

		}

		public override void SetInfo<C>(C command, ArptScriptCmdItem parentItem)
		{
			base.SetInfo(command, parentItem);
			labelReqExpression.text = cmd.Expression.Substring(0, Mathf.Min(20, cmd.Expression.Length));
		}

		private void SaveCallback(AsptVarCheckCmd cmd)
		{
			this.cmd = cmd;
			labelReqExpression.text = cmd.Expression.Substring(0, Mathf.Min(20, cmd.Expression.Length));
			Apply();
		}

		public override void Apply(object tag = null)
		{
			parentItem.UpdateInfo(cmd);
		}

		public override void InitNewInfo()
		{
			cmd = null;
			labelReqExpression.text = "<new>";

			var saveCallback = new Action<object>((object result) =>
			{
				var cmd = result as AsptVarCheckCmd;
				SaveCallback(cmd);
			});

			ArptRawCmdEditDialog.Instance.ShowDialogAsNewCmd(saveCallback, null);
		}
	}
}