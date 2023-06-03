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
using Arcript.Utility.Loader;
using Arcript.Data;
using Arcript.FormatExt.Siglus.PortableWPF;
using Arcript.FormatExt.Siglus.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Enhance.Unity;

namespace Arcript.FormatExt.Siglus
{
	internal class G00MetaData
	{
		public int Type;
		public float Width, Height;
		public int BPP; // Bits Per Pixel
	}

	internal class Tile
	{
		public int X, Y, Length;
		public uint Offset;
	}

	internal sealed class G00FormatParser : IDisposable
	{
		internal Stream input;
		internal byte[] output;
		internal int width, height, type;

		public byte[] Data => output;
		public PixelFormat Format { get; private set; }
		public BitmapPalette Palette { get; private set; }

		public G00FormatParser(Stream input, G00MetaData info)
		{
			width = (int)info.Width;
			height = (int)info.Height;
			type = info.Type;
			this.input = input;
		}

		public void Unpack()
		{
			input.Position = 5;
			switch (type)
			{
				case 0: UnpackV0(); break;
				case 1: UnpackV1(); break;
				default: UnpackV2(); break;
			}
		}

		/// <summary>
		/// Unpack as Bitmap(.bmp)
		/// </summary>
		private void UnpackV0()
		{
			output = LzDecompress(input, 1, 3);
			Format = PixelFormat.Bgr24; // Bgr24 (commonly used for BMP)
		}

		/// <summary>
		/// Unpack as Indexed8(.gif)
		/// </summary>
		private void UnpackV1()
		{
			output = LzDecompress(input, 2, 4);
			int colors = LittleEndian.ToUInt16(output, 0);
			int src = 2;
			var palette = new Color32[colors]; // replace 'System.Drawing.Color'
			for (int i = 0; i < colors; i++)
			{
				// replace 'Color.FromArgb(output[src+3], output[src+2], output[src+1], output[src])'
				// Color.FromArgb(a, r, g, b) => Color32(r, g, b, a)
				palette[i] = new Color32(output[src + 2], output[src + 1], output[src], output[src + 3]);
				src += 4;
			}
			Palette = new BitmapPalette(palette);
			Format = PixelFormat.Indexed8; // Indexed8 (commonly used for GIF)
			Buffer.BlockCopy(output, src, output, 0, output.Length - src);
		}

		/// <summary>
		/// Unpack as Bgra32((mostly).png)
		/// </summary>
		private void UnpackV2()
		{
			Format = PixelFormat.Bgra32; // Bgra32 (commonly used for PNG [虽然JPG和BMP也为该种颜色模式，但其均不支持透明度])
			var reader = new BinaryReader(input, Encoding.Default, leaveOpen: true);
			int tileCount = reader.ReadInt32();
			var tiles = new List<Tile>(tileCount); // 固定大小的List
			for (int i = 0; i < tileCount; i++)
			{
				var preTile = new Tile()
				{
					X = reader.ReadInt32(),
					Y = reader.ReadInt32()
				};
				tiles.Add(preTile);
				input.Seek(0x10, SeekOrigin.Current); // 0x10 == 16
			}
			using var msInput = new BinaryReader(new MemoryStream(LzDecompress(input, 2, 1)));
			if (msInput.ReadInt32() != tileCount)
			{
				throw new FormatException("Tile count mismatch.");
			}
			int dstStride = width * 4; // stride意为跨度，即每行的字节数
			output = new byte[dstStride * height];
			for (int i = 0; i < tileCount; ++i)
			{
				tiles[i].Offset = msInput.ReadUInt32();
				tiles[i].Length = msInput.ReadInt32();
			}
			var tile = tiles.First(t => t.Length != 0);

			msInput.BaseStream.Position = tile.Offset;
			int tileType = msInput.ReadUInt16();
			int count = msInput.ReadUInt16();
			if (tileType != 1)
			{
				throw new FormatException("Tile type mismatch.");
			}
			msInput.BaseStream.Seek(0x70, SeekOrigin.Current); // 0x70 == 112
			for (int i = 0; i < count; i++)
			{
				int tileX = msInput.ReadUInt16();
				int tileY = msInput.ReadUInt16();
				msInput.ReadInt16(); // skip
				int tileWidth = msInput.ReadUInt16();
				int tileHeight = msInput.ReadUInt16();
				msInput.BaseStream.Seek(0x52, SeekOrigin.Current); // 0x52 == 82

				tileX += tile.X;
				tileY += tile.Y;
				if ((tileX + tileWidth) > width || (tileY + tileHeight) > height)
				{
					throw new FormatException("Tile out of bounds.");
				}
				int dst = tileY * dstStride + tileX * 4;
				int tileStride = tileWidth * 4;
				for (int row = 0; row < tileHeight; row++)
				{
					msInput.BaseStream.Read(output, dst, tileStride);
					dst += dstStride;
				}
			}
		}

