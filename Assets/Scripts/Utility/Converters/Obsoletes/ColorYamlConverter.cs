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
			//var value = ((Scalar)parser.Current).Value;
			//parser.MoveNext();
			//var parts = value.Split(',');
			//if (parts.Length != 4)
			//{
			//	Debug.LogError($"Invalid Color format: {value}");
			//}
			//return new Color(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
			// 按照数组处理
			if (parser.TryConsume<SequenceStart>(out _))
			{
				float r = float.Parse(parser.Consume<Scalar>().Value);
				float g = float.Parse(parser.Consume<Scalar>().Value);
				float b = float.Parse(parser.Consume<Scalar>().Value);
				float a = float.Parse(parser.Consume<Scalar>().Value);
				parser.Consume<SequenceEnd>();
				return new Color(r, g, b, a);
			}
			else
			{
				Debug.LogError($"Invalid Color format: {parser.Current}");
				return Color.white;
			}
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
