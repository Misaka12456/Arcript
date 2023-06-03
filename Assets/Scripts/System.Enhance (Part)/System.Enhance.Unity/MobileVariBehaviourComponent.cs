using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace System.Enhance.Unity
{
	/// <summary>
	/// 表示一个在移动端显示行为不同的组件。
	/// </summary>
	public class MobileVariBehaviourComponent : MonoBehaviour
	{
		/// <summary>
		/// 在移动端是否显示该组件。
		/// </summary>
		public bool ShowInMobile;

		private void Awake()
		{
#if UNITY_ANDROID || UNITY_IOS
			gameObject.SetActive(ShowInMobile);
#endif
		}
	}
}
