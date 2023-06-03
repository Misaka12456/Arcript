//namespace Arcript.Assets.Scripts.System.Enhance__Part_
using System.Collections;

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
	}
}
