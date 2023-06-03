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
using UnityEngine;

namespace Arcript.FormatExt.Siglus.PortableWPF
{
	public enum PixelFormat
	{
		Bgr101010 = 0x2B,

		[CorresTexFormat(TextureFormat.RGB24)] // bmp
		Bgr24 = 0x3,
		Bgr32 = 0x4,
		Bgr555 = 0x25,
		Bgr565 = 0x26,
		[CorresTexFormat(TextureFormat.BGRA32)] // png
		Bgra32 = 0x5,
		BlackWhite = 0x1B,
		Cmyk32 = 0x1A,
		Default = 0,
		Gray16 = 0x10,
		Gray2 = 0x1D,
		Gray32Float = 0x12,
		Gray4 = 0x1E,
		Gray8 = 0x11,
		Indexed1 = 0x1F,
		Indexed2 = 0x20,
		Indexed4 = 0x21,

		[CorresTexFormat(TextureFormat.Alpha8)] // gif (Unity *does not* support this format)
		Indexed8 = 0x22,
		Pbgra32 = 0x6,
		Prgba128Float = 0x13,
		Prgba64 = 0x14,
		Rgb128Float = 0x15,
		Rgb24 = 0x7,
		Rgb48 = 0x16,
		Rgba128Float = 0x17,
		Rgba64 = 0x18
	}

	public class CorresTexFormatAttribute : Attribute
	{
		public TextureFormat? Format { get; set; }

		public CorresTexFormatAttribute(TextureFormat format)
		{
			Format = format;
		}

		public CorresTexFormatAttribute()
		{
			Format = null;
		}
	}

	public static class PixelFormatExtension
	{
		public static TextureFormat ToTextureFormat(this PixelFormat format)
		{
			var type = typeof(PixelFormat);
			var memInfo = type.GetMember(format.ToString());
			var attributes = memInfo[0].GetCustomAttributes(typeof(CorresTexFormatAttribute), false);
			var attribute = (CorresTexFormatAttribute)attributes[0];
			return attribute.Format ?? throw new NotSupportedException("Given PixelFormat is not supported (cannot find corresponding TextureFormat).");
		}
	}
}
