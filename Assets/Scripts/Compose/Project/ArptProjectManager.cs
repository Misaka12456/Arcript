using Arcript.Aspt;
using Arcript.Aspt.RawArcVNScripts;
using Arcript.Compose;
using Arcript.Compose.UI;
using Arcript.Data;
using Arcript.I18n;
using Arcript.Utility;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SFB;
using System;
using System.Enhance.Unity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace Arcript
{
	public class ArptProjectManager : Singleton<ArptProjectManager>
	{
		private static readonly string[] AutoCreateFolders = new string[]
		{
			"Resources", "Scripts", "Build"
		};

		private static readonly IYamlTypeConverter[] Converters = new IYamlTypeConverter[]
		{
			new ColorYamlConverter(), new DescStr2EnumConverter<AsptCmdType>(), new DescStr2EnumConverter<CurveType>(),
			new Vector2YamlConverter(), DictionaryYamlConverter<object>.String
		};
		
		[HideInInspector] public ArcriptProject CurrentProject;
		[HideInInspector] public ArcriptScript CurrentScript;
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
						await LoadScript(CurrentProject.LastOpenScript);
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

		public async UniTask LoadScript(string scriptPath)
		{
			var builder = new DeserializerBuilder();
			foreach (var c in Converters)
			{
				builder.WithTypeConverter(c);
			}
			var reader = builder.Build();
			FileStream fs = null;
			StreamReader sr = null;
			try
			{
				sr = new StreamReader(new MemoryStream(), Encoding.UTF8); // 默认即leaveOpen = false
				fs = File.OpenRead(Path.Combine(CurrentProjectFolder, CurrentProject.LastOpenScript));
				await fs.CopyToAsync(sr.BaseStream);
				sr.BaseStream.Seek(0, SeekOrigin.Begin);
				CurrentScript = reader.Deserialize<ArcriptScript>(sr);
				CurrentScriptRelativePath = CurrentProject.LastOpenScript;
				string scriptName = Path.GetFileNameWithoutExtension(Path.Combine(CurrentProjectFolder, CurrentProject.LastOpenScript));
				ArcriptComposeManager.Instance.SetTitle(projName: CurrentProject.ProjectName, scriptName: scriptName);
			}
			catch (Exception ex)
			{
				Debug.LogError($"An error occurred when loading Arcript Script '{scriptPath}':\n{ex}");
				CurrentScript = null;
				CurrentScriptRelativePath = string.Empty;
				ArcriptComposeManager.Instance.SetTitle(projName: CurrentProject.ProjectName, scriptName: string.Empty);
			}
			finally
			{
				fs?.Dispose();
				sr?.Dispose();
			}
		}

		public async UniTask SaveScript(string scriptPath, ArcriptScript script)
		{
			var builder = new SerializerBuilder();
			foreach (var c in Converters)
			{
				builder.WithTypeConverter(c);
			}
			var writer = builder.Build();
			FileStream fs = null;
			StreamWriter sw = null;
			try
			{
				fs = File.OpenWrite(Path.Combine(CurrentProjectFolder, scriptPath));
				sw = new StreamWriter(fs, Encoding.UTF8);
				await sw.WriteAsync(writer.Serialize(script));
				CurrentScriptRelativePath = scriptPath;
				string scriptName = Path.GetFileNameWithoutExtension(Path.Combine(CurrentProjectFolder, scriptPath));
				ArcriptComposeManager.Instance.SetTitle(projName: CurrentProject.ProjectName, scriptName: scriptName);
			}
			catch (Exception ex)
			{
				Debug.LogError($"An error occurred when saving Arcript Script '{scriptPath}':\n{ex}");
			}
			finally
			{
				sw?.Dispose();
				fs?.Dispose();
			}
		}
	}
}
