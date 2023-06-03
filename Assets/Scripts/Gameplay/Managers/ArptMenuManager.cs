using System.Enhance.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Arcript.Gameplay
{
	public class ArptMenuManager : Singleton<ArptMenuManager>
	{
		[Header("General")]
		public Button btnMenuShow, btnMenuClose;
		public Animator animMenuShow; // 速度调成-1即可实现"animMenuHide"
		public Button btnHideText;
		public Button btnQuit;

		protected override void SingletonAwake()
		{
			AllowRepeatInit = true;
		}
	}
}
