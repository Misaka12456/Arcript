#if UNITY_EDITOR
using System.Enhance.Unity;
using System.Threading;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEditor;

namespace Arcript.EditorExt
{
	public class CopyrightScreenshoter : MonoBehaviour
	{
		public static CopyrightScreenshoter S;

		[Header("Change this to call OnValidate() for instance saving")]
		public bool MyValue;

		private string copyrightFormatStr;

		private void OnValidate()
		{
			S = this;
		}

		private void CopyrightSave()
		{
			var canvas = GameObjectHelper.FindIncludesInactive("ArcaeaVNPlayer/PlayerCanvas/PanelCopyrightCanvas");
			var text = GameObjectHelper.FindIncludesInactive("ArcaeaVNPlayer/PlayerCanvas/PanelCopyrightCanvas/PanelCprt/LabelCprt").
				GetComponent<Text>();
			canvas.SetActive(true);
			copyrightFormatStr = text.text;
			// set text to post-formatted (args are the real value instead of {0}, {1}, ...)
			text.text = string.Format(text.text, DateTime.Now.ToString("yyyy-M-d H:mm:ss"));
			Thread.Sleep(1); // wait for end of frame
		}

		private void CopyrightHide()
		{
			var canvas = GameObjectHelper.FindIncludesInactive("ArcaeaVNPlayer/PlayerCanvas/PanelCopyrightCanvas");
			var text = GameObjectHelper.FindIncludesInactive("ArcaeaVNPlayer/PlayerCanvas/PanelCopyrightCanvas/PanelCprt/LabelCprt").
				GetComponent<Text>();
			text.text = copyrightFormatStr; // restore text to pre-formatted (args are like {0}, {1}, ...)
			canvas.SetActive(false);
			Thread.Sleep(1);
		}

		public void ScreenshotCaptureInternal()
		{
			CopyrightSave();
			var t = ScreenCapture.CaptureScreenshotAsTexture();
			byte[] pngData = t.EncodeToPNG();
			CopyrightHide();
			
			if (!Directory.Exists("Debugging/Screenshots"))
			{
				Directory.CreateDirectory("Debugging/Screenshots");
			}
			//File.WriteAllBytes($"Debugging/Screenshots/{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png", pngData);
			string savePath = EditorUtility.SaveFilePanel("Save Screenshot", "", DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"), "png");
			if (!string.IsNullOrWhiteSpace(savePath))
			{
				File.WriteAllBytes(savePath, pngData);
				Debug.Log($"Screenshot saved to {savePath}");
			}
		}

		public void GameplayCameraCaptureInternal()
		{
			CopyrightSave();
			var c = GameObject.Find("GameplayCamera").GetComponent<Camera>();
			var rect = c.rect; // viewport rect
			c.rect = new Rect(0, 0, 1, 1); // fullscreen
			var fsSign = GameObjectHelper.FindIncludesInactive("ArcaeaVNPlayer/PlayerCanvas/PanelEditorOverlayCanvas/PanelFullScreen");
			fsSign.SetActive(true); // show fullscreen sign
			var t = c.targetTexture;
			c.targetTexture = new RenderTexture(1280, 720, 24);
			c.Render(); // render to texture
			RenderTexture.active = c.targetTexture;
			var tex = new Texture2D(1280, 720, TextureFormat.RGB24, false);
			tex.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
			RenderTexture.active = null;
			c.targetTexture = null;
			c.rect = rect; // restore viewport rect
			c.Render(); // reset render to screen
			fsSign.SetActive(false); // hide fullscreen sign
			byte[] pngData = tex.EncodeToPNG();
			DestroyImmediate(tex);
			CopyrightHide();
			
			if (!Directory.Exists("Debugging/Screenshots"))
			{
				Directory.CreateDirectory("Debugging/Screenshots");
			}

			string savePath = EditorUtility.SaveFilePanel("Save Camerashot (Camera Screenshot)", "", DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"), "png");
			if (!string.IsNullOrWhiteSpace(savePath))
			{
				File.WriteAllBytes(savePath, pngData);
				Debug.Log($"Camerashot saved to {savePath}");
			}
		}
	}
}
#endif