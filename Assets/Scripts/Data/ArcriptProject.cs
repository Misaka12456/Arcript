using System;
using Newtonsoft.Json; // for JsonConvert
using UnityEngine.Scripting;

namespace Arcript.Data
{
	[Serializable]
	public class ArcriptProject
	{
		public string ProjectId = "org.misakacastle.arcript.NewEmptyProject";

		public string ProjectName = "New Empty Project";

		public string ProjectAuthor = "Misaka Castle with Arcript";

		public bool EnableArcriptPlusFeatures = false; // Enable Arcript+ features (a.k.a. Project Arcaba VN script features)

		public string LastOpenScript = string.Empty;

		public long LastOpenScriptLine = 0;
	}
}