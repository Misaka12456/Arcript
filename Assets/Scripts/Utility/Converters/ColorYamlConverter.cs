using System;
using UnityEngine;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Arcript.Utility
{
	public class ColorYamlConverter : IYamlTypeConverter
	{
		public bool Accepts(Type type) => type == typeof(Color);

		public object ReadYaml(IParser parser, Type type)
		{
			var value = ((Scalar)parser.Current).Value;
			parser.MoveNext();
			var parts = value.Split(',');
			if (parts.Length != 4)
			{
				Debug.LogError($"Invalid Color format: {value}");
			}
			return new Color(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
		}

		public void WriteYaml(IEmitter emitter, object value, Type type)
		{
			var v = (Color)value;
			emitter.Emit(new SequenceStart(null, null, false, SequenceStyle.Flow));
			emitter.Emit(new Scalar(null, null, $"{v.r}", ScalarStyle.Any, true, false));
			emitter.Emit(new Scalar(null, null, $"{v.g}", ScalarStyle.Any, true, false));
			emitter.Emit(new Scalar(null, null, $"{v.b}", ScalarStyle.Any, true, false));
			emitter.Emit(new Scalar(null, null, $"{v.a}", ScalarStyle.Any, true, false));
			emitter.Emit(new SequenceEnd());
		}
	}
}
