using System;
using UnityEngine;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Arcript.Utility
{
	public class Vector2YamlConverter : IYamlTypeConverter
	{
		public bool Accepts(Type type) => type == typeof(Vector2);

		public object ReadYaml(IParser parser, Type type)
		{
			var value = ((Scalar)parser.Current).Value;
			parser.MoveNext();
			var parts = value.Split(',');
			if (parts.Length != 2)
			{
				throw new InvalidOperationException($"Invalid Vector2 format: {value}");
			}
			return new Vector2(float.Parse(parts[0]), float.Parse(parts[1]));
		}

		public void WriteYaml(IEmitter emitter, object value, Type type)
		{
			var v = (Vector2)value;
			emitter.Emit(new SequenceStart(null, null, false, SequenceStyle.Flow));
			emitter.Emit(new Scalar(null, null, $"{v.x}", ScalarStyle.Any, true, false));
			emitter.Emit(new Scalar(null, null, $"{v.y}", ScalarStyle.Any, true, false));
			emitter.Emit(new SequenceEnd());
		}
	}
}
