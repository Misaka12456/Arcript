using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YamlDotNet.Serialization;

namespace Arcript.Aspt.QuickFuncs
{
	[AsptCmd(AsptCmdType.SubQuickFunction, isArptPlusCmd: true, isSubCmd: true)]
	public class QuickFunction : AsptCmdBase, ISubCmd
	{
		[YamlMember(Alias = "Type")]
		public override AsptCmdType Type => AsptCmdType.SubQuickFunction;

		[YamlMember(Alias = "Block", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public override bool IsBlock { get; set; } = false;

		[YamlMember(Alias = "Name")]
		public string FuncName { get; set; } // 以命名空间命名格式的方法名，如'arpt.floflo.stands.centershow'

		[YamlMember(Alias = "ShortCut")]
		public string ShortCutName { get; set; } // 简称，如'SCentershow'

		[YamlMember(Alias = "Arguments")]
		public string[] Arguments { get; set; } // 参数列表, 每个元素的格式都是$"{参数名} = {要替换的命令模板中的字段名}"
												// 如"imgPath = ImagePath", "duration = Transition.Duration"

		[YamlMember(Alias = "CmdTemplate")]
		public AsptCmdBase[] CmdTemplate { get; set; } // 命令模板
	}

	public class QFuncShortCut
	{
		[YamlMember(Alias = "Name")]
		public string NameOrShortCut { get; set; }

		[YamlMember(Alias = "Args")]
		public object[] Arguments { get; set; }
	}
}