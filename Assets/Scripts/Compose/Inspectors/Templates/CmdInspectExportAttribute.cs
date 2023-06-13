using System;

namespace Arcript.Compose.Inspectors
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class CmdInspectExportAttribute : Attribute
	{
		public Type CmdModelType { get; private set; }

		public string CmdTypeName { get; private set; }

		public string CmdFriendlyName { get; private set; }

		public string CmdInspectPrefabPath { get; set; }

		public CmdInspectExportAttribute(Type cmdModelType, string typeName, string shownName, string prefabPath)
		{
			CmdModelType = cmdModelType;
			CmdTypeName = typeName;
			CmdFriendlyName = shownName;
			CmdInspectPrefabPath = prefabPath;
		}
	}
}
