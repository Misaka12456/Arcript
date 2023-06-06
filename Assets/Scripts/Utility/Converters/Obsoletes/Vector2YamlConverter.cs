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
			// 改成按照数组 [ x, y ] 的格式读取
			if (parser.TryConsume<SequenceStart>(out _))
			{
				var x = float.Parse(parser.Consume<Scalar>().Value);
				var y = float.Parse(parser.Consume<Scalar>().Value);
				parser.Consume<SequenceEnd>();
				return new Vector2(x, y);
			}
			else
			{
				Debug.LogError($"Invalid Vector2 format: {parser.Current}");
				return Vector2.zero;
			}
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
