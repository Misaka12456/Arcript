using System.Linq;
using UnityEngine;

namespace System.Enhance.Unity
{
	[RequireComponent(typeof(RectTransform))]
	public class TabPage : MonoBehaviour
	{
		public string TabId;
		private RectTransform pageTransform;
		public RectTransform PageTransform
		{
			get
			{
				if (pageTransform == null)
				{
					pageTransform = GetComponent<RectTransform>();
				}
				return pageTransform;
			}
		}

		[HideInInspector]
		public TabToggleGroup parentGroup;

		public void ShowMe()
		{
			var pages = parentGroup.pages;
			var myPage = pages.First(p => p.TabId == TabId);
			var otherPages = pages.Where(p => p.TabId != TabId && p.gameObject.activeSelf).ToList();
			myPage.gameObject.SetActive(true);
			otherPages.ForEach(p => p.gameObject.SetActive(false));
		}
	}
}
