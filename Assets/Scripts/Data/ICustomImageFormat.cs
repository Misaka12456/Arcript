using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Arcript.Data
{
	public interface ICustomImageFormat
	{
		public string FormatName { get; }

		public string FormatDescription { get; }
		public string FormatExtension { get; }

		public Texture2D LoadTexture2D(string absolutePath, bool noOutput = false);

		public Sprite LoadSprite(string absolutePath, bool noOutput = false);

		public bool TryLoadTexture2D(string absolutePath, out Texture2D t2d);

		public bool TryLoadSprite(string absolutePath, out Sprite sprite);
	}
}
