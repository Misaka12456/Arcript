using Arcript.ArcVNScripts;
using Cysharp.Threading.Tasks;
using System.Enhance.Unity;
using UnityEngine;
using UnityEngine.Events;

namespace Arcript.Gameplay
{
	public class ArptGameplayManager : Singleton<ArptGameplayManager>
	{
		#region Quasi Consts (准常量)
		public Canvas canvasGameplay;
		public AudioSource audioBGM, audioEffect;
		#endregion

		public ArcVNScript Script { get; private set; }
		public bool Blocking { get; private set; } = false;
		public bool IgnoreInput { get; set; } = false;

		#region Events
		[HideInInspector] public UnityEvent eventOnGameplayStart;
		[HideInInspector] public UnityEvent eventOnGameplayEnd;
		[HideInInspector] public UnityEvent eventOnScreenTap;
		#endregion

		protected override void SingletonAwake()
		{
			AllowRepeatInit = true;
			eventOnGameplayStart = eventOnGameplayEnd = eventOnScreenTap
				= new UnityEvent();
			eventOnScreenTap.AddListener(GoNext);
		}

		private void GoNext()
		{
			if (IgnoreInput) return;
			if (Blocking)
			{
				Blocking = false;
			}
		}

		public void Load(ArcVNScript script)
		{
			ArptScriptSandBoxManager.Instance.Load(script);
			Script = script;
		}

		private async UniTask ExecuteCmdListAsync(int startCmdIdx = 0)
		{
			for (int i = startCmdIdx; i < Script.Commands.Count; i++)
			{
				var cmd = Script.Commands[i];
				if (!cmd.Execute()) await UniTask.WaitUntil(() => !Blocking);
			}
		}
	}
}