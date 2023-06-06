using UnityEngine;
using System.Enhance.Unity;
using TMPText = TMPro.TextMeshProUGUI;
using DG.Tweening; // Demigiant DOTween

namespace Arcript.Compose.UI
{
	public class ArptToast : Singleton<ArptToast>
	{
		[Header("General")]
		public RectTransform rectToast;
		public TMPText labelToast;

		protected override void SingletonAwake()
		{
			AllowRepeatInit = true;
		}

		public void Show(string text, float duration = 1.5f, float animDuration = 1.0f)
		{
			labelToast.SetText(text);
			labelToast.ForceMeshUpdate(); // 保证text赋值效果

			ResetToast(); // 重置Toast
			rectToast.DOPivotX(0f, animDuration);
			rectToast.DOPivotX(1f, animDuration).SetDelay(duration + animDuration).OnComplete(ResetToast);
		}

		private void ResetToastPos()
		{
			rectToast.DOKill(); // 停止所有动画
			rectToast.pivot = new Vector2(0f, 0.5f); // 重置pivot
			rectToast.anchoredPosition = new Vector2(0f, 0f); // 重置anchoredPosition
		}

		private void ResetToastText()
		{
			labelToast.SetText("Toast Text");
			labelToast.ForceMeshUpdate();
		}

		private void ResetToast()
		{
			ResetToastPos();
			ResetToastText();
		}
	}
}
