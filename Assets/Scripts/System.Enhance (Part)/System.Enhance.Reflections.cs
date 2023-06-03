using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Enhance.Reflections
{
	public static class ReflectionExtension
	{
		public static object FetchDefaultValue<T>(this T type, string fieldName) where T : Type
		{
			var attr = type.GetField(fieldName).GetCustomAttributes(typeof(DefaultValueAttribute), false);
			if (attr.Length > 0)
			{
				return ((DefaultValueAttribute)attr[0]).Value;
			}
			return null;
		}

		public static TField FetchDefaultValue<TObject, TField>(this TObject type, string fieldName) where TObject : Type
		{
			var attr = type.GetField(fieldName).GetCustomAttributes(typeof(DefaultValueAttribute), false);
			if (attr.Length > 0)
			{
				return (TField)((DefaultValueAttribute)attr[0]).Value;
			}
			return default;
		}
	}
}