using System.Enhance.Unity; // for Singleton<T> where T : MonoBehaviour
using UnityEngine;

namespace Arcript.Gameplay
{
	public class ArptBranchSelectionManager : Singleton<ArptBranchSelectionManager>
	{
		[Header("UI")]
		public DislocatedVerticalList listSelection;

		protected override void SingletonAwake()
		{
			AllowRepeatInit = true;
		}
	}
}
