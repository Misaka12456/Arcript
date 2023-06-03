using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Enhance.IO
{
	public static class DirectoryHelper
	{
		/// <summary>
		/// Copy the whole directory with complete structure.
		/// </summary>
		/// <param name="source">The source directory.</param>
		/// <param name="target">The target directory. Will be created if not exist.</param>
		/// <param name="overwrite">Should overwrite the existing files in the target directory. Default is <see langword="true"/> .</param>
		/// <exception cref="ArgumentNullException">Occurs when <paramref name="source"/> or <paramref name="target"/> is <see langword="null"/> .</exception>
		/// <exception cref="DirectoryNotFoundException">Occurs when the source directory does not exist or could not be found.</exception>
		public static void XCopy(this DirectoryInfo source, DirectoryInfo target, bool overwrite = true)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			if (target == null)
			{
				throw new ArgumentNullException(nameof(target));
			}
			if (!source.Exists)
			{
				throw new DirectoryNotFoundException($"Source directory does not exist or could not be found: {source.FullName}");
			}
			if (!target.Exists)
			{
				target.Create();
			}
			foreach (var file in source.GetFiles())
			{
				file.CopyTo(Path.Combine(target.FullName, file.Name), overwrite);
			}
			foreach (var dir in source.GetDirectories())
			{
				XCopy(dir, new DirectoryInfo(Path.Combine(target.FullName, dir.Name)), overwrite);
			}
		}

		/// <summary>
		/// Copy the whole directory with complete structure.
		/// </summary>
		/// <param name="source">The source directory.</param>
		/// <param name="target">The target directory. Will be created if not exist.</param>
		/// <param name="overwrite">Should overwrite the existing files in the target directory. Default is <see langword="true"/> .</param>
		/// <exception cref="ArgumentNullException">Occurs when <paramref name="source"/> or <paramref name="target"/> is <see langword="null"/> .</exception>
		/// <exception cref="DirectoryNotFoundException">Occurs when the source directory does not exist or could not be found.</exception>
		public static void XCopy(string source, string target, bool overwrite = true)
		{
			XCopy(new DirectoryInfo(source), new DirectoryInfo(target), overwrite);
		}
	}
}
