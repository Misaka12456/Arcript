using UnityEngine;
using UGUIText = UnityEngine.UI.Text;
using UnityEngine.UI;

namespace System.Enhance.Unity
{
	[RequireComponent(typeof(UGUIText))]
	[ExecuteInEditMode]
	public class TextSizeFitter : MonoBehaviour
	{
		private UGUIText _text;
		private LayoutElement _layoutSettings;
		public UGUIText Text
		{
			get
			{
				if (_text == null)
				{
					_text = GetComponent<UGUIText>();
				}
				return _text;
			}
		}

		private void OnRectTransformDimensionsChange()
		{
			Adjust();
		}

		public void Adjust()
		{
			float width = Text.preferredWidth;
			float height = Text.preferredHeight;
			GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
		}
	}
}
