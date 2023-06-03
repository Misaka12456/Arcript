using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcript.Aspt.RawArcVNScripts
{
	public static class StringParserExtension
	{
		// several extended methods for StringParser

		public static FuncArgument ReadStringAsFuncArg(this StringParser parser, int line, string ternimator = "")
		{
			// 函数形参数(Function Argument)的形式为：
			// <funcName:string>(<arg1:object?>, <arg2:object?>, ...)
			// 如:
			// fade(sineinout, 0.25)
			string funcName = parser.ReadString(ternimator: "(");
			if (funcName == "none" || funcName == "null") return null;
			parser.Skip("(");
			var args = new List<object>();
			while (parser.Current[parser.Pos] != ')')
			{
				if (parser.Current[parser.Pos] == ',')
				{
					parser.Skip(",");
				}
				else
				{
					if (parser.Peek(1) == "\"") // stringB (string with Quotes / B initially is 'Brace', but it is '{' instead of '"', so it actually is string with Quotes)
					{
						string stringB = parser.ReadQuotedString();
					}
					else if (parser.TryReadFloat(out float @float))
					{
						args.Add(@float);
					}
					else if (parser.TryReadInt(out int @int))
					{
						args.Add(@int);
					}
					else if (parser.TryReadBool(out bool @bool))
					{
						args.Add(@bool);
					}
					else
					{
						throw new ArcriptScriptParseException(parser.Current[parser.Pos], line: line, column: parser.Pos, message: $"Unexpected token {parser.Current[parser.Pos]}.");
					}
				}
			}
			parser.Skip(")");
			return new FuncArgument()
			{
				FuncName = funcName,
				Arguments = args
			};
		}
	}
}
