using Arcript.Utility.Loader;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPText = TMPro.TextMeshProUGUI;
using System.Collections.Generic;
using Arcript.Data;
using Arcript.I18n;
using System.Enhance.Win32;
using Cysharp.Threading.Tasks;
using UnityEngine.EventSystems;

namespace Arcript.Compose.UI
{
	public class FileItem : MonoBehaviour, IPointerClickHandler
	{
		// key = (扩展名, 同扩展名的优先级)
		// value = 通过反射创建的对应的格式解析器 (ICustomImageFormat) 的实例
		public static Dictionary<(string, int), ICustomImageFormat> CustomImageFileExt = new Dictionary<(string, int), ICustomImageFormat>();
		public readonly static string[] ImageFileExt = new string[]
		{
			".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tiff",
		};
		private readonly static string[] AudioFileExt = new string[] { ".mp3", ".wav", ".ogg", ".flac" };
		private readonly static string[] VideoFileExt = new string[] { ".mp4", ".avi", ".mkv", ".mov", ".wmv" };
		private readonly static string[] TextFileExt = new string[] { ".txt", ".md", ".json", ".xml", ".csv" };
		private readonly static string[] CompressedFileExt = new string[]
		{
			".zip", ".rar", ".7z", ".tar", ".gz", ".xz",
		};
		private readonly static string[] ScriptFileExt = new string[] { ".aspt" };
		// arst = [Ar]cript [S]crip[t] or [Ar]cript [S]cript [T]ext

		[Header("General")]
		public Image imgFileIcon;
		public TMPText labelFileName;
		public Toggle toggleSelect;

		private ArptFolderBrowserManager parentManager;
		[HideInInspector] public string relativePath;
		[HideInInspector] public string fileTypeName;

		public void ItemInit(ArptFolderBrowserManager parent, string relativePath, ToggleGroup relatedGroup)
		{
			string absPath = Path.Combine(ArptProjectManager.Instance.CurrentProjectFolder, relativePath);
			this.relativePath = relativePath;
			if (!File.Exists(absPath) && !Directory.Exists(absPath))
			{
				Debug.LogError($"FileItem: {absPath} not exists!");
				Destroy(gameObject);
				return;
			}
			parentManager = parent;
			bool isFolder = Directory.Exists(absPath);
			if (isFolder)
			{
				imgFileIcon.sprite = parent.texFolderIcon;
				fileTypeName = "Folder";
			}
			else
			{
				string ext = Path.GetExtension(absPath).ToLower();
				if (ImageFileExt.Contains(ext) || CustomImageFileExt.Keys.Any(keyPair => keyPair.Item1 == ext))
				{
					imgFileIcon.sprite = BuildOrGetImgThumbnail(relativePath, isCustomFormat: CustomImageFileExt.Keys.Any(keyPair => keyPair.Item1 == ext), customImgType: out string customImgType);
					if (string.IsNullOrEmpty(customImgType))
					{
						fileTypeName = I.S["compose.sideright.inspector.type.texture2d.normal"].value;
					}
					else
					{
						fileTypeName = string.Format(I.S["compose.sideright.inspector.type.texture2d.custom"].value, customImgType);
					}
				}
				else if (AudioFileExt.Contains(ext))
				{
					imgFileIcon.sprite = parentManager.texAudioFileIcon;
					fileTypeName = I.S["compose.sideright.inspector.type.audio"].value;
				}
				else if (VideoFileExt.Contains(ext))
				{
					imgFileIcon.sprite = parentManager.texVideoFileIcon;
					fileTypeName = I.S["compose.sideright.inspector.type.video"].value;
				}
				else if (TextFileExt.Contains(ext))
				{
					imgFileIcon.sprite = parentManager.texTextFileIcon;
					fileTypeName = I.S["compose.sideright.inspector.type.plaintext"].value;
				}
				else if (CompressedFileExt.Contains(ext))
				{
					imgFileIcon.sprite = parentManager.texCompressedFileIcon;
					fileTypeName = I.S["compose.sideright.inspector.type.deflate"].value;
				}
				else if (ScriptFileExt.Contains(ext))
				{
					imgFileIcon.sprite = parentManager.texScriptFileIcon;
					fileTypeName = I.S["compose.sideright.inspector.type.script"].value;
				}
				else
				{
					imgFileIcon.sprite = parentManager.texUnknownFileIcon;
					fileTypeName = I.S["compose.sideright.inspector.type.unknown"].value;
				}
			}
			string fileOrFolderName = Path.GetFileNameWithoutExtension(absPath); // 类似Unity Editor的显示方式(无后缀)
			labelFileName.text = fileOrFolderName;
			toggleSelect.group = relatedGroup;
		}

