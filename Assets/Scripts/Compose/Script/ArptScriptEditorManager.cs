using Arcript.Aspt;
using Arcript.Aspt.RawArcVNScripts;
using Arcript.Compose.UI;
using Arcript.I18n;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Enhance;
using System.Enhance.Unity;
using System.Enhance.Unity.UI;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using MsgBoxType = System.Enhance.Unity.UI.MsgBoxDialog.MsgBoxType;
using MsgBoxResult = System.Enhance.Unity.UI.MsgBoxDialog.MsgBoxResult;
using System.Linq;

namespace Arcript.Compose
{
	public class ArptScriptEditorManager : Singleton<ArptScriptEditorManager>
	{
		private const int CmdLimitPerPage = 500;

		public bool IsInScriptEditing => panelScriptEditor.activeSelf;
		public bool IsNotSaved { get; private set; } = false; // Not Saved会在标题显示为$"*{title}"，Saved会在标题显示为$"{title}"
															  // 且Not Saved会在关闭脚本编辑器时提示是否保存，Saved则不会

		[Header("Global")]
		public GameObject panelScriptEditor;

		[Header("General")]
		public ToggleGroup tGroupCmds;
		public RectTransform panelCmdListParent;
		public ScrollRect scrollCmdList;
		public Button btnPrevPage, btnNextPage, btnFirstPage, btnLastPage;
		public Button btnBringBgScriptToFront; // 将处于后台的脚本编辑器(Script Editor)唤至前台

		[Header("Resources")]
		public GameObject prefabCmdItem;

		private LinkedList<AsptCmdBase> ScriptCmds = new LinkedList<AsptCmdBase>();

		[HideInInspector] public int CurrentPage = 0;
		[HideInInspector] public int CurrentChosenCmdIdx
		{
			get
			{
				int index = 0;
				// 获取当前选中的指令在panelCmdListParent中的索引(如果没选中则视为0)(指令的组均为tGroupCmds)
				var cmdItems = panelCmdListParent.GetComponentsInChildren<ArptScriptCmdItem>().Where(item => item.toggleChoosing.group == tGroupCmds);
				foreach (var item in cmdItems)
				{
					if (item.toggleChoosing.isOn)
					{
						break;
					}
					index++;
				}

				// 根据当前页数和当前选中的指令在panelCmdListParent中的索引计算出当前选中的指令在ScriptCmds中的索引
				index = CurrentPage * CmdLimitPerPage + index;
				return index;
			}
		}

		protected override void SingletonAwake()
		{
			AllowRepeatInit = true;
		}

		public void LoadScript(ArcriptScript script, int lastCmd = -1)
		{
			BackToScriptEditor();
			ScriptCmds.Clear();
			foreach (var c in script.Commands)
			{
				ScriptCmds.AddLast(c);
			}
			if (lastCmd == -1 && ScriptCmds.Count > 0)
			{
				lastCmd = 0;
			}
			LoadAndScrollToCmdPage(lastCmd);
		}

		private void LoadAndScrollToCmdPage(int lastCmdIndex)
		{
			if (lastCmdIndex < 0)
			{
				CurrentPage = 0;
			}
			else if (lastCmdIndex / CmdLimitPerPage > 0)
			{
				CurrentPage = lastCmdIndex / CmdLimitPerPage;
			}
			LoadCmds();
			scrollCmdList.Scroll(elementIdx: lastCmdIndex % CmdLimitPerPage, duration: 0.5f).SetEase(Ease.OutCubic);
		}

		private void LoadCmds()
		{
			if (CurrentPage > 0 && ScriptCmds.OutOfRange((CurrentPage - 1) * CmdLimitPerPage))
			{
				CurrentPage = ScriptCmds.Count / CmdLimitPerPage;
			}
			// 计算指定区间({CurrentPage * CmdLimitPerPage} ~ {CurrentPage * CmdLimitPerPage + CmdLimitPerPage})的指令范围（若不足则取到最后一条指令）
			int currPageIdxStart = CurrentPage * CmdLimitPerPage;

			// 在完整的C# 8.0里可以直接ScriptCmds[currPageIdxStart..(currPageIdxStart + CmdLimitPerPage)]，但你Unity 2020.2.1f1支持的是个残废的C# 8
			// 也不等什么hack了，直接等CoreCLR更新吧(Unity CoreCLR when?)
			var currPageCmds = ScriptCmds.PickRange(startIdx: currPageIdxStart, count: CmdLimitPerPage);
			GenerateCmdItemList(currPageCmds, clearFirst: true);
		}

