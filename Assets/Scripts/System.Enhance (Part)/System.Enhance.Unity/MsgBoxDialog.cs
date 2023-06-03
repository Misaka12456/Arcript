#if UNITY_STANDALONE_WIN
using System.ComponentModel;
using System.Enhance.Win32;
#endif
using System.Runtime.InteropServices;
using UnityEngine;

namespace System.Enhance.Unity.UI
{
	public static class MsgBoxDialog
	{
#if UNITY_STANDALONE_WIN // Windows
		#region Windows
		// Windows采用Win32 API

		public enum MsgBoxType : uint
		{
			#region Buttons
			OKOnly = 0x000000,
			OKCancel = 0x000001,
			YesNo = 0x000004,
			YesNoCancel = 0x000003,
			RetryCancel = 0x000005,
			AbortRetryIgnore = 0x000002,
			#endregion
			#region Icon
			// Error
			IconStop = 0x000010,
			IconError = 0x000010,
		
			// Question
			IconQuestion = 0x000020,

			// Warning
			IconWarning = 0x000030,
			IconExclamation = 0x000030,

			// Information
			IconInformation = 0x000040,
			IconAsterisk = 0x000040,
			#endregion
		};

		public enum MsgBoxResult : int
		{
			// OK & Cancel
			OK = 1,
			Cancel = 2,

			// Abort Retry Ignore
			Abort = 3,
			Retry = 4,
			Ignore = 5,

			// Yes & No
			Yes = 6,
			No = 7
		};

		public static MsgBoxResult Show(string content, string caption = "", MsgBoxType type = MsgBoxType.OKOnly, bool blockGame = true)
		{
			if (string.IsNullOrEmpty(caption)) caption = Application.productName;
			var handle = blockGame ? WindowHelper.MyWindowHandle : IntPtr.Zero;
			uint typeInt = (uint)type;
			int r = (int)Win32APIHelper.MsgBox(handle: handle, msg: content, title: caption, type: (Win32APIHelper.MsgBoxFlags)typeInt);
			if (r == 0)
			{
				uint errorCode = Win32APIHelper.GetLastError();
				throw new Win32Exception((int)errorCode);
			}
			return (MsgBoxResult)r;
		}
		#endregion
#else
		#region NotSupported Platform(s)
		public enum MsgBoxType : uint { };

		public enum MsgBoxResult : int { };

		public static MsgBoxResult Show(string content, string caption = "", MsgBoxType type = MsgBoxType.OKOnly, bool blockGame = true)
		{
			throw new PlatformNotSupportedException("[MsgBox] Platform not supported native PopupBox(MsgBox) API function yet.");
		}
		#endregion
#endif
	}
}