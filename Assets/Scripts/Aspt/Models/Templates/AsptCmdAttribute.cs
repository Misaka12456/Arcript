using System;

namespace Arcript.Aspt
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public class AsptCmdAttribute : Attribute
	{
		public AsptCmdType CmdType { get; }

		public bool IsArptPlusCmd { get; } = false;

		public bool IsSubCmd { get; set; } = false;

		public AsptCmdAttribute(AsptCmdType cmdType, bool isArptPlusCmd = false, bool isSubCmd = false) : base()
		{
			CmdType = cmdType;
			IsArptPlusCmd = isArptPlusCmd;
			IsSubCmd = isSubCmd;
		}
	}
}
