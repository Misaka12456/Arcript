using System;

namespace Arcript.ArcVNScripts
{
	/// <summary>
	/// 表示一个视觉小说脚本(VN Script)解析过程中引发的异常。
	/// </summary>
	public class ArcriptScriptParseException : Exception
	{
		public int Line { get; private set; }
		public int Column { get; private set; } // a.k.a. Column

		public char Token { get; private set; }

		public ArcriptScriptParseException(char tokenChar, int line, int column) : base($"Unexpected token '{tokenChar}'. Line {line}, column {column}.")
		{
			Token = tokenChar;
			Line = line;
			Column = column;
		}

		public ArcriptScriptParseException(char tokenChar, int line, int column, string message) : base(message)
		{
			Token = tokenChar;
			Line = line;
			Column = column;
		}

		public ArcriptScriptParseException(char tokenChar, int line, int column, Exception innerException) : base($"Unexpected token '{tokenChar}'. Line {line}, column {column}.", innerException)
		{
			Token = tokenChar;
			Line = line;
			Column = column;
		}

		public ArcriptScriptParseException(char tokenChar, int line, int column, string message, Exception innerException) : base(message, innerException)
		{
			Token = tokenChar;
			Line = line;
			Column = column;
		}
	}
}
