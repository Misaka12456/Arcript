using Arcript.Data;
using System;
using System.Enhance.Unity;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using System.Linq;
using System.IO;
using Arcript.Compose.Inspectors;

namespace Arcript.Compose.UI
{
	public class ArptFolderBrowserManager : Singleton<ArptFolderBrowserManager>
	{
		[Header("Global")]
		public GameObject panelFolderBrowser;

		[Header("Folder Struct Tree")]
		public TreeViewControl treeFolderStructure;
		public GameObject panelOpenProjectFirstTip;

		[Header("Folder File List")]
		public VerticalLayoutGroup layoutGroupFileList;
		public GameObject prefabFileItem;
		public ToggleGroup toggleGroupFileList;
		public Sprite texFolderIcon;
		// 未知文件, 音频文件, 视频文件, 文本文件, 压缩文件, 脚本文件(图片文件直接显示其缩略图)
		public Sprite texUnknownFileIcon,
			texAudioFileIcon, texVideoFileIcon,
			texTextFileIcon, texCompressedFileIcon,
			texScriptFileIcon;

		protected override void SingletonAwake()
		{
			AllowRepeatInit = true;
			treeFolderStructure.gameObject.SetActive(false);
			panelOpenProjectFirstTip.SetActive(true);
			LoadCustomImageFormatParsers();
		}

		public void Load()
		{
			string projBaseFolder = ArptProjectManager.Instance.CurrentProjectFolder;
			if (string.IsNullOrWhiteSpace(projBaseFolder)) return;
			panelOpenProjectFirstTip.SetActive(false);
			treeFolderStructure.gameObject.SetActive(true);
			treeFolderStructure.GenerateTreeViewByFolderStructure(basePath: projBaseFolder, rootName: "Project Root");
			treeFolderStructure.ClickItemEvent += TreeFolderStructure_OnItemClicked;
		}

		private void TreeFolderStructure_OnItemClicked(GameObject go)
		{
			var item = go.GetComponent<TreeViewItem>();
			string itemRelativePath = (string)item.Data.Tag;
			if (item.Data.IsRoot)
			{
				Debug.Log($"Project Root Folder (Absolute Path): {ArptProjectManager.Instance.CurrentProjectFolder}");
			}
			NavigateTo(itemRelativePath);
		}

		public void NavigateTo(string relativePath)
		{
			ArptBreadCrumbNavigator.Instance.NavigateTo(relativePath);
			GenerateFileList();
		}

		private void GenerateFileList()
		{
			layoutGroupFileList.transform.DestroyAllChildren();
			var files = Directory.GetFiles(ArptBreadCrumbNavigator.Instance.CurrentFullPath, searchPattern: "*.*", SearchOption.TopDirectoryOnly);
			foreach (string filePath in files)
			{
				string relativePath = filePath.Replace(ArptProjectManager.Instance.CurrentProjectFolder + Path.DirectorySeparatorChar, string.Empty);
				var go = Instantiate(prefabFileItem, layoutGroupFileList.transform);
				var item = go.GetComponent<FileItem>();
				item.ItemInit(parent: this, relativePath: relativePath, relatedGroup: toggleGroupFileList);
				item.toggleSelect.onValueChanged.AddListener((isOn) =>
				{
					if (!isOn) return;
					ArptItemInspector.Instance.SetFileInfo(item.labelFileName.text, item.fileTypeName);
				});
			}
		}

		private void LoadCustomImageFormatParsers()
		{
			// 全局查找所有继承自ICustomImageFormat接口且拥有"ImgFormatExport"特性的类
			// Arcript.Data.ImgFormatExportAttribute
			// Arcript.Data.ICustomImageFormat
			var attrType = typeof(ImgFormatExportAttribute);
			var interfaceType = typeof(ICustomImageFormat);
			var allTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes());
			var customImgFmtClasses = allTypes.Where(t => t.GetInterfaces().Contains(interfaceType) && t.GetCustomAttributes(attrType, false).Length > 0);
			foreach (var imgFmtType in customImgFmtClasses)
			{
				var attr = imgFmtType.GetCustomAttribute<ImgFormatExportAttribute>();
				string ext = attr.FindExtensionPattern.Replace("*", string.Empty).Trim().ToLower();
				// e.g. ext = ".g00"
				var inst = Activator.CreateInstance(imgFmtType) as ICustomImageFormat;
				Debug.Log($"[ArptFolderBrowserManager]Loaded Custom Image Format Plugin Class: {imgFmtType.FullName}(for {ext})");
				FileItem.CustomImageFileExt.Add((ext, attr.Priority), inst);
			}
		}
	}
}
