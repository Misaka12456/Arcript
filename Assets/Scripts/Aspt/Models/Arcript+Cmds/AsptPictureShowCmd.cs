using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	[AsptCmd(AsptCmdType.ShowPicture, isArptPlusCmd: true)]
	public class AsptPictureShowCmd : AsptCmdBase
	{
		[YamlMember(Alias = "Type")]
		public override string TypeStr => "show";

		[YamlMember(Alias = "Block")]
		public override bool IsBlock { get; set; }

		[YamlMember(Alias = "ImagePath")]
		public string ImagePath { get; set; }

		[YamlMember(Alias = "Size")]
		public float[] SizeArray { get; set; }

		[YamlIgnore]
		public Vector2 Size { get => new Vector2(SizeArray[0], SizeArray[1]); set => SizeArray = new float[] { value.x, value.y }; }

		[YamlMember(Alias = "StartPoint")]
		public float[] StartPointArray { get; set; }

		[YamlIgnore]
		public Vector2 StartPoint { get => new Vector2(StartPointArray[0], StartPointArray[1]); set => StartPointArray = new float[] { value.x, value.y }; }

		[YamlMember(Alias = "Scale", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public float[] ScaleArray { get; set; } = new float[] { 1, 1 };

		[YamlIgnore]
		public Vector2 Scale { get => new Vector2(ScaleArray[0], ScaleArray[1]); set => ScaleArray = new float[] { value.x, value.y }; }

		[YamlMember(Alias = "Transition")]
		public Transition Transition { get; set; }

		[YamlMember(Alias = "Layer")]
		public string Layer { get; set; }

		[YamlMember(Alias = "ScaleToWidth", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public bool ScaleToWidth { get; set; } = false;

		public override T ExpandFromQFunc<T>(ArcriptScript parent)
		{
			if (typeof(T) != typeof(AsptPictureShowLegacyCmd))
			{
				return default;
			}
			var qfuncShortcut = base.QuickFunction;
			var qfunc = parent.QFuncs.Find(f => f.FuncName == qfuncShortcut.NameOrShortCut || f.ShortCutName == qfuncShortcut.NameOrShortCut);
			if (qfunc == null) return default;
			string[] rawArgs = qfunc.Arguments; // 每个元素都是"argName = targetFieldName"的形式
			var argFieldTable = rawArgs.ToDictionary(x => x.Split('=')[0].Trim(), x => x.Split('=')[1].Trim());
			object[] rawValues = qfuncShortcut.Arguments;

			var args = new Dictionary<string, object>();
			for (int i = 0; i < rawArgs.Length; i++)
			{
				var argName = rawArgs[i].Split('=')[0].Trim();
				var argValue = rawValues[i];
				args.Add(argName, argValue);
			}

			var fieldValueTable = new Dictionary<string, object>();
			foreach (var arg in args)
			{
				var argName = arg.Key;
				var argValue = arg.Value;
				var targetFieldName = argFieldTable[argName];
				fieldValueTable.Add(targetFieldName, argValue);
			}

			var templates = qfunc.CmdTemplate;
			
			if (templates.Length <= 0) return default;
			var t = templates[0];
			var tCmd = t as AsptPictureShowCmd;

			// 通过反射设置tCmd的值
			foreach (var field in tCmd.GetType().GetFields())
			{
				if (fieldValueTable.ContainsKey(field.Name))
				{
					field.SetValue(tCmd, fieldValueTable[field.Name]);
				}
			}

			return tCmd as T;
		}

		public override string ToItemShortString()
		{
			var sb = new StringBuilder("<b>show</b> ");
			sb.Append(ImagePath).Append(" | ");
			sb.Append("size = ").Append(Size).Append(" | ");
			sb.Append("startPoint = ").Append(StartPoint).Append(" | ");
			sb.Append("scale = ").Append(Scale).Append(" | ");
			sb.Append("transition = ").Append(Transition.ToItemShortString()).Append(" | ");
			sb.Append("layer = ").Append(Layer);
			if (ScaleToWidth)
			{
				sb.Append(" | scaleToWidth = ").Append(ScaleToWidth);
			}
			return sb.ToString();
		}
	}
}
