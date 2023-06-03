using UnityEngine;

namespace System.Enhance.Unity
{
	[RequireComponent(typeof(Camera))]
	[ExecuteInEditMode]
	public class CameraUIMapper : MonoBehaviour
	{
		public int targetWidth;
		public int targetHeight;
		public Vector2 uiBottomLeftAnchor;

		private new Camera camera;
		private Vector2 lastScreenSize;

		private void Awake()
		{
			camera = GetComponent<Camera>();
		}

		// 脚本开始时调用
		private void Start()
		{
			MapCameraToUIArea();
		}

		private void OnEnable()
		{
			lastScreenSize = new Vector2(Screen.width, Screen.height);
		}

		// Inspector数据变化时调用(Editor Only)
		private void OnValidate()
		{
			MapCameraToUIArea();
		}

		private void OnRectTransformDimensionsChange()
		{
			MapCameraToUIArea();
			lastScreenSize = new Vector2(Screen.width, Screen.height);
		}

		private void Update()
		{
			if (Screen.width != lastScreenSize.x || Screen.height != lastScreenSize.y)
			{
				MapCameraToUIArea();
				lastScreenSize = new Vector2(Screen.width, Screen.height);
			}
		}

		private void MapCameraToUIArea()
		{
			float screenWidth = Screen.width;
			float screenHeight = Screen.height;
			float screenAspect = screenWidth / screenHeight;

			float targetAspect = (float)targetWidth / targetHeight;

			float viewportWidth = targetWidth / screenWidth;
			float viewportHeight = targetHeight / screenHeight;

			float viewportX = (1 - viewportWidth) / 2f;
			float viewportY = (1 - viewportHeight) / 2f;

			if (screenAspect > targetAspect)
			{
				float scaleHeight = targetAspect / screenAspect;
				viewportY = (1 - scaleHeight) / 2f;
				viewportHeight = scaleHeight;
			}
			else
			{
				float scaleWidth = screenAspect / targetAspect;
				viewportX = (1 - scaleWidth) / 2f;
				viewportWidth = scaleWidth;
			}

			// Add anchor point offset to Viewport X and Y coordinates
			viewportX += uiBottomLeftAnchor.x / screenWidth;
			viewportY += uiBottomLeftAnchor.y / screenHeight;

			var rect = new Rect(viewportX, viewportY, viewportWidth, viewportHeight);
			camera.rect = rect;
		}
	}
}
