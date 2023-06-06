using System;
using System.Collections.Generic;
using System.Enhance;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using System.ComponentModel;
using UnityEngine;

namespace Arcript.Utility
{
	/// <summary>
	/// Provides a way to convert a string to an enum value (and reverse) using the enum's <see cref="DescriptionAttribute"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class DescStr2EnumConverter<T> : IYamlTypeConverter where T : Enum
	{
		private readonly Dictionary<string, T> map;
		public DescStr2EnumConverter()
		{
			map = Enum.GetValues(typeof(T))
				.Cast<T>()
				.ToDictionary(x => x.GetDescription(), x => x);
		}

		public bool Accepts(Type type) => type == typeof(T);

		public object ReadYaml(IParser parser, Type type)
		{
			var value = ((Scalar)parser.Current).Value;
			parser.MoveNext();
			if (!map.TryGetValue(value, out T result))
			{
				Debug.LogError($"Unknown value '{value}' for enum '{typeof(T).FullName}'.");
			}
			return result;
		}

		public void WriteYaml(IEmitter emitter, object value, Type type)
		{
			var description = ((T)value).GetDescription();
			emitter.Emit(new Scalar(null, null, description, ScalarStyle.SingleQuoted, true, false));
		}
	}
}
