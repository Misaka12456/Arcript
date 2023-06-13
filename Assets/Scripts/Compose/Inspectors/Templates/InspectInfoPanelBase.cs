using UnityEngine;

namespace Arcript.Compose.Inspectors
{
	public abstract class InspectInfoPanelBase : MonoBehaviour
	{
		public abstract GameObject TemplatePrefab { get; }

		public abstract void SetInfo(string fileName, string typeName);
	}
}
