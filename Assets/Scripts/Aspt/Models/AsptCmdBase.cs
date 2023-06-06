using Arcript.Aspt.QuickFuncs;
using System;
using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	public abstract class AsptCmdBase
	{
		[YamlMember(Alias = "Type")]
		public virtual string TypeStr { get; set; }

		[YamlMember(Alias = "Block", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public virtual bool IsBlock { get; set; } = false;

		[YamlMember(Alias = "EBeforeExec", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public virtual AsptCmdBase[] EventBeforeExec { get; set; } = Array.Empty<AsptCmdBase>();

		[YamlMember(Alias = "EAfterExec", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public virtual AsptCmdBase[] EventAfterExec { get; set; } = Array.Empty<AsptCmdBase>();

		[YamlMember(Alias = "QFunc", DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		//public virtual QuickFunction QuickFunction { get; set; } = null;
		public virtual QFuncShortCut QuickFunction { get; set; } = null;

		public virtual T ExpandFromQFunc<T>(ArcriptScript parent) where T : AsptCmdBase
		{
			return (T)this;
		}

		public abstract string ToItemShortString(); // 显示在CmdItem上的字符串
	}
}