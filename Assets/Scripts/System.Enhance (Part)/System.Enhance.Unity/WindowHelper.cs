using System.Runtime.InteropServices;
using UnityEngine;

namespace System.Enhance.Unity.UI
{
	public static class WindowHelper
	{
		[DllImport("user32.dll", EntryPoint = "SetWindowTextW", CharSet = CharSet.Unicode)]
		private static extern bool SetWindowTextW(IntPtr hwnd, string lpString);
		[DllImport("user32.dll", EntryPoint = "FindWindow")]
		private static extern IntPtr FindWindow(string className, string windowName);

		private static IntPtr? myWindowHandle = null;

		public static IntPtr MyWindowHandle
		{
			get
			{
				if (!myWindowHandle.HasValue)
				{
					myWindowHandle = FindWindow(null, Application.productName);
				}
				return myWindowHandle.Value;
			}
		}

		public static void SetTitle(string title)
		{
#if UNITY_STANDALONE_WIN
			if (Debug.isDebugBuild)
			{
				title += " - 内部测试版本, 严禁外传";
			}
			SetWindowTextW(MyWindowHandle, title);
#endif
		}
	}
}