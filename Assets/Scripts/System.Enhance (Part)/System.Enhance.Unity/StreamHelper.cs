using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Enhance.Unity
{
	public static class StreamHelper
	{
		public static byte[] ReadToEnd(this Stream stream)
		{
			if (!stream.CanSeek)
			{
				throw new ArgumentException("Stream must be seekable", nameof(stream));
			}
			if (stream.Position == stream.Length - 1)
			{
				return Array.Empty<byte>();
			}
			byte[] buffer = new byte[stream.Length - stream.Position];
			stream.Read(buffer, 0, buffer.Length);
			return buffer;
		}

		public static async UniTask<byte[]> ReadToEndAsync(this Stream stream)
		{
			if (!stream.CanSeek)
			{
				throw new ArgumentException("Stream must be seekable", nameof(stream));
			}
			if (stream.Position == stream.Length - 1)
			{
				return Array.Empty<byte>();
			}
			byte[] buffer = new byte[stream.Length - stream.Position];
			await stream.ReadAsync(buffer, 0, buffer.Length);
			return buffer;
		}
	}
}
