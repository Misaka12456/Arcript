using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace System.Enhance.Unity
{
	public static class ScrollRectHelper
	{
		// 直接滚动到指定元素
		public static void ScrollImmediately(this ScrollRect rect, int elementIdx)
		{
			// 模仿NGUI的Scroll()方法
			var elements = rect.content.GetComponentsInChildren<RectTransform>();
			if (elements.OutOfRange(elementIdx))
			{
				throw new ArgumentOutOfRangeException("Are you kidding me?");
			}
			float normalizedPos = (float)elementIdx / elements.Length;
			rect.verticalNormalizedPosition = 1f - normalizedPos;
			Thread.Sleep(0); // == await UniTask.WaitForEndOfFrame();
		}

		// DOTween滚动到指定元素
		public static Tweener Scroll(this ScrollRect rect, int elementIdx, float duration = 0.5f)
		{
			// 模仿NGUI的Scroll()方法
			var elements = rect.content.GetComponentsInChildren<RectTransform>();
			if (elements.OutOfRange(elementIdx))
			{
				throw new ArgumentOutOfRangeException("Are you kidding me?");
			}
			float normalizedPos = (float)elementIdx / elements.Length;
			var t = DOTween.To(getter: () => rect.verticalNormalizedPosition,
				setter: x => rect.verticalNormalizedPosition = x,
				endValue: 1f - normalizedPos,
				duration: duration);
			return t;
		}

		// UniTask滚动到指定元素
		public static async UniTask ScrollAsync(this ScrollRect rect, int elementIdx, float duration = 0.5f, int steps = 10)
		{
			await UniTask.SwitchToMainThread();
			var elements = rect.content.GetComponentsInChildren<RectTransform>();
			if (elements.OutOfRange(elementIdx))
			{
				throw new ArgumentOutOfRangeException("Are you kidding me?");
			}
			float normalizedPos = (float)elementIdx / elements.Length;
			float stepDistance = (normalizedPos - rect.verticalNormalizedPosition) / steps;
			float stepDuration = duration / steps;

			for (int i = 0; i < steps; i++)
			{
				rect.verticalNormalizedPosition += stepDistance;
				await UniTask.Delay(TimeSpan.FromSeconds(stepDuration));
			}

			// 确保最终位置正确
			rect.verticalNormalizedPosition = normalizedPos;
		}
	}
}
