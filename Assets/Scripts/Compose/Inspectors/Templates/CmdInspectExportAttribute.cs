using System;

namespace Arcript.Compose.Inspectors
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class CmdInspectExportAttribute : Attribute
	{
		public Type CmdModelType { get; private set; }

		public string CmdTypeName { get; private set; }

		public CmdInspectExportAttribute(Type cmdModelType, string typeName)
		{
			CmdModelType = cmdModelType;
			CmdTypeName = typeName;
		}
	}
}