		private Sprite BuildOrGetImgThumbnail(string fileRelativePath, out string customImgType, bool forceUpdate = false, bool isCustomFormat = false)
		{
			// 缩略图路径: {Project Root}/Arcript/Temp/Thumbnails/{File Relative Path}.jpg
			// 文件名中的'/'会被替换为'_'，以避免在Windows下的文件名非法问题
			string thumbnailPath = Path.Combine(ArptProjectManager.Instance.CurrentProjectFolder, "Arcript", "Temp", "Thumbnails", $"{fileRelativePath.Replace('/', '_')}.jpg");
			if (File.Exists(thumbnailPath) && !forceUpdate)
			{
				var t2d = Loader.LoadTexture2D(thumbnailPath);
				customImgType = null;
				return Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), new Vector2(0.5f, 0.5f)); // 0.5f:0.5 (middle-center)
			}
			// 缩略图均等比缩放到最长边为256px
			Texture2D img = null;
			string fullPath = Path.Combine(ArptProjectManager.Instance.CurrentProjectFolder, fileRelativePath);
			if (!isCustomFormat)
			{
				img = Loader.LoadTexture2D(fullPath);
				customImgType = null;
			}
			else
			{
				img = LoadCustomFormatTexture2D(fullPath, out customImgType);
				if (img == null)
				{
					return parentManager.texUnknownFileIcon;
				}
			}
			
			// 这里改用RenderTexture的方式
			var midRt = new RenderTexture(img.width, img.height, 0);
			Graphics.Blit(img, midRt);
			var dst = new Texture2D(img.width, img.height, TextureFormat.RGBA32, false);
			RenderTexture.active = midRt;
			dst.ReadPixels(new Rect(0, 0, img.width, img.height), 0, 0);
			dst.Apply();
			RenderTexture.active = null;

			midRt.Release();
			Destroy(midRt);

			//dst.Resize(width, height, TextureFormat.RGBA32, false);
			//dst.Apply(true);

			Destroy(img);
			return Sprite.Create(dst, new Rect(0, 0, img.width, img.height), new Vector2(0.5f, 0.5f)); // 0.5f:0.5 (middle-center)
		}

		private Texture2D LoadCustomFormatTexture2D(string fileRelativePath, out string customImgTypeFormat)
		{
			Texture2D img = null;
			try
			{
				string ext = Path.GetExtension(fileRelativePath).ToLower();
				string fullPath = Path.Combine(ArptProjectManager.Instance.CurrentProjectFolder, fileRelativePath);
				var formatInsts = CustomImageFileExt.Where(kv => kv.Key.Item1 == ext).OrderByDescending(kv => kv.Key.Item2)
					.ToDictionary(kv => kv.Key.Item2, kv => kv.Value);
				// key = 优先级, value = 格式实例
				foreach (var inst in formatInsts)
				{
					if (inst.Value.TryLoadTexture2D(Path.Combine(ArptProjectManager.Instance.CurrentProjectFolder, fullPath), out var dynamicT2d))
					{
						img = dynamicT2d;
						Debug.Log($"Loaded custom image from {fullPath} with plugin {inst.Value.GetType().Name}");
						customImgTypeFormat = inst.Value.FormatDescription;
						return img;
					}
				}
				if (img == null)
				{
					throw new KeyNotFoundException("Cannot find any custom image format for this file");
				}
			}
			catch
			{
				customImgTypeFormat = null;
				return null;
			}
			customImgTypeFormat = null;
			return null;
		}

		private void OnDoubleClicked()
		{
			string absPath = Path.Combine(ArptProjectManager.Instance.CurrentProjectFolder, relativePath);
			bool isFolder = File.GetAttributes(absPath).HasFlag(FileAttributes.Directory);
			if (isFolder)
			{
				ArptFolderBrowserManager.Instance.NavigateTo(relativePath);
			}
			else
			{
				bool isScript = ScriptFileExt.Contains(Path.GetExtension(absPath).ToLower());
				if (isScript)
				{
					UniTask.Create(() => ArptProjectManager.Instance.LoadScriptAsync(absPath));
				}
				else
				{
#if UNITY_STANDALONE_WIN
					Win32APIHelper.ShellExecute(absPath, Win32APIHelper.ShowCommands.SW_SHOWDEFAULT | Win32APIHelper.ShowCommands.SW_NORMAL);
#endif
				}
			}
		}

		#region Double-Click Support
		private const float doubleClickTime = 0.3f;
		private float lastClickTime;
		private Vector2 lastClickPosition;
		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.clickCount == 1)
			{
				float timeSinceLastClick = Time.time - lastClickTime;
				float distanceSinceLastClick = Vector2.Distance(eventData.position, lastClickPosition);

				Debug.Log(string.Format("{0} {1} {2}", timeSinceLastClick, doubleClickTime, distanceSinceLastClick));
				if (timeSinceLastClick < doubleClickTime && distanceSinceLastClick < 10)
				{
					OnDoubleClicked();
				}

				lastClickTime = Time.time;
				lastClickPosition = eventData.position;
			}
			else if (eventData.clickCount == 2)
			{
				OnDoubleClicked();
			}
		}
		#endregion
	}
}