		private static byte[] LzDecompress(Stream input, int minCount, int bytesPP)
		{
			var reader = new BinaryReader(input, Encoding.Default, leaveOpen: true);
			int packedSize = reader.ReadInt32() - 8;
			int outputSize = reader.ReadInt32();
			byte[] output = new byte[outputSize];
			int dst = 0, bits = 2;
			while (dst < output.Length && packedSize > 0)
			{
				bits >>= 1;
				if (bits == 1)
				{
					bits = reader.ReadByte() | 0x100;
					packedSize--;
				}
				if ((bits & 1) != 0)
				{
					input.Read(output, dst, bytesPP);
					dst += bytesPP;
					packedSize -= bytesPP;
				}
				else
				{
					if (packedSize < 2) break;
					int offset = reader.ReadUInt16();
					packedSize -= 2;
					int count = (offset & 0xF) + minCount;
					offset >>= 4;
					offset *= bytesPP;
					count *= bytesPP;
					CopyOverlapped(output, dst - offset, dst, count);
					dst += count;
				}
			}
			reader.Close();
			return output;
		}
		
		/// <summary>
		/// Copy potentially overlapping sequence of <paramref name="count"/> bytes in array
		/// <paramref name="data"/> from <paramref name="src"/> to <paramref name="dst"/>.
		/// If destination offset resides within source region then sequence will repeat itself.  Widely used
		/// in various compression techniques.
		/// </summary>
		private static void CopyOverlapped(byte[] data, int src, int dst, int count)
		{
			if (dst > src)
			{
				while (count > 0)
				{
					int preceding = System.Math.Min(dst - src, count);
					Buffer.BlockCopy(data, src, data, dst, preceding);
					dst += preceding;
					count -= preceding;
				}
			}
			else
			{
				Buffer.BlockCopy(data, src, data, dst, count);
			}
		}

		public void Dispose()
		{
			// Do nothing (Leave blank)
		}
	}

	[ImgFormatExport("*.g00", priority: 0)] // 优先级: Siglus Engine Encrypted JPEG (.g00) -> RealLive Engine G00 Image Format (.g00)
	public class G00Format : ICustomImageFormat
	{
		public string FormatName => "G00";

		public string FormatDescription => "RealLive Engine G00 Image Format";

		public string FormatExtension => ".g00";

