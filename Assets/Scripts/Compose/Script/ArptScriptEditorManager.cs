using Arcript.Aspt;
using Arcript.Aspt.RawArcVNScripts;
using Arcript.Compose.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Enhance;
using System.Enhance.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Arcript.Compose
{
	public class ArptScriptEditorManager : Singleton<ArptScriptEditorManager>
	{
		private const int CmdLimitPerPage = 500;

		[Header("Global")]
		public GameObject panelScriptEditor;

		[Header("General")]
		public ToggleGroup tGroupCmds;
		public RectTransform panelCmdListParent;
		public ScrollRect scrollCmdList;
		public Button btnPrevPage, btnNextPage, btnFirstPage, btnLastPage;

		[Header("Resources")]
		public GameObject prefabCmdItem;

		private LinkedList<AsptCmdBase> ScriptCmds = new LinkedList<AsptCmdBase>();

		[HideInInspector] public int CurrentPage = 0;

		protected override void SingletonAwake()
		{
			AllowRepeatInit = true;
		}

		public void LoadScript(ArcriptScript script, int lastCmd = -1)
		{
			if (ArptFolderBrowserManager.Instance.panelFolderBrowser.activeSelf)
			{
				ArptFolderBrowserManager.Instance.panelFolderBrowser.SetActive(false);
			}
			panelScriptEditor.SetActive(true);
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
	}
}
