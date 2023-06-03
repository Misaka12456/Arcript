using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace System.Enhance.Unity
{
	public class ButtonHelper : MonoBehaviour, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler, IPointerClickHandler
	{
		// 延迟时间
		[InspectorName("General")]
		public float Delay;
		public UnityEvent OnLongPressed;
		public UnityEvent OnPressed;
		public UnityEvent OnDoubleClicked;
		public UnityEvent OnPressingDown;
		public UnityEvent OnPressedUp;

		// 按钮是否是按下状态
		private bool isDown = false;

		// 按钮最后一次是被按住状态时候的时间
		private DateTime lastIsDownTime;

		void Update()
		{
			// 如果按钮是被按下状态
			if (isDown)
			{
				// 当前时间 -  按钮最后一次被按下的时间 > 延迟时间0.2秒
				if ((DateTime.Now - lastIsDownTime).TotalSeconds > Delay)
				{
					// 触发长按方法
					OnLongPressed?.Invoke();
					// 记录按钮最后一次被按下的时间
					lastIsDownTime = DateTime.Now;
					isDown = false;
				}
			}
		}

#if UNITY_EDITOR || UNITY_STANDALONE
		// 当按钮被按下后系统自动调用此方法
		public void OnPointerDown(PointerEventData eventData)
		{
			isDown = true;
			lastIsDownTime = DateTime.Now;
			OnPressingDown?.Invoke();
		}

		// 当按钮抬起的时候自动调用此方法
		public void OnPointerUp(PointerEventData eventData)
		{
			if (isDown && (DateTime.Now - lastIsDownTime).TotalSeconds < Delay)
			{
				OnPressedUp?.Invoke();
				OnPressed?.Invoke();
			}
			isDown = false;
		}

		// 当鼠标从按钮上离开的时候自动调用此方法
		public void OnPointerExit(PointerEventData eventData)
		{
			isDown = false;
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.clickCount == 2)
			{
				OnDoubleClicked?.Invoke();
			}
		}
#else
		public void OnPointerDown(PointerEventData eventData) { }

		public void OnPointerUp(PointerEventData eventData) { }

		public void OnPointerExit(PointerEventData eventData) { }

		public void OnPointerClick(PointerEventData eventData) { }
#endif
	}
}
