using System.Collections.Generic;
using UnityEngine;
using TMPText = TMPro.TextMeshProUGUI;

[RequireComponent(typeof(TreeViewControl))]
public class TreeViewTest : MonoBehaviour
{
	private void Awake()
	{
		var tv = GetComponent<TreeViewControl>();
		var datas = new List<TreeViewData>()
		{
			new TreeViewData(tv)
			{
				Name = "Project Root",
				ParentID = -1
			},
			new TreeViewData(tv)
			{
				Name = "Resources",
				ParentID = 0
			},
			new TreeViewData(tv)
			{
				Name = "fl²_chap2",
				ParentID = 1
			},
			new TreeViewData(tv)
			{
				Name = "sd",
				ParentID = 2
			},
			new TreeViewData(tv)
			{
				Name = "arc_meeting",
				ParentID = 1
			},
			new TreeViewData(tv)
			{
				Name = "stands",
				ParentID = 4
			},
			new TreeViewData(tv)
			{
				Name = "bgs",
				ParentID = 4
			},
		};
		tv.Data = datas;
		tv.GenerateTreeView();
		tv.RefreshTreeView();
		tv.ClickItemEvent += TreeView_Clicked;
	}

	private void TreeView_Clicked(GameObject itemObj)
	{
		string text = itemObj.transform.Find("TreeViewText").GetComponent<TMPText>().text;
		var item = itemObj.GetComponent<TreeViewItem>();
		Debug.Log($"You clicked item {text} (path: {item.Data}");
	}
}
