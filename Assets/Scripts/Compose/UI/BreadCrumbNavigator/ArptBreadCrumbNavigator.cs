using System.Collections.Generic;
using System.Enhance.Unity;
using System.Enhance.Unity.UI;
using UnityEngine;
using UnityEngine.UI;
using MsgBoxType = System.Enhance.Unity.UI.MsgBoxDialog.MsgBoxType;
using MsgBoxResult = System.Enhance.Unity.UI.MsgBoxDialog.MsgBoxResult;
using Arcript.I18n;
using System.IO;

namespace Arcript.Compose
{
	[RequireComponent(typeof(HorizontalLayoutGroup))]
	public class ArptBreadCrumbNavigator : Singleton<ArptBreadCrumbNavigator>
	{
		[HideInInspector] public string CurrentFullPath;

		[Header("Lists")]
		public List<string> listBreadCrumb = new List<string>(); // split the path into a list of strings
		public List<ArptBreadCrumb> listBreadCrumbObjs = new List<ArptBreadCrumb>(); // list of bread crumb objects

		[Header("GameObjects & Prefabs")]
		public HorizontalLayoutGroup panelBreadCrumbParent; // parent of the bread crumb objects
		public GameObject prefabBreadCrumb; // prefab of the bread crumb
		public GameObject prefabArrow; // prefab of the arrow (between two bread crumbs)

		[Header("UI")]
		public Button btnReopenScript; // button to reopen the script

		protected override void SingletonAwake()
		{
			AllowRepeatInit = true;
		}

		public void NavigateTo(string relativePath)
		{
			if (ArptProjectManager.Instance.CurrentScript != null && relativePath != ArptProjectManager.Instance.CurrentScriptRelativePath)
			{
				// 当前脚本处于打开状态的情况下切换至Directory Browser(保留脚本的打开状态)
				var r = MsgBoxDialog.Show(I.S["compose.script.leaveOpen"].value, I.S["compose.script.leaveOpen.title"].value, MsgBoxType.YesNo);
				if (r == MsgBoxResult.No) return;
				btnReopenScript.interactable = true;
			}
			GenerateBreadCrumbList(relativePath);
			CurrentFullPath = Path.Combine(ArptProjectManager.Instance.CurrentProjectFolder, relativePath);
		}

		private void GenerateBreadCrumbList(string path)
		{
			listBreadCrumb.Clear();
			listBreadCrumbObjs.Clear();
			panelBreadCrumbParent.transform.DestroyAllChildren();
			string[] pathParts = path.Split('/');
			foreach (var part in pathParts)
			{
				listBreadCrumb.Add(part);
			}
			for (int i = 0; i < listBreadCrumb.Count; i++)
			{
				string partName = listBreadCrumb[i];
				var crumbObj = Instantiate(prefabBreadCrumb, panelBreadCrumbParent.transform);
				var crumb = crumbObj.GetComponent<ArptBreadCrumb>();
				crumb.labelBreadCrumb.text = partName;
				crumb.CrumbInit(parent: this, relativePath: string.Join("/", listBreadCrumb.GetRange(0, i + 1).ToArray()));
				crumb.GetComponent<TextSizeFitter>().Adjust();

				Instantiate(prefabArrow, panelBreadCrumbParent.transform);

				listBreadCrumbObjs.Add(crumb);
			}
		}
	}
}
