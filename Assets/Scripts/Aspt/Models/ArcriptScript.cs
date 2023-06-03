using Arcript.Aspt.QuickFuncs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace Arcript.Aspt
{
	public class ArptScriptInfo
	{
		[YamlMember(Alias = "ScriptName")]
		public string ScriptName { get; set; } = "New Script";

		[YamlMember(Alias = "ScriptId")]
		public string ScriptId { get; set; } = "moe.misakacastle.arcript.newscript";

		[YamlMember(Alias = "EnableArcriptPlusFeatures")]
		public bool EnableArcriptPlusFeatures { get; set; } = false;
	}

	public class ArcriptScript // 即"Aspt"("Arcript Script")的缩写
	{
		[YamlMember(Alias = "Settings")]
		public ArptScriptInfo ScriptInfo { get; set; }

		[YamlMember(Alias = "QuickFunctions")]
		public List<QuickFunction> QFuncs { get; set; } = new List<QuickFunction>();

		[YamlMember(Alias = "QuickStands")]
		public List<QuickStand> QStands { get; set; } = new List<QuickStand>();

		[YamlMember(Alias = "Masks")]
		public List<QuickMask> Masks { get; set; } = new List<QuickMask>();

		[YamlMember(Alias = "Commands")]
		public LinkedList<AsptCmdBase> Commands { get; set; } = new LinkedList<AsptCmdBase>();
	}
}
