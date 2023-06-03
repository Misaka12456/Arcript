// RealLive Engine Format & Siglus Engine Format Extensions for Arcript
// (C)Copyright 2018-2023 Misaka Castle Group. All rights reserved.
//
// Engine Technology are from VisualArt's/Key (https://key.visualarts.gr.jp/)
// (C)Copyright 2000 VisualArt's RealLive Engine
// (C)Copyright 2016 VisualArt's Siglus Engine
//
// Partial Code Logic and Data Structures are from GARbro project (https://github.com/morkt/GARbro)
// Copyright (C) 2016 by morkt
// Licensed under The MIT License (https://github.com/morkt/GARbro/blob/master/LICENSE).
using Arcript.Data;
using Arcript.FormatExt.Siglus.Utility;
using Arcript.Utility.Loader;
using System;
using System.Enhance.Unity;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Scripting;

namespace Arcript.FormatExt.Siglus
{
	[ImgFormatExport("*.g00", priority: 1)] // 优先级高于RealLive的G00格式(虽然都是VisualArt's开发的)
	public class G00JpegFormat : ICustomImageFormat
	{
		public string FormatName => "G00/JPEG";

		public string FormatDescription => "Siglus Engine Encrypted JPEG Image Format";

		public string FormatExtension => ".g00";

		[Preserve]
		public G00JpegFormat() { }

		public Texture2D LoadTexture2D(string absolutePath, bool noOutput = false)
		{
			using var fs = new FileStream(absolutePath, FileMode.Open, FileAccess.Read, FileShare.Read);
			using var ms = new MemoryStream();
			try
			{
				fs.CopyTo(ms);
				fs.Close();
				ms.Seek(0, SeekOrigin.Begin);
				var decryptedStream = Load(ms, out int width, out int height, noOutput); // 解密完就是标准的Jpeg文件了，因此直接交由Loader处理
				if (decryptedStream == null)
				{
					if (!noOutput)
					{
						Debug.LogError($"[Arcript.FormatExt/Siglus][G00Jpeg] Failed to load image from {absolutePath}.");
					}
					return null;
				}
				using var br = new BinaryReader(decryptedStream, Encoding.Default, leaveOpen: true);
				int type = br.ReadByte();
				try
				{
					byte[] fullData = decryptedStream.ReadToEnd();
					var t2d = Texture2DHelper.CreateFromRawPixelData(fullData, width, height, RawPixelDataOrder.Rgba);
					if (t2d == null) throw new Exception();
					// 坐标系转换: 传入的是左上(0,0)右下(width,height)，而Unity的Texture2D是左下(0,0)右上(width,height)
					t2d.TransfromLeftTopBasedToUnityCoordinates();
					return t2d;
				}
				catch
				{
					if (!noOutput)
					{
						Debug.LogError($"[Arcript.FormatExt/Siglus][G00Jpeg] Failed to load image from {absolutePath}.");
					}
					return null;
				}
			}
			finally
			{
				ms.Close();
			}
		}

