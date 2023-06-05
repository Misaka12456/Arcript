using Arcript.Aspt.RawArcVNScripts;
using Arcript.Compose.Inspectors;
using UnityEngine;
using UnityEngine.UI;

namespace Arcript.Compose
{
	public class ArptScriptCmdItem : MonoBehaviour
	{
		public static readonly Color Transparent = new Color(1, 1, 1, 0);

		[Header("General")]
		public Text labelCmdText;
		public Toggle toggleChoosing;
		public Text labelIsBlockCmdSign;
		
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

		public void SetInfo(string cmdShortInfo, bool isBlock, ToggleGroup parentGroup)
		{
			labelCmdText.supportRichText = true; // 启用富文本，以确保<b>等标签能正常解析并显示
			labelCmdText.horizontalOverflow = HorizontalWrapMode.Overflow; // 指令文本超出时不换行(直接向右Overflow，反正有Mask)(因为指令文本只有一行)

			// 将cmdShortInfo中所有的换行都去掉，防止无法在一行上完全显示
			cmdShortInfo = cmdShortInfo.Replace("\r\n", string.Empty).Replace("\n", string.Empty);

			labelCmdText.text = cmdShortInfo;

			//labelIsBlockCmdSign.gameObject.SetActive(isBlock);
			// 这里直接SetActive会导致布局意义上直接消失了，所以这里改用alpha
			labelIsBlockCmdSign.color = isBlock ? Color.white : Transparent;

			parentGroup.RegisterToggle(toggleChoosing); // == toggleChoosing.group = parentGroup;
		}
	}
}
