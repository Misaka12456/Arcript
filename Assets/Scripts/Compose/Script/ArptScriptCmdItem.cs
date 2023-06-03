using Arcript.ArcVNScripts;
using Arcript.Compose.Inspectors;
using UnityEngine;
using UnityEngine.UI;

namespace Arcript.Compose.Script
{
	public class ArptScriptCmdItem : MonoBehaviour
	{
		[Header("General")]
		public Text labelCmdText;
		public Toggle toggleChoosing;

		[SerializeField]
		private ArcVNScriptCmdBase refCmd;

		private void Awake()
		{
			toggleChoosing.onValueChanged.AddListener((bool isOn) =>
			{
				if (!isOn) return;
				ArptItemInspector.Instance.SetCmdInfo(this, refCmd);
			});
		}
	}
}
