using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace System.Enhance.Unity
{
	public class WaitForExecutionCall : CustomYieldInstruction
	{
		private bool m_IsCalled = false;
		public WaitForExecutionCall()
		{

		}

		public void Invoke()
		{
			m_IsCalled = true;
		}

		public override bool keepWaiting { get => !m_IsCalled; }
	}
}
