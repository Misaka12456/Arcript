//namespace Arcript.Assets.Scripts.System.Enhance__Part_
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System.Enhance
{
	public static class IEnumerableExtension
	{
		public static bool OutOfRange(this IEnumerable enumerable, int index)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException("enumerable");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", index, "index must be non-negative");
			}
			if (enumerable is ICollection)
			{
				return index >= ((ICollection)enumerable).Count;
			}
			if (enumerable is IList)
			{
				return index >= ((IList)enumerable).Count;
			}
			var enumerator = enumerable.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (index == 0)
					{
						return false;
					}
					index--;
				}
			}
			finally
			{
				var disposable = enumerator as IDisposable;
				disposable?.Dispose();
			}
			return true;
		}

		public static IEnumerable<T> PickRange<T>(this IEnumerable<T> source, int startIdx, int count)
		{
			int i = 0, size = source.Count();
			foreach (T item in source)
			{
				if (i >= startIdx && i < startIdx + count)
				{
					yield return item;
				}
				else if (i >= startIdx + count || i >= size)
				{
					yield break;
				}
				i++;
			}
		}
	}
}
