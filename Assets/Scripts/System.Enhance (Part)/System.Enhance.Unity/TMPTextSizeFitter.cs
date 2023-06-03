using UnityEngine;
using TMPText = TMPro.TextMeshProUGUI;
using UnityEngine.UI;

namespace System.Enhance.Unity
{
	[RequireComponent(typeof(TMPText))]
	[RequireComponent(typeof(LayoutElement))]
	[ExecuteInEditMode]
	public class TMPTextSizeFitter : MonoBehaviour
	{
		private TMPText _text;
		private LayoutElement _layoutSettings;
		public TMPText Text
		{
			get
			{
				if (_text == null)
				{
					_text = GetComponent<TMPText>();
				}
				return _text;
			}
		}
		public LayoutElement BaseLayout
		{
			get
			{
				if (_layoutSettings == null)
				{
					_layoutSettings = GetComponent<LayoutElement>();
				}
				return _layoutSettings;
			}
		}

		private void OnRectTransformDimensionsChange()
		{
			Adjust(auto: true);
		}

		public void Adjust(bool auto = false)
		{
			if (!Text.autoSizeTextContainer && auto)
			{
				Debug.LogWarning("TextSizeFitter: Text.autoSizeTextContainer is false. Automatically set to true for calculation.");
				Text.autoSizeTextContainer = true;
				Text.ClearMesh(); // refresh
				Text.ForceMeshUpdate();
			}
			float width = Text.preferredWidth;
			float height = Text.preferredHeight;
			BaseLayout.preferredWidth = width;
			BaseLayout.preferredHeight = height;
		}
	}
}
