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
using System.Linq;

namespace Arcript.FormatExt.Siglus.Utility
{
	public static class ByteArrayExt
	{
		public static ushort ToUInt16<TArray>(this TArray arr, int index) where TArray : IList<byte>
		{
			return (ushort)(arr[index] | arr[index + 1] << 8);
		}

		public static short ToInt16<TArray>(this TArray arr, int index) where TArray : IList<byte>
		{
			return (short)(arr[index] | arr[index + 1] << 8);
		}

		public static int ToInt24<TArray>(this TArray arr, int index) where TArray : IList<byte>
		{
			return arr[index] | arr[index + 1] << 8 | arr[index + 2] << 16;
		}

		public static uint ToUInt32<TArray>(this TArray arr, int index) where TArray : IList<byte>
		{
			return (uint)(arr[index] | arr[index + 1] << 8 | arr[index + 2] << 16 | arr[index + 3] << 24);
		}

		public static int ToInt32<TArray>(this TArray arr, int index) where TArray : IList<byte>
		{
			return (int)ToUInt32(arr, index);
		}

		public static ulong ToUInt64<TArray>(this TArray arr, int index) where TArray : IList<byte>
		{
			return (ulong)ToUInt32(arr, index) | ((ulong)ToUInt32(arr, index + 4) << 32);
		}

		public static long ToInt64<TArray>(this TArray arr, int index) where TArray : IList<byte>
		{
			return (long)ToUInt64(arr, index);
		}

		public static bool AsciiEqual<TArray>(this TArray arr, int index, string str) where TArray : IList<byte>
		{
			if (arr.Count - index < str.Length)
				return false;
			for (int i = 0; i < str.Length; ++i)
				if ((char)arr[index + i] != str[i])
					return false;
			return true;
		}

		public static bool AsciiEqual<TArray>(this TArray arr, string str) where TArray : IList<byte>
		{
			return arr.AsciiEqual(0, str);
		}
	}
}
