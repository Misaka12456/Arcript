#if UNITY_EDITOR
using DG.Tweening;
using System.Enhance.Unity;
using UnityEditor;
using UnityEngine;

namespace Arcript.EditorExt
{
	public static class CopyrightScreenshoterExt
	{

		[MenuItem("Arcript/Editors/Capture Screenshot (with copyright watermark)")]
		public static void CaptureScreenshot()
		{
			if (CopyrightScreenshoter.S == null)
			{
				Debug.LogError("Please change MyValue to update current instance");
			}
			else
			{
				CopyrightScreenshoter.S.ScreenshotCaptureInternal();
			}
		}

		[MenuItem("Arcript/Editors/Capture Gameplay Camerashot (with copyright watermark)")]
		public static void CaptureGameplayCamerashot()
		{
			if (CopyrightScreenshoter.S == null)
			{
				Debug.LogError("Please change MyValue to update current instance");
			}
			else
			{
				CopyrightScreenshoter.S.GameplayCameraCaptureInternal();
			}
		}
	}

	public static class PreviewWindowExt
	{
		private static Rect _previewRect = new Rect(0.1658f, 0.271f, 0.6684f, 0.6703f); // 通过编辑器中实测得到的值

		[MenuItem("Arcript/Editors/Preview Window/Enter Fullscreen Mode")]
		public static void SwitchToFullScreenPreviewWindow()
		{
			var camera = GameObjectHelper.FindIncludesInactive("GameplayCamera").GetComponent<Camera>();
			var editors = GameObjectHelper.FindIncludesInactive("ArcriptEditor/EditorCanvas");
			var fsSign = GameObjectHelper.FindIncludesInactive("ArcaeaVNPlayer/PlayerCanvas/PanelEditorOverlayCanvas/PanelFullScreen");
			if (fsSign.activeSelf)
			{
				Debug.LogWarning("Already in fullscreen mode");
				return;
			}
			_previewRect = camera.rect;
			camera.rect = new Rect(0, 0, 1, 1);
			fsSign.SetActive(true);
			editors.SetActive(false);
		}

		[MenuItem("Arcript/Editors/Preview Window/Quit Fullscreen Mode (Back to Editor Mode)")]
		public static void SwitchToNormalPreviewWindow()
		{
			var camera = GameObjectHelper.FindIncludesInactive("GameplayCamera").GetComponent<Camera>();
			var editors = GameObjectHelper.FindIncludesInactive("ArcriptEditor/EditorCanvas");
			var fsSign = GameObjectHelper.FindIncludesInactive("ArcaeaVNPlayer/PlayerCanvas/PanelEditorOverlayCanvas/PanelFullScreen");
			if (!fsSign.activeSelf)
			{
				Debug.LogWarning("Already in editor mode");
				return;
			}
			camera.rect = _previewRect;
			fsSign.SetActive(false);
			editors.SetActive(true);
		}
	}
}
#endif