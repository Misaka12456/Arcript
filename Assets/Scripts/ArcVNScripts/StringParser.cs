using System;
using System.Text;

namespace Arcript.ArcVNScripts
{
	public class StringParser
	{
		public const string Space = " ";
		public const string Colon = ":";
		public const string Comma = ",";
		public const string Semicolon = ";";
		public const string LeftBrace = "{", RightBrace = "}";
		public const string Equal = "=";

		public int Pos { get; private set; } = 0;
		public bool EOF => Pos >= str.Length;
		public bool EOFSpaceIgnored
		{
			get
			{
				int i = Pos;
				while (i < str.Length && str[i] == ' ')
				{
					i++;
				}
				return i >= str.Length;
			}
		}
		public string FullStr => str;
		private string str;
		
		public StringParser(string str)
		{
			this.str = str;
		}
		public void Skip(int length)
		{
			Pos += length;
		}
		public void Skip(string str)
		{
			if (Peek(str.Length) == str)
			{
				Pos += str.Length;
			}
		}

		public void SkipWhitespaces()
		{
			while (Current == Space || Current == "\n" || Current == "\r")
			{
				Skip(1);
			}
		}

		public float ReadFloat(string ternimator = null)
		{
			int end = ternimator != null ? str.IndexOf(ternimator, Pos) : (str.Length - 1);
			float value = float.Parse(str.Substring(Pos, end - Pos));
			Pos += (end - Pos + 1);
			return value;
		}
		public int ReadInt(string ternimator = null)
		{
			int end = ternimator != null ? str.IndexOf(ternimator, Pos) : (str.Length - 1);
			int value = int.Parse(str.Substring(Pos, end - Pos));
			Pos += (end - Pos + 1);
			return value;
		}
		public bool ReadBool(string ternimator = null)
		{
			int end = ternimator != null ? str.IndexOf(ternimator, Pos) : (str.Length - 1);
			bool value = bool.Parse(str.Substring(Pos, end - Pos));
			Pos += (end - Pos + 1);
			return value;
		}
		public string ReadString(string ternimator = null)
		{
			int end = ternimator != null ? str.IndexOf(ternimator, Pos) : (str.Length - 1);
			string value = str.Substring(Pos, end - Pos);
			Pos += (end - Pos + 1);
			return value;
		}
		public string Current
		{
			get
			{
				return str[Pos].ToString();
			}
		}
		public string Peek(int count)
		{
			return str.Substring(Pos, count);
		}
		public bool TryReadFloat(out float target, string ternimator = null, bool skip = false)
		{
			try
			{
				int end = ternimator != null ? str.IndexOf(ternimator, Pos) : (str.Length - 1);
				bool value = float.TryParse(str.Substring(Pos, end - Pos), out target);
				if (value || skip) Pos += (end - Pos + 1);
				return value;
			}
			catch (Exception)
			{
				target = 0;
				return false;
			}
		}
		public bool TryReadInt(out int target, string ternimator = null, bool skip = false)
		{
			try
			{
				int end = ternimator != null ? str.IndexOf(ternimator, Pos) : (str.Length - 1);
				bool value = int.TryParse(str.Substring(Pos, end - Pos), out target);
				if (value || skip) Pos += (end - Pos + 1);
				return value;
			}
			catch (Exception)
			{
				target = 0;
				return false;
			}
		}

		public bool TryReadBool(out bool target, string ternimator = null, bool skip = false)
		{
			try
			{
				int end = ternimator != null ? str.IndexOf(ternimator, Pos) : (str.Length - 1);
				bool value = bool.TryParse(str.Substring(Pos, end - Pos), out target);
				if (value || skip) Pos += (end - Pos + 1);
				return value;
			}
			catch (Exception)
			{
				target = false;
				return false;
			}
		}

		public string ReadQuotedString()
		{
			// 读取带引号的字符串(字符串中以\"表示的转义引号直接转义到最终字符串中，且不将该引号作为字符串的结束符)
			// 例如：str = "abcd\"ef", ReadQuotedString返回值应为abcd"ef，而不是abcd\
			var sb = new StringBuilder();
			if (Current != "\"")
			{
				if (Current == "n")
				{
					string nullSign = Peek(4);
					if (nullSign == "null")
					{
						Skip(4);
						return null;
					}
				}
				throw new Exception("String should start with \"");
			}
			Skip(1);
			while (Pos < str.Length)
			{
				if (Current == "\"")
				{
					if (Peek(1) == "\"")
					{
						sb.Append("\"");
						Skip(2);
					}
					else
					{
						Skip(1);
						return sb.ToString();
					}
				}
				else
				{
					sb.Append(Current);
					Skip(1);
				}
			}
			throw new Exception("String should end with \"");
		}

		/// <summary>
		/// 将当前位置(跳过指定字符)移动到EOF(行尾)
		/// </summary>
		/// <param name="skipStr">要跳过的字符</param>
		/// <returns>若当前已在EOF则返回 <see langword="true"/> ，否则返回 <see langword="false"/></returns>
		public bool MoveToEOF(string skipStr = null)
		{
			if (skipStr != null)
			{
				while (Pos < str.Length && skipStr.Contains(Current))
				{
					Skip(1);
				}
				return Pos == str.Length;
			}
			else
			{
				Pos = str.Length;
				return true;
			}
		}
	}
}
