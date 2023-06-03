using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Enhance
{
	public static class EnumsHelper
	{
		/// <summary>
		/// 将一个使用<c>|</c>合并的枚举拆分为多个枚举
		/// </summary>
		/// <typeparam name="T">枚举类型</typeparam>
		/// <param name="source">合并的枚举</param>
		/// <returns>拆分后的枚举列表</returns>
		public static List<T> DetachMergedEnum<T>(this T givenValue) where T : Enum
		{
			var source = givenValue as Enum;
			if (source == null) throw new ArgumentException("T must be an enumerated type");
			var result = new List<T>();
			// foreach (T value in Enum.GetValues(typeof(T)))
			// foreach中进行了object->T的隐式转换，在运行时可能会出错，因此:
			foreach (var value in Enum.GetValues(typeof(T)).Cast<T>())
			{
				if (source.HasFlag(value))
				{
					result.Add(value);
				}
			}
			return result;
		}

		public static bool AnyEquals<T>(this T givenValue, params T[] values) where T : Enum
		{
			return values.Length > 0 ? values.Any(v => v.Equals(givenValue)) : false;
		}

		public static string GetDescription<T>(this T givenValue) where T : Enum
		{
			var type = givenValue.GetType();
			var name = Enum.GetName(type, givenValue);
			if (name != null)
			{
				var field = type.GetField(name);
				if (field != null)
				{
					var attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
					if (attr != null)
					{
						return attr.Description;
					}
				}
			}
			return null;
		}

		public static object ParseByDescription(this Type enumType, string description)
		{
			foreach (var field in enumType.GetFields())
			{
				var attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
				if (attr != null)
				{
					if (attr.Description == description)
					{
						return field.GetValue(null);
					}
				}
				else
				{
					if (field.Name == description)
					{
						return field.GetValue(null);
					}
				}
			}
			throw new ArgumentException($"No such enum with description \"{description}\".");
		}

		public static T ParseByDescription<T>(string description) where T : Enum
		{
			return (T)ParseByDescription(typeof(T), description);
		}
	}
}
