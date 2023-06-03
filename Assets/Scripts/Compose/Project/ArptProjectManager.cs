using Arcript.Aspt.RawArcVNScripts;
using Arcript.Compose;
using Arcript.Compose.UI;
using Arcript.Data;
using Arcript.I18n;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SFB;
using System;
using System.Enhance.Unity;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Arcript
{
	public class ArptProjectManager : Singleton<ArptProjectManager>
	{
		private static readonly string[] AutoCreateFolders = new string[]
		{
			"Resources", "Scripts", "Build"
		};

		[HideInInspector] public ArcriptProject CurrentProject;
		[HideInInspector] public RawArcVNScript CurrentScript;
		[HideInInspector] public string CurrentProjectFolder;
		[HideInInspector] public string CurrentScriptRelativePath;

		protected override void SingletonAwake()
		{
			AllowRepeatInit = true;
		}

		private void LoadProject()
		{
			string path = StandaloneFileBrowser.OpenFolderPanel(I.S["compose.project.load.title"].value, string.Empty, false);
			if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path)) return;
			UniTask.Create(() => LoadProjectAsync(path));
		}

		private async UniTask LoadProjectAsync(string path)
		{
			try
			{
				await UniTask.Yield();
				if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
				{
					path = StandaloneFileBrowser.OpenFolderPanel(I.S["compose.project.load.title"].value, string.Empty, false);
					if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path)) return;
				}
				bool isNewProject = false;
				if (!Directory.Exists(Path.Combine(path, "Arcript")))
				{
					isNewProject = true;
					Directory.CreateDirectory(Path.Combine(path, "Arcript"));
				}
				if (!File.Exists(Path.Combine(path, "Arcript", "Project.arpt")))
				{
					isNewProject = true;
				}
				ArcriptProject project;
				if (isNewProject)
				{
					project = new ArcriptProject();
					File.WriteAllText(Path.Combine(path, "Arcript", "Project.arpt"), JsonConvert.SerializeObject(project), Encoding.UTF8);
					AutoCreateFolders.Where(f => !Directory.Exists(Path.Combine(path, f))).ToList().ForEach(f => Directory.CreateDirectory(Path.Combine(path, f)));
				}
				else
				{
					try
					{
						project = JsonConvert.DeserializeObject<ArcriptProject>(File.ReadAllText(Path.Combine(path, "Arcript", "Project.arpt"), Encoding.UTF8));
					}
					catch
					{
						throw;
					}
				}
				if (!string.IsNullOrEmpty(CurrentProject.LastOpenScript))
				{
					if (File.Exists(CurrentProject.LastOpenScript))
					{
						CurrentScript = JsonConvert.DeserializeObject<RawArcVNScript>(File.ReadAllText(Path.Combine(CurrentProjectFolder, CurrentProject.LastOpenScript), Encoding.UTF8));
						CurrentScriptRelativePath = CurrentProject.LastOpenScript;
						string scriptName = Path.GetFileNameWithoutExtension(Path.Combine(CurrentProjectFolder, CurrentProject.LastOpenScript));
						ArcriptComposeManager.Instance.SetTitle(projName: CurrentProject.ProjectName, scriptName: scriptName);
						return;
					}
					else
					{
						CurrentScript = null;
						Debug.LogError(string.Format(I.S["compose.script.load.notfound.last"].value, CurrentProject.LastOpenScript));
						CurrentProject.LastOpenScript = string.Empty;
						CurrentProject.LastOpenScriptLine = 0;
					}
				}
				CurrentScriptRelativePath = string.Empty;
				ArcriptComposeManager.Instance.SetTitle(projName: CurrentProject.ProjectName, scriptName: string.Empty);
			}
			finally
			{
				if (CurrentProject != null)
				{
					CurrentProjectFolder = path;
					ArptFolderBrowserManager.Instance.Load();
				}
			}
		}

		private void Update()
		{
			if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
#if UNITY_EDITOR // 为防止Ctrl+O与Unity的打开文件快捷键冲突，仅在编辑器下支持Ctrl+Shift+O打开项目
				&& (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
#endif
				&& Input.GetKeyDown(KeyCode.O))
			{
				LoadProject();
			}
		}
	}
}
