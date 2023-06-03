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

namespace Arcript.FormatExt.Siglus.Utility
{
	public class ProxyStream : Stream
	{
		internal Stream stream;
		internal bool shouldDispose;
		internal bool proxyDisposed = false;

		public ProxyStream(Stream input, bool leaveOpen = false)
		{
			stream = input;
			shouldDispose = !leaveOpen;
		}

		public Stream BaseStream { get { return stream; } }

		public override bool CanRead => stream.CanRead;
		public override bool CanSeek => stream.CanSeek;
		public override bool CanWrite => stream.CanWrite;
		public override long Length => stream.Length;
		public override long Position
		{
			get => stream.Position;
			set => stream.Position = value;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return stream.Read(buffer, offset, count);
		}

		public override void Flush()
		{
			stream.Flush();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return stream.Seek(offset, origin);
		}

		public override void SetLength(long length)
		{
			stream.SetLength(length);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			stream.Write(buffer, offset, count);
		}

		protected override void Dispose(bool disposing)
		{
			if (!proxyDisposed)
			{
				if (shouldDispose && disposing)
				{
					stream.Dispose();
				}
				proxyDisposed = true;
				base.Dispose(disposing);
			}
		}
	}

	public class InputProxyStream : ProxyStream
	{
		public InputProxyStream(Stream input, bool leaveOpen = false) : base(input, leaveOpen) { }

		public override bool CanWrite => false;

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException("Stream.Write method is not supported");
		}

		public override void SetLength(long length)
		{
			throw new NotSupportedException("Stream.SetLength method is not supported");
		}
	}
}
