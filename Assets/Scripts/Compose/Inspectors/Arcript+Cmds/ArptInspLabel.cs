using Arcript.Aspt;
using UnityEngine.UI;

namespace Arcript.Compose.Inspectors
{
	[CmdInspectExport(typeof(AsptLabelCmd), "label", "Arcript+ Script Label")]
	public class ArptInspLabel : InspectCmdPanelBase<AsptLabelCmd>
	{
		public InputField inputLabelName;

		private string m_lastLabelName;

		protected override void InspectorAwake()
		{
			inputLabelName.onValueChanged.AddListener((value) =>
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					value = FallbackLabelNameTransaction();
				}
				else
				{
					SaveLabelNameTransaction(value);
				}
				cmd.LabelName = value;
				Apply();
			});
		}

		private string FallbackLabelNameTransaction()
		{
			inputLabelName.SetTextWithoutNotify(m_lastLabelName);
			return m_lastLabelName;
		}

		private void SaveLabelNameTransaction(string value)
		{
			m_lastLabelName = value;
		}

		public override void SetInfo<C>(C command, ArptScriptCmdItem parentItem)
		{
			base.SetInfo(command, parentItem);

			#region labelName
			inputLabelName.SetTextWithoutNotify(cmd.LabelName);
			m_lastLabelName = cmd.LabelName;
			#endregion
		}

		public override void Apply(object tag = null)
		{
			parentItem.UpdateInfo(cmd);
		}

		public override void InitNewInfo()
		{
			inputLabelName.SetTextWithoutNotify("MyNewLabel");
			m_lastLabelName = "MyNewLabel";
		}
	}
}