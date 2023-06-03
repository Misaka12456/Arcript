using UnityEngine;

namespace System.Enhance.Unity
{
	public enum RawPixelDataOrder
	{
		Rgba = 0,
		Bgra = 1
	}

	public enum RotateDirection
	{
		/// <summary>
		/// 顺时针(Clockwise)
		/// </summary>
		Clockwise = 0,
		/// <summary>
		/// 逆时针(CounterClockwise)
		/// </summary>
		CounterClockwise = 1
	}

	public enum RotateType
	{
		Angle90 = 0,
		Angle270 = 1
	}

	public static class Texture2DHelper
	{
		public static void LoadRawPixelData(this Texture2D t2d, byte[] rawData, RawPixelDataOrder order = RawPixelDataOrder.Rgba)
		{
			if (rawData.Length < (t2d.width * t2d.height * 4))
			{
				throw new ArgumentException("The length of rawData is not enough to fill the texture.");
			}
			var colors = new Color32[t2d.width * t2d.height];
			for (int row = 0; row < t2d.height; row++)
			{
				for (int col = 0; col < t2d.width; col++)
				{
					int index = (row * t2d.width + col) * 4;
					if (order == RawPixelDataOrder.Rgba) // rgba
					{
						colors[row * t2d.width + col] = new Color32(rawData[index + 0], rawData[index + 1], rawData[index + 2], rawData[index + 3]);
					}
					else // bgra
					{
						colors[row * t2d.width + col] = new Color32(rawData[index + 2], rawData[index + 1], rawData[index + 0], rawData[index + 3]);
					}
				}
			}
			t2d.SetPixels32(colors);
			t2d.Apply();
		}

		public static Texture2D CreateFromRawPixelData(byte[] rawData, int width = -1, int height = -1, RawPixelDataOrder order = RawPixelDataOrder.Rgba)
		{
			if (width == -1 && height == -1)
			{
				// auto detect width height by rawData length
				if (rawData.Length % 4 != 0)
				{
					Debug.LogError("The length of rawData is not a multiple of 4.");
					return null;
				}
				Debug.LogWarning("Auto detect width & height by rawData length (as 1:1 square), this may cause image distortion.\n" +
								 "Please specify width & height to avoid this warning.");
				width = (int)Mathf.Sqrt(rawData.Length / 4);
				height = width;
			}
			if (rawData.Length < (width * height * 4))
			{
				Debug.LogError("The length of rawData is not enough to fill the texture.");
				return null;
			}
			var t2d = new Texture2D(width, height, TextureFormat.RGBA32, false);
			t2d.LoadRawPixelData(rawData, order);
			return t2d;
		}

		public static Texture2D Rotate180(this Texture2D t2d)
		{
			Texture2D rotatedTex = new Texture2D(t2d.width, t2d.height, t2d.format, false);

			for (int y = 0; y < t2d.height; y++)
			{
				for (int x = 0; x < t2d.width; x++)
				{
					rotatedTex.SetPixel(x, y, t2d.GetPixel(t2d.width - x - 1, t2d.height - y - 1));
				}
			}
			rotatedTex.Apply();
			rotatedTex.name = t2d.name;
			return rotatedTex;
		}
		public static byte[] TransformToUnityCoordinates(this byte[] leftTopBasedCoorColors, int width, int height)
		{
			byte[] transformedColors = new byte[leftTopBasedCoorColors.Length];
			int channelCount = leftTopBasedCoorColors.Length / (width * height); // 每个像素的通道数

			for (int h = 0; h < height; h++) // h = height
			{
				for (int w = 0; w < width; w++) // w = width
				{
					for (int c = 0; c < channelCount; c++) // c = (color) channel
					{
						int currentIndex = (h * width + w) * channelCount + c; // 当前待转换颜色数组的下标
						int targetIndex = ((height - 1 - h) * width + w) * channelCount + c; // 目标颜色数组的下标
						transformedColors[targetIndex] = leftTopBasedCoorColors[currentIndex]; // 赋值
					}
				}
			}

			return transformedColors;
		}
		public static void TransfromLeftTopBasedToUnityCoordinates(this Texture2D t2d)
		{
			byte[] rawColors = t2d.GetRawTextureData(); // 获取原始颜色数组
			int width = t2d.width;
			int height = t2d.height;
			byte[] transformedColors = rawColors.TransformToUnityCoordinates(width, height); // 进行坐标转换

			t2d.LoadRawTextureData(transformedColors); // 加载转换后的颜色数组
			t2d.Apply(); // 应用修改
		}
	}
}
