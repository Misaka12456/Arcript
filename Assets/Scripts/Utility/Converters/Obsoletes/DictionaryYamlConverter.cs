using System;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Arcript.Utility
{

	public delegate T ParseDelegate<T>(string value);
	public class DictionaryYamlConverter<TValue> : IYamlTypeConverter
	{
		public static DictionaryYamlConverter<string> String = new DictionaryYamlConverter<string>(x => x);

		private readonly ParseDelegate<TValue> _parser;

		public DictionaryYamlConverter(ParseDelegate<TValue> parser)
		{
			_parser = parser;
		}

		public bool Accepts(Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>) &&
				typeof(int).IsAssignableFrom(type.GetGenericArguments()[0]);
		}

		public object ReadYaml(IParser parser, Type type)
		{
			var keyType = type.GetGenericArguments()[0];
			var valueType = type.GetGenericArguments()[1];
			var dictionaryType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
			var result = Activator.CreateInstance(dictionaryType);

			parser.Consume<MappingStart>();

			while (parser.Current != null && parser.Current.GetType() != typeof(MappingEnd))
			{
				var key = (int)Convert.ChangeType(((Scalar)parser.Consume<Scalar>()).Value, keyType);
				var value = _parser(((Scalar)parser.Consume<Scalar>()).Value);
				dictionaryType.GetMethod("Add").Invoke(result, new object[] { key, value });
			}

			parser.Consume<MappingEnd>();

			return result;
		}

		public void WriteYaml(IEmitter emitter, object value, Type type)
		{
			var dict = (IDictionary<int, TValue>)value;

			emitter.Emit(new MappingStart());

			foreach (var pair in dict)
			{
				emitter.Emit(new Scalar(null, null, pair.Key.ToString(), ScalarStyle.Any, true, false));
				emitter.Emit(new Scalar(null, null, pair.Value.ToString(), ScalarStyle.Any, true, false));
			}

			emitter.Emit(new MappingEnd());
		}
	}

}
