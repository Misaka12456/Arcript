using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Enhance;
using System.Enhance.Unity;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace System.Enhance.Unity
{
	public class DislocatedVerticalList : MonoBehaviour
	{
		private bool busy = false; // 是否正在进行布局计算

		public List<float> DislocateOffsets = new List<float>(); // 所有元素的错位偏移量(相较于不可见的中轴线(视为0f)的偏移量,正数为向右偏移,负数为向左偏移)
														// 如果元素数量超过了DislocateOffsets的长度,则多出的元素的错位偏移量固定为0(即没有错位偏移)

		protected void OnEnable()
		{
			CheckAndAdjust();
		}

		private void OnTransformChildrenChanged()
		{
			CheckAndAdjust();
		}

		private void OnRectTransformDimensionsChange()
		{
			CheckAndAdjust();
		}

		private void OnRectTransformRemoved()
		{
			var childs = transform.GetComponentsInChildren<RectTransform>(includeInactive: true)
				.Where(child => child.GetComponent<DislocatedListElement>() != null)
				.ToList();
			childs.ForEach(child => child.gameObject.RemoveComponent<DislocatedListElement>());
		}

		private void CheckAndAdjust()
		{
			var childs = transform.GetComponentsInChildren<RectTransform>(includeInactive: true);
			childs = childs.Where(child => child.GetComponent<DislocatedListElement>() != null).ToArray(); // 提取出所有带有DVLGElement组件的子物体
			for (int i = 0; i < childs.Length; i++)
			{
				var child = childs[i];
				float finalXPos;
				if (DislocateOffsets.OutOfRange(i))
				{
					finalXPos = 0;
				}
				else
				{
					float offset = DislocateOffsets[i];
					finalXPos = offset;
				}
				child.anchoredPosition = new Vector2(finalXPos, child.anchoredPosition.y);
			}
		}

		/// <summary>
		/// 立即调整所有列表中元素的位置(在添加元素后可手动调用)
		/// </summary>
		public void AdjustImmediately() => CheckAndAdjust();

		/// <summary>
		/// 添加一个元素到列表中。
		/// </summary>
		/// <param name="childGObj">元素的原始 <see cref="GameObject"/> 对象。</param>
		/// <param name="childY">添加后元素的y坐标。默认为 <see langword="null"/> (即采用元素的原始y坐标)。</param>
		/// <param name="itsOffset">添加后元素的错位偏移量。默认为0f(即相较于中轴线不错位)。</param>
		public void AddElement(GameObject childGObj, float? childY = null, float itsOffset = 0f)
		{
			UniTask.Create(() => AddElementAsync(childGObj, childY, itsOffset)).Forget();
			GC.Collect(); // 防止Forget()导致的内存泄漏(即"创建的UniTask跑不完了,但是GC又不会回收它,导致内存泄漏")
		}

		private async UniTask AddElementAsync(GameObject childGObj, float? childY, float itsOffset)
		{
			await UniTask.SwitchToMainThread();
			await UniTask.WaitUntil(() => !busy);
			busy = true;
			var child = childGObj.GetComponent<RectTransform>();
			child.SetParent(transform);

			// change scale to default
			child.localScale = Vector3.one;

			// change alignment to middle center
			child.anchorMin = child.anchorMax = new Vector2(0.5f, 0.5f);
			child.pivot = new Vector2(0.5f, 0.5f);

			// set to default pos (without dislocate)
			child.anchoredPosition = new Vector2(0, childY.HasValue ? childY.Value : child.anchoredPosition.y);

			int childIdx = transform.GetComponentsInChildren<DislocatedListElement>(includeInactive: true).Length; // 这里不用+1，因为下标是从0开始的
			if (DislocateOffsets.OutOfRange(childIdx))
			{
				DislocateOffsets.Add(itsOffset);
			}
			else
			{
				DislocateOffsets.Insert(childIdx, itsOffset); // 这里不用arr[idx] = value的形式，因为这个会把已有的offset项覆盖掉
			}
			await UniTask.Yield(); // 作用与协程(Coroutine)中的WaitForEndOfFrame()相同
			CheckAndAdjust(); // adjust so position will be correct (*with* dislocate)
			busy = false;
		}

		/// <summary>
		/// 获取列表中指定下标的元素的 <see cref="DislocatedListElement"/> 组件。
		/// </summary>
		public DislocatedListElement this[int idx]
		{
			get
			{
				var childs = transform.GetComponentsInChildren<DislocatedListElement>(includeInactive: true);
				return childs[idx];
			}
		}
	}
}