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
using System.Collections.Generic;
using System.Enhance.Unity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Arcript.Compose
{
	public class ArptProjectManager : Singleton<ArptProjectManager>
	{
		private static readonly string[] AutoCreateFolders = new string[]
		{
			"Resources", "Scripts", "Build"
		};

		public static readonly IYamlTypeConverter[] Converters = new IYamlTypeConverter[]
		{
			new ColorYamlConverter(), new Vector2YamlConverter()
		};
		
		[HideInInspector] public ArcriptProject CurrentProject;
		[HideInInspector] public ArcriptScript CurrentScript;
		[HideInInspector] public string CurrentProjectFolder;
		[HideInInspector] public string CurrentScriptRelativePath;

		protected override void SingletonAwake()
		{
			AllowRepeatInit = true;
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

		#region Load & Save Project
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
						await LoadScriptAsync(CurrentProject.LastOpenScript);
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

		public async UniTask<bool> SaveProjectAsync(bool noSaveScript = false)
		{
			try
			{
				if (CurrentProject == null || string.IsNullOrWhiteSpace(CurrentProjectFolder))
				{
					Debug.LogWarning("Cannot save \"project\" when the \"project\" is totally null.");
					return false;
				}
				if (!File.Exists(Path.Combine(CurrentProjectFolder, "Arcript", "Project.arpt")))
				{
					Debug.LogError("Cannot find the corresponding Arcript Project metadata file (Project.arpt).");
					return false;
				}
				if (CurrentScript != null)
				{
					CurrentProject.LastOpenScript = CurrentScriptRelativePath;
					CurrentProject.LastOpenScriptLine = ArptScriptEditorManager.Instance.CurrentChosenCmdIdx;
					if (!noSaveScript)
					{
						await SaveScriptAsync(CurrentScriptRelativePath, CurrentScript);
					}
				}
				else
				{
					CurrentProject.LastOpenScript = null;
					CurrentProject.LastOpenScriptLine = 0;
				}

				// serialize project metadata into temporary memory stream
				using var ms = new MemoryStream();
				using var jw = new JsonTextWriter(new StreamWriter(ms, Encoding.UTF8, bufferSize: 1024, leaveOpen: true));
				var serializer = new JsonSerializer();
				serializer.Serialize(jw, CurrentProject);
				jw.Flush();
				jw.Close();

				// reset stream position
				ms.Seek(0, SeekOrigin.Begin);

				// copy temporary memory stream to Project.arpt with file stream (write or truncate)
				using var fs = new FileStream(Path.Combine(CurrentProjectFolder, "Arcript", "Project.arpt"), FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, bufferSize: 1024, useAsync: true);
				fs.SetLength(0); // truncate
				fs.Seek(0, SeekOrigin.Begin);
				await ms.CopyToAsync(fs);
				fs.Flush();

				// finish save operation
				fs.Close();
				ms.Close();

				if (!noSaveScript)
				{
					ArptToast.Instance.Show(I.S["compose.project.saved"].value);
				}
				else
				{
					ArptToast.Instance.Show(I.S["compose.project.saved.withScript"].value);
				}
				return true;
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				ArptToast.Instance.Show(I.S["compose.project.save.failed"].value);
				return false;
			}
		}
		#endregion

		#region Load & Save Script
		public async UniTask LoadScriptAsync(string scriptPath)
		{
			var builder = new DeserializerBuilder();

			var typeTags = new Dictionary<string, Type>();

			// 定义几个转换器
			var converters = new IYamlTypeConverter[] { new Vector2YamlConverter(), new ColorYamlConverter() };
			
			// 查找所有的AsptCmdBase的继承类
			var cmdTypes = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(a => a.GetTypes())
				.Where(t => t.IsSubclassOf(typeof(AsptCmdBase)) && !t.IsAbstract);

			// 提取所有继承类的名称以$"!{name}"的形式作为Yaml的Tag
			foreach (var t in cmdTypes)
			{
				string name = t.Name;
				var type = t;
				typeTags.Add($"!{name}", type);
			}

			// 注册所有的Tag
			foreach (var kv in typeTags)
			{
				builder = builder.WithTagMapping(kv.Key, kv.Value);
			}

			// 注册所有的转换器
			foreach (var c in converters)
			{
				builder = builder.WithTypeConverter(c);
			}

			// 构建Deserializer
			var reader = builder.Build();

			FileStream fs = null;
			StreamReader sr = null;
			try
			{
				CurrentProject.LastOpenScript = scriptPath;
				sr = new StreamReader(new MemoryStream(), Encoding.UTF8); // 默认即leaveOpen = false
				fs = File.OpenRead(Path.Combine(CurrentProjectFolder, CurrentProject.LastOpenScript));
				await fs.CopyToAsync(sr.BaseStream);
				sr.BaseStream.Seek(0, SeekOrigin.Begin);

				CurrentScript = reader.Deserialize<ArcriptScript>(sr);

				CurrentScriptRelativePath = CurrentProject.LastOpenScript;
				string scriptName = Path.GetFileNameWithoutExtension(Path.Combine(CurrentProjectFolder, CurrentProject.LastOpenScript));
				ArcriptComposeManager.Instance.SetTitle(projName: CurrentProject.ProjectName, scriptName: scriptName);
				ArptScriptEditorManager.Instance.LoadScript(CurrentScript);
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

		public async UniTask<bool> SaveScriptAsync(string scriptPath, ArcriptScript script)
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
				ArcriptComposeManager.Instance.SetTitle(projName: CurrentProject.ProjectName, scriptName: scriptName,
					pendingModify: false);
				return true;
			}
			catch (Exception ex)
			{
				Debug.LogError($"An error occurred when saving Arcript Script '{scriptPath}':\n{ex}");
				return false;
			}
			finally
			{
				sw?.Dispose();
				fs?.Dispose();
			}
		}
		#endregion
	}
}