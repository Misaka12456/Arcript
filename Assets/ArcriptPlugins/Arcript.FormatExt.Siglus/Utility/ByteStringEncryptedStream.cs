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
using System.IO;

namespace Arcript.FormatExt.Siglus.Utility
{
	/// <summary>
	/// A reader for xor encrypted binary stream data.
	/// </summary>
	public class ByteStringEncryptedStream : InputProxyStream
	{
		byte[] key;
		int basePos;

		public ByteStringEncryptedStream(Stream main, byte[] key, bool leaveOpen = false) : this(main, 0, key, leaveOpen)
		{
			
		}

		public ByteStringEncryptedStream(Stream main, long startPos, byte[] key, bool leaveOpen = false) : base(main, leaveOpen)
		{
			this.key = key;
			basePos = (int)(startPos % key.Length);
			BaseStream.Position = startPos;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int startPos = (int)((basePos + BaseStream.Position) % key.Length);
			int read = BaseStream.Read(buffer, offset, count);
			for (int i = 0; i < read; i++)
			{
				buffer[offset + i] ^= key[(startPos + i) % key.Length]; // xor
			}
			return read;
		}

		public override int ReadByte()
		{
			long pos = BaseStream.Position;
			int b = BaseStream.ReadByte();
			if (b != -1)
			{
				b ^= key[(basePos + pos) % key.Length]; // xor
			}
			return b;
		}
	}
}
