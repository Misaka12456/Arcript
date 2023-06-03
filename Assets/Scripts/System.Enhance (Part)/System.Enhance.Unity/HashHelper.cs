using System;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace System.Enhance.Unity
{
	/// <summary>
	/// (Migrated from Team123it.UnityHelper.HashHelper)
	/// </summary>
	public static class HashHelper
	{
		#region MD5
		public static string ToMD5(this string plainText)
		{
			return ToMD5(plainText, Encoding.UTF8);
		}

		public static string ToMD5(this string plainText, Encoding textEncoding)
		{
			return ToMD5(textEncoding.GetBytes(plainText));
		}

		public static string ToMD5(this byte[] data)
		{
			using var md5 = MD5.Create();
			var hash = md5.ComputeHash(data);
			return BitConverter.ToString(hash).Replace("-", "").ToLower();
		}

		public static string ToMD5(this Stream stream)
		{
			using var md5 = MD5.Create();
			var hash = md5.ComputeHash(stream);
			return BitConverter.ToString(hash).Replace("-", "").ToLower();
		}

		public static string FileToMD5(this string filePath)
		{
			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException($"File not found: {filePath}");
			}
			using var stream = File.OpenRead(filePath);
			return ToMD5(stream.ToMD5()); // use ToMD5(Stream) instead of ToMD5(byte[]) to reduce unnecessary memory usage
		}
		#endregion
	}
}