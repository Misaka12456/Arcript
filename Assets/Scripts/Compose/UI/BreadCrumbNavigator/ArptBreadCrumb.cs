using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Arcript.Compose
{
	public class ArptBreadCrumb : MonoBehaviour
	{
		public string RelativePath; // relative path of the bread crumb
		public Text labelBreadCrumb; // label of the bread crumb
		public Button btnGoTo; // button to go to the path of the bread crumb

		[HideInInspector] public ArptBreadCrumbNavigator parentNavigator; // parent navigator of the bread crumb

		public void CrumbInit(ArptBreadCrumbNavigator parent, string relativePath)
		{
			string currentFolder = relativePath.Split(Path.DirectorySeparatorChar).Last();
			labelBreadCrumb.text = currentFolder;
			RelativePath = relativePath;
			btnGoTo.onClick.AddListener(() => parent.NavigateTo(relativePath));
		}
	}
}
