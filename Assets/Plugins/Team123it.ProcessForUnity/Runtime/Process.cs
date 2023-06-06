using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Team123it.ProcessForUnity
{
	/// <summary>
	/// A fast, convenient and safe way to start and control a process in Unity. <br />
	/// Supported platform:<br />
	/// - x64: Windows, Linux, macOS (Intel Silicon), Android (Partially/See <c>Android-x86 Project</c> for more information)<br />
	/// - ARM64: Linux, Android, iOS, macOS (Apple Silicon)
	/// </summary>
	public class Process
    {
		#region Exported C++ functions

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN // windows
		[DllImport("Team123it.ProcessForUnity.Core.Win32.dll")]
        private static extern IntPtr Process_Create(string command);

		[DllImport("Team123it.ProcessForUnity.Core.Win32.dll")]
		private static extern void Process_Destroy(IntPtr handle);

		[DllImport("Team123it.ProcessForUnity.Core.Win32.dll")]
		private static extern void Process_Start(IntPtr handle);

		[DllImport("Team123it.ProcessForUnity.Core.Win32.dll")]
		private static extern bool Process_IsRunning(IntPtr handle);

		[DllImport("Team123it.ProcessForUnity.Core.Win32.dll")]
		private static extern int Process_GetExitCode(IntPtr handle);

		[DllImport("Team123it.ProcessForUnity.Core.Win32.dll", CharSet = CharSet.Auto)]
		private static extern void Process_RedirectInput(IntPtr handle, Stream inputStream);

		[DllImport("Team123it.ProcessForUnity.Core.Win32.dll", CharSet = CharSet.Auto)]
		private static extern void Process_RedirectOutput(IntPtr handle, Stream outputStream);
#elif UNITY_STANDALONE_LINUX || UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX // 基于*nix的系统(linux或基于linux[安卓]或基于*nix的FreeBSD[iOS/iPadOS/macOS])
#if (UNITY_64 || UNITY_EDITOR_64) // x86_64(x64)
		[DllImport("libproc4unity64.so", CharSet = CharSet.Auto)]
		private static IntPtr Process_Create(string command);

		[DllImport("libproc4unity64.so")]
		private static void Process_Destroy(IntPtr handle);

		[DllImport("libproc4unity64.so")]
		private static void Process_Start(IntPtr handle);

		[DllImport("libproc4unity64.so")]
		private static bool Process_IsRunning(IntPtr handle);

		[DllImport("libproc4unity64.so")]
		private static int Process_GetExitCode(IntPtr handle);

		[DllImport("libproc4unity64.so", CharSet = CharSet.Auto)]
		private static void Process_RedirectInput(IntPtr handle, Stream inputStream);

		[DllImport("libproc4unity64.so", CharSet = CharSet.Auto)]
		private static void Process_RedirectOutput(IntPtr handle, Stream outputStream);
		
#else // arm64
		[DllImport("libproc4unityarm64.so", CharSet = CharSet.Auto)]
		private static IntPtr Process_Create(string command);

		[DllImport("libproc4unityarm64.so")]
		private static void Process_Destroy(IntPtr handle);

		[DllImport("libproc4unityarm64.so")]
		private static void Process_Start(IntPtr handle);

		[DllImport("libproc4unityarm64.so")]
		private static bool Process_IsRunning(IntPtr handle);

		[DllImport("libproc4unityarm64.so")]
		private static int Process_GetExitCode(IntPtr handle);

		[DllImport("libproc4unityarm64.so", CharSet = CharSet.Auto)]
		private static void Process_RedirectInput(IntPtr handle, Stream inputStream);

		[DllImport("libproc4unityarm64.so", CharSet = CharSet.Auto)]
		private static void Process_RedirectOutput(IntPtr handle, Stream outputStream);
#endif
#endif
		#endregion

		private IntPtr m_handle;

		public Process(string command)
		{
			m_handle = Process_Create(command);
		}

		~Process()
		{
			Process_Destroy(m_handle);
		}

		public void Start()
		{
			Process_Start(m_handle);
		}

		public bool IsRunning => Process_IsRunning(m_handle);

		public int GetExitCode() => Process_GetExitCode(m_handle);
		
		public void RedirectInput(Stream inputStream)
		{
			Process_RedirectInput(m_handle, inputStream);
		}

		public void RedirectOutput(Stream outputStream)
		{
			Process_RedirectOutput(m_handle, outputStream);
		}
	}
}
