#pragma warning disable CS0649
using AOT;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Web.UI;

namespace System.Enhance.Win32
{
	public static class Win32APIHelper
	{
		private readonly static Encoding GBK = Encoding.GetEncoding(936);

		public enum ShowCommands : int
		{
			SW_HIDE = 0,
			SW_SHOWNORMAL = 1,
			SW_NORMAL = 1,
			SW_SHOWMINIMIZED = 2,
			SW_SHOWMAXIMIZED = 3,
			SW_MAXIMIZE = 3,
			SW_SHOWNOACTIVATE = 4,
			SW_SHOW = 5,
			SW_MINIMIZE = 6,
			SW_SHOWMINNOACTIVE = 7,
			SW_SHOWNA = 8,
			SW_RESTORE = 9,
			SW_SHOWDEFAULT = 10,
			SW_FORCEMINIMIZE = 11,
			SW_MAX = 11
		}

		public enum MsgBoxFlags
		{
			OKOnly = 0,
			OKCancel = 1,
			AbortRetryIgnore = 2,
			YesNoCancel = 3,
			YesNo = 4,
			RetryCancel = 5,
			CancelTryContinue = 6,
			HandIcon = 16,
			QuestionIcon = 32,
			ExclamationIcon = 48,
			AsteriskIcon = 64,
			UserIcon = 128,
			WarningIcon = ExclamationIcon,
			ErrorIcon = HandIcon,
			InformationIcon = AsteriskIcon,
			StopIcon = HandIcon
		}

		public enum MsgBoxResults
		{
			OK = 1,
			Cancel = 2,
			Abort = 3,
			Retry = 4,
			Ignore = 5,
			Yes = 6,
			No = 7,
			Unexpected = 0
		};

		[StructLayout(LayoutKind.Sequential)]
		private struct ProcessEntry32
		{
			public uint dwSize;
			public uint cntUsage;
			public uint th32ProcessID;
			public IntPtr th32DefaultHeapID;
			public uint th32ModuleID;
			public uint cntThreads;
			public uint th32ParentProcessID;
			public int pcPriClassBase;
			public uint dwFlags;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szExeFile;
		}

		[DllImport("user32.dll")]
		private static extern int GetClientRect(IntPtr hwnd, out Rect lpRect);

		[DllImport("user32.dll")]
		private static extern int GetWindowRect(IntPtr hwnd, out Rect lpRect);

		/// <summary>
		/// 一个窗口区域的结构体
		/// </summary>
		private struct Rect
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}

		private delegate bool WNDENUMPROC(IntPtr hwnd, uint lParam);

		[DllImport("user32.dll")]
		private static extern long GetWindowThreadProcessId(IntPtr hWnd, ref uint lpdwProcessId);

		[DllImport("KERNEL32.DLL ")]
		private static extern IntPtr CreateToolhelp32Snapshot(uint flags, uint processid);
		[DllImport("KERNEL32.DLL ")]
		private static extern int CloseHandle(IntPtr handle);
		[DllImport("KERNEL32.DLL ")]
		private static extern int Process32First(IntPtr handle, ref ProcessEntry32 pe);
		[DllImport("KERNEL32.DLL ")]
		private static extern int Process32Next(IntPtr handle, ref ProcessEntry32 pe);

		[DllImport("User32.dll", EntryPoint = "SendMessage")]
		private static extern int SendMessage(int hWnd, int Msg, int wParam, string lParam);

		[DllImport("user32", EntryPoint = "IsWindow")]
		private static extern bool IsWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		private static extern int SetParent(IntPtr hWndChild, IntPtr hWndParent);

		[DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
		private static extern IntPtr GetParent(IntPtr hWnd);

		[DllImport("user32.dll", EntryPoint = "SetWindowPos")]
		private static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter,
										int X, int Y, int cx, int cy, uint uFlags);

		[DllImport("kernel32.dll", EntryPoint = "SetLastError")]
		private static extern void SetLastError(uint dwErrCode);

		[DllImport("kernel32.dll", EntryPoint = "GetLastError")]
		public static extern uint GetLastError();

