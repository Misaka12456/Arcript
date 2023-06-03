using System.Collections.Generic;

namespace System.Enhance
{
	public static class TupleHelper
	{
		#region KeyValuePair to Tuple & ValueTuple
		/// <summary>
		/// Convert a <see cref="KeyValuePair{K, V}"/> to a <see cref="Tuple{K, V}"/> instance.
		/// </summary>
		/// <typeparam name="K">The type of the key.</typeparam>
		/// <typeparam name="V">The type of the value.</typeparam>
		/// <param name="kv">The <see cref="KeyValuePair{K, V}"/> instance.</param>
		/// <returns>Converted <see cref="Tuple{K, V}"/> instance.</returns>
		public static Tuple<K, V> ToTuple<K, V>(this KeyValuePair<K, V> kv) => new Tuple<K, V>(kv.Key, kv.Value);

		/// <summary>
		/// Convert a <see cref="KeyValuePair{K, V}"/> to a <see cref="ValueTuple{K, V}"/> (<c>ValueTuple</c>) instance.
		/// </summary>
		/// <typeparam name="K">The type of the key.</typeparam>
		/// <typeparam name="V">The type of the value.</typeparam>
		/// <param name="kv">The <see cref="KeyValuePair{K, V}"/> instance.</param>
		/// <returns>Converted <see cref="ValueTuple{K, V}"/> instance.</returns>
		public static (K, V) ToValueTuple<K, V>(this KeyValuePair<K, V> kv) => (kv.Key, kv.Value);
		#endregion
	}
}
