using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	[AsptCmd(AsptCmdType.ShowPicture, isArptPlusCmd: true)]
	public class AsptPictureShowCmd : AsptCmdBase
	{
		[YamlMember(Alias = "Type")]
		public override AsptCmdType Type => AsptCmdType.ShowPicture;

		[YamlMember(Alias = "Block")]
		public override bool IsBlock { get; set; }

		[YamlMember(Alias = "ImagePath")]
		public string ImagePath { get; set; }

		[YamlMember(Alias = "Size")]
		public Vector2 Size { get; set; }

		[YamlMember(Alias = "StartPoint")]
		public Vector2 StartPoint { get; set; }

		[YamlMember(Alias = "Scale", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public Vector2 Scale { get; set; } = Vector2.one;

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
	}
}
