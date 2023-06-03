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
using System;
using System.IO;
using System.Text;

namespace Arcript.FormatExt.Siglus.Utility
{

	/// <summary>
	/// Represents a region within existing stream.
	/// Underlying stream should allow seeking (CanSeek == true).
	/// </summary>
	public class StreamRegion : InputProxyStream
	{
		private long begin, end;

		public StreamRegion(Stream main, long offset, long length, bool leaveOpen = false) : base(main, leaveOpen)
		{
			begin = offset;
			end = Math.Min(offset + length, BaseStream.Length);
			BaseStream.Position = begin;
		}

		public StreamRegion(Stream main, long offset, bool leaveOpen = false) : this(main, offset, main.Length - offset, leaveOpen)
		{
			
		}

		public override bool CanSeek => true;
		public override long Length => end - begin;
		public override long Position
		{
			get => BaseStream.Position - begin;
			set => BaseStream.Position = Math.Max(begin + value, begin);
		}
		
		public override long Seek(long offset, SeekOrigin origin)
		{
			offset += origin switch
			{
				SeekOrigin.Begin => begin,
				SeekOrigin.Current => BaseStream.Position,
				SeekOrigin.End => end,
				_ => throw new ArgumentException("Invalid seek origin", nameof(origin)),
			};
			offset = Math.Max(offset, begin);
			BaseStream.Position = offset;
			return offset - begin;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int read = 0;
			long available = end - BaseStream.Position;
			if (available > 0)
			{
				read = BaseStream.Read(buffer, offset, (int)Math.Min(count, available));
			}
			return read;
		}

		public override int ReadByte()
		{
			if (BaseStream.Position < end)
			{
				return BaseStream.ReadByte();
			}
			else
			{
				return -1;
			}
		}
	}
}