		public Sprite LoadSprite(string absolutePath, bool noOutput = false)
		{
			var t2d = LoadTexture2D(absolutePath, noOutput);
			if (t2d == null) return null;
			return Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), new Vector2(0.5f, 0.5f));
		}

		private Stream Load(Stream stream, out int width, out int height, bool noOutput = false)
		{
			var reader = new BinaryReader(stream, Encoding.Default, leaveOpen: true);
			int type = reader.ReadByte();
			if (type != 3)
			{
				if (!noOutput)
				{
					Debug.LogError($"[Arcript.FormatExt/Siglus][G00Jpeg] Unknown type: {type}. Previous version of the G00 file should use 'RealLive Format' (G00Format) instead.");
				}
				width = height = 0;
				return null;
			}
			uint rWidth = reader.ReadUInt16();
			uint rHeight = reader.ReadUInt16();
			if (rWidth == 0 || rWidth > 0x8000 || rHeight == 0 || rHeight > 0x8000)
			{
				if (!noOutput)
				{
					Debug.LogError($"[Arcript.FormatExt/Siglus][G00Jpeg] Invalid image size: {rWidth}x{rHeight}. maybe too big or failed to read(width or height is 0)?");
				}
				width = height = 0;
				return null;
			}
			reader.Close();
			Stream input = new StreamRegion(stream, 5, true);
			input = new ByteStringEncryptedStream(input, DefaultKey);
			SkipMetadata(input, out int rWidth2, out int rHeight2);
			if (rWidth != rWidth2 || rHeight != rHeight2)
			{
				if (!noOutput)
				{
					Debug.LogError($"[Arcript.FormatExt/Siglus][G00Jpeg] Image size mismatch: {rWidth}x{rHeight} (in header) != {rWidth2}x{rHeight2} (in metadata).");
				}
				width = height = 0;
				return null;
			}
			width = rWidth2;
			height = rHeight2;
			return input;
		}

		private bool SkipMetadata(Stream stream, out int width, out int height)
		{
			using var br = new BinaryReader(stream, Encoding.Default, leaveOpen: true);
			if (br.ReadByte() != 0xFF || br.ReadByte() != 0xD8)
			{
				width = height = 0;
				return false;
			}
			while (br.PeekChar() != -1)
			{
				ushort marker = BigEndian(br.ReadUInt16());
				if ((marker & 0xff00) != 0xff00)
				{
					break;
				}
				int length = BigEndian(br.ReadUInt16());
				if ((marker & 0x00f0) == 0xc0 && marker != 0xffc4)
				{
					if (length < 8)
					{
						break;
					}
					int bits = br.ReadByte();
					uint rHeight = BigEndian(br.ReadUInt16());
					uint rWidth = BigEndian(br.ReadUInt16());
					int components = br.ReadByte();
					//return new ImageMetaData
					//{
					//	Width = width,
					//	Height = height,
					//	BPP = bits * components,
					//};
					width = (int)rWidth;
					height = (int)rHeight;
					return true;
				}
				br.BaseStream.Seek(length - 2, SeekOrigin.Current);
			}
			width = height = 0;
			return false;
		}

		public bool TryLoadTexture2D(string absolutePath, out Texture2D t2d)
		{
			t2d = LoadTexture2D(absolutePath, true);
			return t2d != null;
		}

		public bool TryLoadSprite(string absolutePath, out Sprite sprite)
		{
			sprite = LoadSprite(absolutePath, true);
			return sprite != null;
		}

		internal static readonly byte[] DefaultKey = {
			0x45, 0x0C, 0x85, 0xC0, 0x75, 0x14, 0xE5, 0x5D, 0x8B, 0x55, 0xEC, 0xC0, 0x5B, 0x8B, 0xC3, 0x8B,
			0x81, 0xFF, 0x00, 0x00, 0x04, 0x00, 0x85, 0xFF, 0x6A, 0x00, 0x76, 0xB0, 0x43, 0x00, 0x76, 0x49,
			0x00, 0x8B, 0x7D, 0xE8, 0x8B, 0x75, 0xA1, 0xE0, 0x0C, 0x85, 0xC0, 0xC0, 0x75, 0x78, 0x30, 0x44,
			0x00, 0x85, 0xFF, 0x76, 0x37, 0x81, 0x1D, 0xD0, 0xFF, 0x00, 0x00, 0x75, 0x44, 0x8B, 0xB0, 0x43,
			0x45, 0xF8, 0x8D, 0x55, 0xFC, 0x52, 0x00, 0x76, 0x68, 0x00, 0x00, 0x04, 0x00, 0x6A, 0x43, 0x8B,
			0xB1, 0x43, 0x00, 0x6A, 0x05, 0xFF, 0x50, 0xFF, 0xD3, 0xA1, 0xE0, 0x04, 0x00, 0x56, 0x15, 0x2C,
			0x44, 0x00, 0x85, 0xC0, 0x74, 0x09, 0xC3, 0xA1, 0x5F, 0x5E, 0x33, 0x8B, 0xE5, 0x5D, 0xE0, 0x30,
			0x04, 0x00, 0x81, 0xC6, 0x00, 0x00, 0x81, 0xEF, 0x04, 0x00, 0x85, 0x30, 0x44, 0x00, 0x00, 0x00,
			0x5D, 0xC3, 0x8B, 0x55, 0xF8, 0x8D, 0x5E, 0x5B, 0x4D, 0xFC, 0x51, 0xC4, 0x04, 0x5F, 0x8B, 0xE5,
			0x43, 0x00, 0xEB, 0xD8, 0x8B, 0x45, 0xFF, 0x15, 0xE8, 0x83, 0xC0, 0x57, 0x56, 0x52, 0x2C, 0xB1,
			0x01, 0x00, 0x8B, 0x7D, 0xE8, 0x89, 0x00, 0xE8, 0x45, 0xF4, 0x8B, 0x20, 0x50, 0x6A, 0x47, 0x28,
			0x00, 0x50, 0x53, 0xFF, 0x15, 0x34, 0xE4, 0x6A, 0xB1, 0x43, 0x00, 0x0C, 0x8B, 0x45, 0x00, 0x6A,
			0x8B, 0x4D, 0xEC, 0x89, 0x08, 0x8A, 0x85, 0xC0, 0x45, 0xF0, 0x84, 0x8B, 0x45, 0x10, 0x74, 0x05,
			0xF5, 0x28, 0x01, 0x00, 0x83, 0xC4, 0x52, 0x6A, 0x08, 0x89, 0x45, 0x83, 0xC2, 0x20, 0x00, 0xE8,
			0xE8, 0xF4, 0xFB, 0xFF, 0xFF, 0x8B, 0x8B, 0x5D, 0x45, 0x0C, 0x83, 0xC0, 0x74, 0xC5, 0xF8, 0x53,
			0xC4, 0x08, 0x85, 0xC0, 0x75, 0x56, 0x30, 0x44, 0x8B, 0x1D, 0xD0, 0xF0, 0xA1, 0xE0, 0x00, 0x83,
		};

		private static uint BigEndian(uint u)
		{
			return u << 24 | (u & 0xff00) << 8 | (u & 0xff0000) >> 8 | u >> 24;
		}
		private static int BigEndian(int i)
		{
			return (int)BigEndian((uint)i);
		}
		private static ushort BigEndian(ushort u)
		{
			return (ushort)(u << 8 | u >> 8);
		}
		private static short BigEndian(short i)
		{
			return (short)BigEndian((ushort)i);
		}
		private static ulong BigEndian(ulong u)
		{
			return (ulong)BigEndian((uint)(u & 0xffffffff)) << 32
				 | (ulong)BigEndian((uint)(u >> 32));
		}
		private static long BigEndian(long i)
		{
			return (long)BigEndian((ulong)i);
		}
	}
}