		private void GenerateCmdItemList(IEnumerable<AsptCmdBase> cmds, bool clearFirst = false)
		{
			if (clearFirst)
			{
				panelCmdListParent.DestroyAllChildren();
			}
			foreach (var cmd in cmds)
			{
				var go = Instantiate(prefabCmdItem, panelCmdListParent);
				var item = go.GetComponent<ArptScriptCmdItem>();
				item.SetInfo(cmdShortInfo: cmd.ToItemShortString(), isBlock: cmd.IsBlock, parentGroup: tGroupCmds);
			}
		}

		public void BackToScriptEditor()
		{
			if (ArptFolderBrowserManager.Instance.panelFolderBrowser.activeSelf)
			{
				ArptFolderBrowserManager.Instance.panelFolderBrowser.SetActive(false);
			}
			panelScriptEditor.SetActive(true);
			btnBringBgScriptToFront.interactable = false;
		}

		public void HideScriptEditorToBackground()
		{
			panelScriptEditor.SetActive(false);
			ArptFolderBrowserManager.Instance.panelFolderBrowser.SetActive(true);
			btnBringBgScriptToFront.interactable = true;
		}

		public void CloseScript()
		{
			
		}

		private async UniTask CloseScriptAsync()
		{
			await UniTask.SwitchToMainThread();
			#region Not Save Warning Implementation
			if (IsNotSaved)
			{
				string template = I.S["compose.script.closeNotSave"].value;
				// {0} = Project Name
				// {1} = Relative Path (without file name)
				// {2} = File Name
				string path = ArptProjectManager.Instance.CurrentScriptRelativePath;
				string rPathWithoutFName = path.Substring(0, path.LastIndexOf(Path.DirectorySeparatorChar));
				string fileName = path.Substring(path.LastIndexOf(Path.DirectorySeparatorChar) + 1);

				string msg = string.Format(template,
					ArptProjectManager.Instance.CurrentProject.ProjectName,
					rPathWithoutFName,
					fileName);

				var dialogR = MsgBoxDialog.Show(msg, I.S["compose.script.closeNotSave.title"].value, MsgBoxType.YesNoCancel);
				if (dialogR == MsgBoxResult.Cancel)
				{
					return;
				}
				else if (dialogR == MsgBoxResult.Yes)
				{
					var script = ArptProjectManager.Instance.CurrentScript;
					script.Commands.Clear();
					foreach (var c in ScriptCmds)
					{
						script.Commands.AddLast(c);
					}
					ArptProjectManager.Instance.CurrentScript = script;
					bool r = await ArptProjectManager.Instance.SaveScriptAsync(scriptPath: ArptProjectManager.Instance.CurrentScriptRelativePath, script: script);
					if (r)
					{
						IsNotSaved = false;
					}
				}
			}
			#endregion

			// Codes & Data
			ScriptCmds.Clear();
			ArptProjectManager.Instance.CurrentScript = null;
			ArptProjectManager.Instance.CurrentScriptRelativePath = null;
			ArptProjectManager.Instance.CurrentProject.LastOpenScript = null;
			ArptProjectManager.Instance.CurrentProject.LastOpenScriptLine = 0;

			// UI
			panelCmdListParent.DestroyAllChildren();
			ArcriptComposeManager.Instance.SetTitle(projName: ArptProjectManager.Instance.CurrentProject.ProjectName);
			panelScriptEditor.SetActive(false);
			btnBringBgScriptToFront.interactable = false;
			ArptFolderBrowserManager.Instance.panelFolderBrowser.SetActive(true);


		}
	}
}
