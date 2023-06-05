using Arcript.Aspt;
using System;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Arcript.Utility
{
	public class PolymorphicTypeConverter : IYamlTypeConverter
	{
		private readonly Dictionary<string, Type> _typeMap = new Dictionary<string, Type>();

		public bool Accepts(Type type)
		{
			return typeof(AsptCmdBase).IsAssignableFrom(type);
		}

		public void RegisterType(Type type)
		{
			var cmdBaseType = typeof(AsptCmdBase);
			if (!cmdBaseType.IsAssignableFrom(type))
			{
				throw new ArgumentException($"Type {type.FullName} does not inherit from {cmdBaseType.FullName}");
			}

			var cmd = (AsptCmdBase)Activator.CreateInstance(type);
			_typeMap[cmd.TypeStr] = type;
		}

		public object ReadYaml(IParser parser, Type type)
		{
			var value = ((Scalar)parser.Current).Value;
			parser.MoveNext();
			if (!_typeMap.TryGetValue(value, out Type result))
			{
				throw new Exception($"Unknown value '{value}' for enum '{typeof(AsptCmdBase).FullName}'.");
			}
			return result;
		}

		public void WriteYaml(IEmitter emitter, object value, Type type)
		{
			var cmd = (AsptCmdBase)value;
			emitter.Emit(new Scalar(null, null, cmd.TypeStr, ScalarStyle.SingleQuoted, true, false));
		}
	}
}
