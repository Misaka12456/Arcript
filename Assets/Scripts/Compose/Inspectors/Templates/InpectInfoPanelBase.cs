using UnityEngine;

namespace Arcript.Compose.Inspectors
{
	public abstract class InpectInfoPanelBase : MonoBehaviour
	{
		public abstract GameObject TemplatePrefab { get; }

		public abstract void SetInfo(string fileName, string typeName);
	}
}
