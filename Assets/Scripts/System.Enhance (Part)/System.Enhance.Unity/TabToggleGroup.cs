using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace System.Enhance.Unity
{
	[RequireComponent(typeof(ToggleGroup))]
	public class TabToggleGroup : MonoBehaviour
	{
		public List<Toggle> toggles = new List<Toggle>();

		[SerializeField]
		internal List<TabPage> pages = new List<TabPage>();

		private ToggleGroup toggleGroup;
		public ToggleGroup ParentToggleGroup
		{
			get
			{
				if (toggleGroup == null) // singleton pattern as 'Lazy Pattern'
				{
					toggleGroup = GetComponent<ToggleGroup>();
				}
				return toggleGroup;
			}
		}

		public void InitTabToggles()
		{
			ParentToggleGroup.allowSwitchOff = false;
			toggles.ForEach(t =>
			{
				t.onValueChanged.RemoveAllListeners();
				t.SetIsOnWithoutNotify(false);
				t.group = ParentToggleGroup;
			});
			pages.ForEach(p => p.parentGroup = this);
			for (int i = 0; i < toggles.Count; i++)
			{
				var toggle = toggles[i];
				var page = pages[i];
				toggle.onValueChanged.AddListener(isOn =>
				{
					if (!isOn) return;
					page.ShowMe(); // == ShowMe() + HideOthers()
				});
			}
			// on(v.) the first toggle
			toggles[0].isOn = true;
		}
	}
}