		public Texture2D LoadTexture2D(string absolutePath, bool noOutput = false)
		{
			using var fs = new FileStream(absolutePath, FileMode.Open, FileAccess.Read, FileShare.Read);
			using var ms = new MemoryStream();
			fs.CopyTo(ms);
			fs.Close();
			ms.Seek(0, SeekOrigin.Begin);
			var meta = Load(ms, noOutput);
			if (meta == null) return null;
			var parser = new G00FormatParser(input: ms, info: meta);
			parser.Unpack();
			if (!parser.Data.Any())
			{
				if (!noOutput)
				{
					Debug.LogError($"[Arcript.FormatExt/RealLive][G00] No image data found in {absolutePath}.");
				}
				return null;
			}
			try
			{
				bool isBmp, isGif, isPng;
				isBmp = parser.type == 0;
				isGif = parser.type == 1;
				isPng = parser.type == 2;
				if (isGif)
				{
					if (!noOutput)
					{
						Debug.LogError($"[Arcript.FormatExt/RealLive][G00][Type1(GIF)] GIF is not supported in Unity.");
					}
					return null;
				}
				else if (isBmp || isPng)
				{
					try
					{
						// parser.Data是原始RGBA或BGRA顺序的像素数据，因此这里不能直接用Loader处理
						var t2d = Texture2DHelper.CreateFromRawPixelData(rawData: parser.Data, width: (int)meta.Width, height: (int)meta.Height, order: RawPixelDataOrder.Bgra);
						if (t2d == null) throw new Exception();
						// 坐标系转换: 传入的是左上(0,0)右下(width,height)，而Unity的Texture2D是左下(0,0)右上(width,height)
						t2d.TransfromLeftTopBasedToUnityCoordinates();
						t2d.name = Path.GetFileNameWithoutExtension(absolutePath);
//#if DEBUG
//						byte[] testPng = t2d.EncodeToPNG();
//						File.WriteAllBytes($"Debugging/{t2d.name}({t2d.width}x{t2d.height})({DateTime.Now:yyyy-MM-dd_HH-mm-ss.fff}).png", testPng);
//#endif
						return t2d;
					}
					catch
					{
						return null;
					}
				}
				else
				{
					if (!noOutput)
					{
						Debug.LogError($"[Arcript.FormatExt/RealLive][G00] Unknown type: {parser.type}.");
					}
					return null;
				}
			}
			catch (Exception ex)
			{
				if (!noOutput)
				{
					Debug.LogError($"[Arcript.FormatExt/RealLive][G00] Failed to load image from {absolutePath}.\n{ex}");
				}
				return null;
			}
		}

		public Sprite LoadSprite(string absolutePath, bool noOutput = false)
		{
			var t2d = LoadTexture2D(absolutePath, noOutput);
			if (t2d == null) return null;
			return Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), new Vector2(0.5f, 0.5f));
		}

		private G00MetaData Load(Stream stream, bool noOutput)
		{
			var reader = new BinaryReader(stream, Encoding.Default, leaveOpen: true);
			int type = reader.ReadByte();
			if (type > 2)
			{
				if (!noOutput)
				{
					Debug.LogError($"[Arcript.FormatExt/RealLive][G00] Unknown type: {type}. Maybe the version of the G00 file is too new? (Try using Siglus Engine Encrypted JPEG (.g00/type = 3) instead)");
				}
				return null;
			}
			uint width = reader.ReadUInt16();
			uint height = reader.ReadUInt16();
			if (width == 0 || width > 0x8000 || 
				height == 0 || height > 0x8000)
			{
				if (!noOutput)
				{
					Debug.LogError($"[Arcript.FormatExt/RealLive][G00] Invalid image size: {width}x{height}. maybe too big or failed to read(width or height is 0)?");
				}
				return null;
			}
			if (type == 2)
			{
				int count = reader.ReadInt32();
				if (count <= 0 || count > 0x1000)
				{
					if (!noOutput)
					{
						Debug.LogError($"[Arcript.FormatExt/RealLive][G00][Type2(Png)] Invalid palette size: {count}. maybe too big or failed to read(count is 0)?");
					}
					return null;
				}
			}
			else
			{
				uint length = reader.ReadUInt32();
				if (length + 5 != reader.BaseStream.Length)
				{
					if (!noOutput)
					{
						Debug.LogError($"[Arcript.FormatExt/RealLive][G00][Type0(Bmp)/Type1(Gif)] Invalid file size: {reader.BaseStream.Length}. maybe failed to read or the file is corrupted?");
					}
					return null;
				}
			}
			bool is8BitGif = type == 1;
			return new G00MetaData()
			{
				Width = width,
				Height = height,
				BPP = is8BitGif ? 8 : 24,
				Type = type
			};
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
	}
}