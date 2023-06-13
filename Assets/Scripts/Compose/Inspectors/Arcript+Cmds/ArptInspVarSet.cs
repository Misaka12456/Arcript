using Arcript.Aspt;
using Arcript.Compose.UI;
using UnityEngine.UI;

namespace Arcript.Compose.Inspectors
{
	[CmdInspectExport(typeof(AsptVarSetCmd), "varSet", "Arcript+ Script Variable Set", "Resources/Prefabs/Editor/Inspectors/varSet")]
	public class ArptInspVarSet : InspectCmdPanelBase<AsptVarSetCmd>
	{
		public InputField inputVarName;
		public InputField inputSetValue;

		private string m_lastVarName, m_lastSetValueStr;

		protected override void InspectorAwake()
		{
			inputVarName.onValueChanged.AddListener((value) =>
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					value = FallbackVarNameTransaction();
				}
				else
				{
					SaveVarNameTransaction(value);
				}
				cmd.VarName = value;
				Apply();
			});

			inputSetValue.onValueChanged.AddListener((value) =>
			{
				if (int.TryParse(value, out int intValue))
				{
					SaveSetValueTransaction(value);
				}
				else
				{
					value = FallbackSetValueTransaction();
					intValue = int.Parse(value);
				}
				cmd.TargetValue = intValue;
				Apply();
			});
		}

		private string FallbackVarNameTransaction()
		{
			inputVarName.SetTextWithoutNotify(m_lastVarName);
			return m_lastVarName;
		}

		private void SaveVarNameTransaction(string varName)
		{
			m_lastVarName = varName;
		}

		private string FallbackSetValueTransaction()
		{
			inputSetValue.SetTextWithoutNotify(m_lastSetValueStr);
			return m_lastSetValueStr;
		}

		private void SaveSetValueTransaction(string setValueStr)
		{
			m_lastSetValueStr = setValueStr;
		}

		public override void SetInfo<C>(C command, ArptScriptCmdItem parentItem)
		{
			base.SetInfo(command, parentItem);
			inputVarName.SetTextWithoutNotify(cmd.VarName);
			inputSetValue.SetTextWithoutNotify(cmd.TargetValue.ToString());
		}

		public override void Apply(object tag = null)
		{
			parentItem.UpdateInfo(cmd);
		}

		public override void InitNewInfo()
		{
			inputVarName.SetTextWithoutNotify("MyVar");
			inputSetValue.SetTextWithoutNotify("0");
			m_lastVarName = "MyVar";
			m_lastSetValueStr = "0";
		}
	}
}