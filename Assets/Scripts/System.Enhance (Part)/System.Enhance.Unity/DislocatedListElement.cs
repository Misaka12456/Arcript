using UnityEngine;
using UnityEngine.UI;
using UGUIText = UnityEngine.UI.Text; // 由于"System."开头的命名空间原因，"Text"会被识别为"System.Text"，所以这里使用别名

namespace System.Enhance.Unity
{
	public class DislocatedListElement : MonoBehaviour
	{
		public UGUIText labelContent;
	}
}