using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Team123it.ProcessForUnity
{
	/// <summary>
	/// Provides a type of MemoryStream with compatibility for both C# and C++.
	/// </summary>
	public class MemoryStreamPlus : Stream
	{
		#region Exported C++ functions (with several **unsafe** functions)
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN // windows
		[DllImport("Team123it.ProcessForUnity.Core.Win32.dll", EntryPoint = "MSCreate")]
		private static extern IntPtr MemoryStream_Create();

		[DllImport("Team123it.ProcessForUnity.Core.Win32.dll", EntryPoint = "MSCreateWithCapacity")]
		private static extern IntPtr MemoryStream_CreateWithCapacity(int capacity);

		[DllImport("Team123it.ProcessForUnity.Core.Win32.dll", EntryPoint = "MSCreateWithFixedData")]
		private static unsafe extern IntPtr MemoryStream_Unsafe_CreateWithFixedData(byte* data, int length);

		[DllImport("Team123it.ProcessForUnity.Core.Win32.dll", EntryPoint = "MSDestroy")]
		private static extern void MemoryStream_Destroy(IntPtr handle);

		[DllImport("Team123it.ProcessForUnity.Core.Win32.dll", EntryPoint = "MSRead")]
		private static unsafe extern int MemoryStream_Unsafe_Read(IntPtr handle, byte* buffer, int pos, int count);

		[DllImport("Team123it.ProcessForUnity.Core.Win32.dll", EntryPoint = "MSWrite")]
		private static unsafe extern void MemoryStream_Unsafe_Write(IntPtr handle, byte* buffer, int pos, int count);

		[DllImport("Team123it.ProcessForUnity.Core.Win32.dll", EntryPoint = "MSSeek")]
		private static extern void MemoryStream_Seek(IntPtr handle, int pos, int originType);

		[DllImport("Team123it.ProcessForUnity.Core.Win32.dll", EntryPoint = "MSGetPos")]
		private static extern int MemoryStream_GetPos(IntPtr handle);

		[DllImport("Team123it.ProcessForUnity.Core.Win32.dll", EntryPoint = "MSGetLength")]
		private static extern long MemoryStream_GetLength(IntPtr handle); // C++层为long long

		[DllImport("Team123it.ProcessForUnity.Core.Win32.dll", EntryPoint = "MSCanRead")]
		private static extern bool MemoryStream_CanRead(IntPtr handle);

		[DllImport("Team123it.ProcessForUnity.Core.Win32.dll", EntryPoint = "MSCanWrite")]
		private static extern bool MemoryStream_CanWrite(IntPtr handle);

		[DllImport("Team123it.ProcessForUnity.Core.Win32.dll", EntryPoint = "MSClose")]
		private static extern void MemoryStream_Close(IntPtr handle);
#elif UNITY_STANDALONE_LINUX || UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX // 基于*nix的系统(linux或基于linux[安卓]或基于*nix的FreeBSD[iOS/iPadOS/macOS])
#if (UNITY_64 || UNITY_EDITOR_64) // x86_64(x64)
		[DllImport("libproc4unity64.so", EntryPoint = "MSCreate")]
		private static extern IntPtr MemoryStream_Create();

		[DllImport("libproc4unity64.so", EntryPoint = "MSCreateWithCapacity")]
		private static extern IntPtr MemoryStream_CreateWithCapacity(int capacity);

		[DllImport("libproc4unity64.so", EntryPoint = "MSCreateWithFixedData")]
		private static unsafe extern IntPtr MemoryStream_Unsafe_CreateWithFixedData(byte* data, int length);

		[DllImport("libproc4unity64.so", EntryPoint = "MSDestroy")]
		private static extern void MemoryStream_Destroy(IntPtr handle);

		[DllImport("libproc4unity64.so", EntryPoint = "MSRead")]
		private static unsafe extern int MemoryStream_Unsafe_Read(IntPtr handle, byte* buffer, int pos, int count);

		[DllImport("libproc4unity64.so", EntryPoint = "MSWrite")]
		private static unsafe extern void MemoryStream_Unsafe_Write(IntPtr handle, byte* buffer, int pos, int count);

		[DllImport("libproc4unity64.so", EntryPoint = "MSSeek")]
		private static extern void MemoryStream_Seek(IntPtr handle, int pos, int originType);

		[DllImport("libproc4unity64.so", EntryPoint = "MSGetPos")]
		private static extern int MemoryStream_GetPos(IntPtr handle);
		
		[DllImport("libproc4unity64.so", EntryPoint = "MSGetLength")]
		private static extern long MemoryStream_GetLength(IntPtr handle); // C++层为long long

		[DllImport("libproc4unity64.so", EntryPoint = "MSCanRead")]
		private static extern bool MemoryStream_CanRead(IntPtr handle);

		[DllImport("libproc4unity64.so", EntryPoint = "MSCanWrite")]
		private static extern bool MemoryStream_CanWrite(IntPtr handle);

#else
		[DllImport("libproc4unityarm64.so", EntryPoint = "MSCreate")]
		private static extern IntPtr MemoryStream_Create();

		[DllImport("libproc4unityarm64.so", EntryPoint = "MSCreateWithCapacity")]
		private static extern IntPtr MemoryStream_CreateWithCapacity(int capacity);

		[DllImport("libproc4unityarm64.so", EntryPoint = "MSCreateWithFixedData")]
		private static unsafe extern IntPtr MemoryStream_Unsafe_CreateWithFixedData(byte* data, int length);

		[DllImport("libproc4unityarm64.so", EntryPoint = "MSDestroy")]
		private static extern void MemoryStream_Destroy(IntPtr handle);

		[DllImport("libproc4unityarm64.so", EntryPoint = "MSRead")]
		private static unsafe extern int MemoryStream_Unsafe_Read(IntPtr handle, byte* buffer, int pos, int count);

		[DllImport("libproc4unityarm64.so", EntryPoint = "MSWrite")]
		private static unsafe extern void MemoryStream_Unsafe_Write(IntPtr handle, byte* buffer, int pos, int count);

		[DllImport("libproc4unityarm64.so", EntryPoint = "MSSeek")]
		private static extern void MemoryStream_Seek(IntPtr handle, int pos, int originType);

		[DllImport("libproc4unityarm64.so", EntryPoint = "MSGetPos")]
		private static extern int MemoryStream_GetPos(IntPtr handle);
		
		[DllImport("libproc4unityarm64.so", EntryPoint = "MSGetLength")]
		private static extern long MemoryStream_GetLength(IntPtr handle); // C++层为long long

		[DllImport("libproc4unityarm64.so", EntryPoint = "MSCanRead")]
		private static extern bool MemoryStream_CanRead(IntPtr handle);

		[DllImport("libproc4unityarm64.so", EntryPoint = "MSCanWrite")]
		private static extern bool MemoryStream_CanWrite(IntPtr handle);
#endif
#endif
		#endregion

		private IntPtr _handle;

		public MemoryStreamPlus()
		{
			_handle = MemoryStream_Create();
		}

		public MemoryStreamPlus(int capacity)
		{
			_handle = MemoryStream_CreateWithCapacity(capacity);
		}

		public MemoryStreamPlus(byte[] buffer)
		{
			unsafe
			{
				fixed (byte* p = buffer)
				{
					_handle = MemoryStream_Unsafe_CreateWithFixedData(p, buffer.Length);
				}
			}
		}

		~MemoryStreamPlus()
		{
			Dispose(false);
		}

		public override bool CanRead => MemoryStream_CanRead(_handle);

		public override bool CanSeek => true;

		public override bool CanWrite => MemoryStream_CanWrite(_handle);

		public override long Length => MemoryStream_GetLength(_handle);

		public override long Position
		{
			get => MemoryStream_GetPos(_handle);
			set => Seek(value, SeekOrigin.Begin);
		}

		public override void Flush()
		{
			// do nothing
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException(nameof(buffer));
			}

			if (offset < 0 || count < 0 || offset + count > buffer.Length)
			{
				throw new ArgumentOutOfRangeException();
			}

			var intPtr = Marshal.AllocHGlobal(count);
			unsafe
			{
				try
				{
					byte* ptr = (byte*)intPtr.ToPointer();
					MemoryStream_Unsafe_Read(_handle, ptr, offset, count);
					Marshal.Copy(intPtr, buffer, offset, count);

					// do something...
				}
				finally
				{
					Marshal.FreeHGlobal(intPtr);
				}
			}
			return count;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException(nameof(buffer));
			}

			if (offset < 0 || count < 0 || offset + count > buffer.Length)
			{
				throw new ArgumentOutOfRangeException();
			}

			var intPtr = Marshal.AllocHGlobal(count);
			unsafe
			{
				try
				{
					Marshal.Copy(buffer, offset, intPtr, count);
					byte* ptr = (byte*)intPtr.ToPointer();
					MemoryStream_Unsafe_Write(_handle, ptr, offset, count);
				}
				finally
				{
					Marshal.FreeHGlobal(intPtr);
				}
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			int type;
			switch (origin)
			{
				case SeekOrigin.Begin:
					type = 0;
					break;
				case SeekOrigin.Current:
					type = 1;
					break;
				case SeekOrigin.End:
					type = 2;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(origin), origin, null);
			}

			MemoryStream_Seek(_handle, (int)offset, type);
			return Position;
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		public override void Close()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public unsafe IntPtr GetPointer()
		{
			return _handle; // 使用unsafe标记防止意外调用导致的GC
		}
	}
}
