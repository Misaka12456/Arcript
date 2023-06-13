using Arcript.Data;
using Arcript.I18n;
using System.Enhance.Unity;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

namespace Arcript.Compose.Inspectors
{
	public class ArptItemInspector : Singleton<ArptItemInspector>
	{
		[Header("File Info")]
		public Text labelFileName;
		public InputField inputFileTypeName;

		[Header("VN Command Info")]
		public RectTransform panelVNCmdInspector;

		protected override void SingletonAwake()
		{
			AllowRepeatInit = true;
		}

		public void SetFileInfo(string fileName, string typeName)
		{
			labelFileName.text = fileName;
			inputFileTypeName.text = typeName;
		}

		public void SetCmdInfo(GameObject panelCmd, CmdInspectExportAttribute attr)
		{
			labelFileName.text = attr.CmdFriendlyName;
			inputFileTypeName.text = attr.CmdModelType.FullName;
			inputFileTypeName.readOnly = true;

			panelVNCmdInspector.DestroyAllChildren();
			panelCmd.transform.SetParent(panelVNCmdInspector, false);
		}

		public GameObject SetCmdInfo(string cmdInspPrefabPath, CmdInspectExportAttribute attr)
		{
			cmdInspPrefabPath = cmdInspPrefabPath.Replace("Resources/", string.Empty);
			var goPrefab = Resources.Load<GameObject>(cmdInspPrefabPath);
			if (goPrefab == null)
			{
#if UNITY_EDITOR
				Debug.LogError($"[ArptScriptCmdItem] Cannot find inspector prefab for cmd {attr.CmdModelType.FullName} at path {cmdInspPrefabPath}");
#else
				Debug.LogError($"[ArptScriptCmdItem] Cannot find inspector prefab for cmd {attr.CmdModelType.FullName}");
#endif
				throw new ArcriptRuntimeException(string.Format(I.S["compose.dialogs.error.cmdInspNotFound.prefab"].value, attr.CmdModelType.FullName));
			}

			panelVNCmdInspector.DestroyAllChildren();
			var go = Instantiate(goPrefab, panelVNCmdInspector, false);

			labelFileName.text = attr.CmdFriendlyName;
			inputFileTypeName.text = attr.CmdModelType.FullName;
			inputFileTypeName.readOnly = true;

			return go;
		}

		public GameObject SetCmdInfo(CmdInspectExportAttribute attr)
		{
			string prefabPath = attr.CmdInspectPrefabPath;
			return SetCmdInfo(prefabPath, attr);
		}
	}
}
