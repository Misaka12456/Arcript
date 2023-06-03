namespace System.Enhance
{
	public static class StringHelper
	{
		/// <summary>
		/// 以字符串中最后一个指定的字符作为分隔线切分字符串。<br />
		/// 若字符串中不存在指定的字符则返回只有一个值为原完整字符串的元素的 <see cref="string[]"/> 数组。
		/// <para>如:
		/// <code>"123,456,789".SplitBeforeLast(','); // 结果为 ["123,456","789"]</code><br />
		/// <code>"I love CSharp".SplitBeforeLast(' '); // 结果为 ["I love","CSharp"]</code><br />
		/// <code>"Hello world!".SplitBeforeLast(','); // 结果为 ["Hello world!"]</code></para>
		/// </summary>
		/// <param name="str">当前要切分的字符串。</param>
		/// <param name="chars">要指定的字符。</param>
		/// <returns>切分结果。</returns>
		public static string[] SplitBeforeLast(this string str, char keyChar)
		{
			int index = str.LastIndexOf(keyChar);
			if (index != -1)
			{
				string before = str.Substring(0, index);
				string after;
				if (index == str.Length - 1)
				{
					after = string.Empty;
				}
				else
				{
					after = str.Substring(index + 1);
				}
				return new[] { before, after };
			}
			else
			{
				return new[] { str };
			}
		}

		public static string Override(this string @base, string value, bool isEnd = false)
		{
			string temp = isEnd ? @base.Substring(0, @base.Length - value.Length) : @base.Substring(value.Length, @base.Length - value.Length);
			return isEnd ? (temp + value) : (value + temp);
		}
	}
}
