using Arcript.Aspt;
using UnityEngine.UI;

namespace Arcript.Compose.Inspectors
{
	[CmdInspectExport(typeof(AsptVarDefineCmd), "var", "Arcript+ Script Variable Definition", "Resources/Prefabs/Editor/Inspectors/var")]
	public class ArptInspVarDefine : InspectCmdPanelBase<AsptVarDefineCmd>
	{
		public InputField inputVarName;
		public InputField inputInitValue;

		private string m_lastVarName, m_lastInitValueStr;

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

			inputInitValue.onValueChanged.AddListener((value) =>
			{
				if (int.TryParse(value, out int intValue))
				{
					SaveInitValueTransaction(value);
				}
				else
				{
					value = FallbackInitValueTransaction();
					intValue = int.Parse(value);
				}
				cmd.InitValue = intValue;
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

		private string FallbackInitValueTransaction()
		{
			inputInitValue.SetTextWithoutNotify(m_lastInitValueStr);
			return m_lastInitValueStr;
		}

		private void SaveInitValueTransaction(string initValueStr)
		{
			m_lastInitValueStr = initValueStr;
		}

		public override void SetInfo<C>(C command, ArptScriptCmdItem parentItem)
		{
			base.SetInfo(command, parentItem);
			inputVarName.SetTextWithoutNotify(cmd.VarName);
			inputInitValue.SetTextWithoutNotify(cmd.InitValue.ToString());
		}

		public override void Apply(object tag = null)
		{
			parentItem.UpdateInfo(cmd);
		}

		public override void InitNewInfo()
		{
			inputVarName.SetTextWithoutNotify("MyNewVar");
			inputInitValue.SetTextWithoutNotify("0");
			m_lastVarName = "MyNewVar";
			m_lastInitValueStr = "0";
		}
	}
}