		[DllImport("user32.dll", EntryPoint = "EnumWindows", SetLastError = true)]
		private static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, uint lParam);

		[DllImport("user32.dll", EntryPoint = "FindWindow")]
		private static extern IntPtr FindWindow(string IpClassName, string IpWindowName);

		[DllImport("shell32.dll")]
		private static extern IntPtr ShellExecute(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, ShowCommands nShowCmd);

		[DllImport("user32.dll", EntryPoint = "MessageBoxW", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern int MsgBox(IntPtr hWnd, string msg, string caption, int type);
		private static Hashtable processWnd = null;

		[DllImport("kernel32.dll")]
		private static extern int GetLogicalDrives();

		static Win32APIHelper()
		{
			if (processWnd == null)
			{
				processWnd = new Hashtable();
			}
		}

		[MonoPInvokeCallback(typeof(WNDENUMPROC))]
		private static bool EnumWindowsProc(IntPtr hwnd, uint lParam)
		{
			uint uiPid = 0;
			if (GetParent(hwnd) == IntPtr.Zero)
			{
				GetWindowThreadProcessId(hwnd, ref uiPid);
				if (uiPid == lParam)    // 找到进程对应的主窗口句柄
				{
					processWnd.Add(uiPid, hwnd);   // 把句柄缓存起来
					SetLastError(0);    // 设置无错误
					return false;   // 返回 false 以终止枚举窗口
				}
			}
			return true;
		}

		public static IntPtr GetHandleByProcessName(string ProcessName)
		{
			var list = new List<ProcessEntry32>();
			IntPtr handle = CreateToolhelp32Snapshot(0x2, 0);
			IntPtr hh = IntPtr.Zero;
			if ((int)handle > 0)
			{
				ProcessEntry32 pe32 = new ProcessEntry32();
				pe32.dwSize = (uint)Marshal.SizeOf(pe32);
				int bMore = Process32First(handle, ref pe32);
				while (bMore == 1)
				{
					IntPtr temp = Marshal.AllocHGlobal((int)pe32.dwSize);
					Marshal.StructureToPtr(pe32, temp, true);
					ProcessEntry32 pe = (ProcessEntry32)Marshal.PtrToStructure(temp, typeof(ProcessEntry32));
					Marshal.FreeHGlobal(temp);
					list.Add(pe);
					if (pe.szExeFile == ProcessName)
					{
						hh = GetCurrentWindowHandle(pe.th32ProcessID);
						break;
					}
					bMore = Process32Next(handle, ref pe32);
				}
			}
			return hh;
		}

		public static IntPtr GetCurrentWindowHandle(uint proid)
		{
			IntPtr ptrWnd = IntPtr.Zero;
			uint uiPid = proid;
			object objWnd = processWnd[uiPid];
			if (objWnd != null)
			{
				ptrWnd = (IntPtr)objWnd;
				if (ptrWnd != IntPtr.Zero && IsWindow(ptrWnd))  // 从缓存中获取句柄
				{
					return ptrWnd;
				}
				else
				{
					ptrWnd = IntPtr.Zero;
				}
			}
			bool bResult = EnumWindows(new WNDENUMPROC(EnumWindowsProc), uiPid);
			// 枚举窗口返回 false 并且没有错误号时表明获取成功
			if (!bResult && Marshal.GetLastWin32Error() == 0)
			{
				objWnd = processWnd[uiPid];
				if (objWnd != null)
				{
					ptrWnd = (IntPtr)objWnd;
				}
			}
			return ptrWnd;
		}

		public static MsgBoxResults MsgBox(IntPtr handle, string msg, string title, MsgBoxFlags type)
		{
			return (MsgBoxResults)MsgBox(handle, msg, title, (int)type);
		}

		public static void ShellExecute(string path, ShowCommands showCommands, string startDirectory = @"C:\", params string[] parameters)
		{
			string parametersStr = string.Join(" ", parameters);
			ShellExecute(IntPtr.Zero, "open", path, parametersStr, startDirectory, showCommands);
		}

		public static List<string> GetAllDrives()
		{
			var drives = new List<string>();
			int drivesInt = GetLogicalDrives();
			for (int i = 0; i < 26; i++)
			{
				if ((drivesInt & (1 << i)) != 0)
				{
					char driveLetter = (char)('A' + i);
					drives.Add($@"{driveLetter}:\");
				}
			}
			int err = Marshal.GetLastWin32Error();
			if (err != 0)
			{
				throw new Win32Exception(err);
			}
			return drives;
		}
	}
}