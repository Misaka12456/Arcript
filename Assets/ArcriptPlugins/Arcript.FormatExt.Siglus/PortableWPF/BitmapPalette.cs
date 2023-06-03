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
using Color = UnityEngine.Color32; // replace 'System.Drawing.Color'

namespace Arcript.FormatExt.Siglus.PortableWPF
{
	
	public class BitmapPalette
	{
		public BitmapPalette(Color[] colors)
		{
			Colors = colors;
		}

		public Color[] Colors { get; }
	}
}
