﻿using Arcript.Aspt;
using Arcript.Aspt.RawArcVNScripts;
using Arcript.Compose.Inspectors;
using System.Enhance.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Arcript.Compose.Script
{
	public class ArptScriptCmdItem : MonoBehaviour
	{
		[Header("General")]
		public Text labelCmdText;
		public Toggle toggleChoosing;

		[SerializeField]
		private ArcVNScriptCmdBase refCmd;

		private void Awake()
		{
			toggleChoosing.onValueChanged.AddListener((bool isOn) =>
			{
				if (!isOn) return;
				ArptItemInspector.Instance.SetCmdInfo(this, refCmd);
			});
		}
	}

	public class ArptScriptEditorManager : Singleton<ArptScriptEditorManager>
	{

		protected override void SingletonAwake()
		{
			AllowRepeatInit = true;
		}

		public void LoadScript(ArcriptScript script)
		{
			
		}
	}
}
