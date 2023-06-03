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
using System.Collections.Generic;

namespace Arcript.FormatExt.Siglus.Utility
{
	public static class LittleEndian
	{
		public static ushort ToUInt16<TArray>(TArray value, int index) where TArray : IList<byte>
		{
			return (ushort)(value[index] | value[index + 1] << 8);
		}

		public static short ToInt16<TArray>(TArray value, int index) where TArray : IList<byte>
		{
			return (short)(value[index] | value[index + 1] << 8);
		}

		public static uint ToUInt32<TArray>(TArray value, int index) where TArray : IList<byte>
		{
			return (uint)(value[index] | value[index + 1] << 8 | value[index + 2] << 16 | value[index + 3] << 24);
		}

		public static int ToInt32<TArray>(TArray value, int index) where TArray : IList<byte>
		{
			return (int)ToUInt32(value, index);
		}

		public static ulong ToUInt64<TArray>(TArray value, int index) where TArray : IList<byte>
		{
			return (ulong)ToUInt32(value, index) | ((ulong)ToUInt32(value, index + 4) << 32);
		}

		public static long ToInt64<TArray>(TArray value, int index) where TArray : IList<byte>
		{
			return (long)ToUInt64(value, index);
		}

		public static void Pack(ushort value, byte[] buf, int index)
		{
			buf[index] = (byte)(value);
			buf[index + 1] = (byte)(value >> 8);
		}

		public static void Pack(uint value, byte[] buf, int index)
		{
			buf[index] = (byte)(value);
			buf[index + 1] = (byte)(value >> 8);
			buf[index + 2] = (byte)(value >> 16);
			buf[index + 3] = (byte)(value >> 24);
		}

		public static void Pack(ulong value, byte[] buf, int index)
		{
			Pack((uint)value, buf, index);
			Pack((uint)(value >> 32), buf, index + 4);
		}

		public static void Pack(short value, byte[] buf, int index)
		{
			Pack((ushort)value, buf, index);
		}

		public static void Pack(int value, byte[] buf, int index)
		{
			Pack((uint)value, buf, index);
		}

		public static void Pack(long value, byte[] buf, int index)
		{
			Pack((ulong)value, buf, index);
		}
	}
}