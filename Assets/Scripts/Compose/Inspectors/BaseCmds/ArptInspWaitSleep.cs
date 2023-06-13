using Arcript.Aspt;
using UnityEngine.UI;

namespace Arcript.Compose.Inspectors
{
	[CmdInspectExport(typeof(AsptWaitSleepCmd), "wait", "Arcript Script Wait/Sleep", "Resources/Prefabs/Editor/Inspectors/wait")]
	public class ArptInspWaitSleep : InspectCmdPanelBase<AsptWaitSleepCmd>
	{
		public InputField inputDuration;

		private float m_lastDuration;

		protected override void InspectorAwake()
		{
			inputDuration.onValueChanged.AddListener((value) =>
			{
				if (!float.TryParse(value, out float r))
				{
					value = FallbackDurationTransaction();
					r = float.Parse(value);
				}
				else
				{
					SaveDurationTransaction(value);
				}
				cmd.Duration = r;
				Apply();
			});
		}

		private string FallbackDurationTransaction()
		{
			inputDuration.SetTextWithoutNotify(m_lastDuration.ToString());
			return m_lastDuration.ToString();
		}

		private void SaveDurationTransaction(string value)
		{
			m_lastDuration = float.Parse(value);
		}

		public override void SetInfo<C>(C command, ArptScriptCmdItem parentItem)
		{
			base.SetInfo(command, parentItem);

			#region duration
			inputDuration.SetTextWithoutNotify(cmd.Duration.ToString());
			m_lastDuration = cmd.Duration;
			#endregion
		}

		public override void Apply(object tag = null)
		{
			parentItem.UpdateInfo(cmd);
		}

		public override void InitNewInfo()
		{
			inputDuration.SetTextWithoutNotify("0.5");
			m_lastDuration = 0.5f;
		}
	}
}