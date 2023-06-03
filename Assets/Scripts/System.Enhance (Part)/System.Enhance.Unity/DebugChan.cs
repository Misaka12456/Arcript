using System.Runtime.CompilerServices;
using UnityEngine;
using Object2 = UnityEngine.Object;

namespace System.Enhance.Unity
{
	public class DebugChan
	{
		public static bool ActiveDebugInProduction { get; set; } = false;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void DoWhenInEditorOrDebugBuild(Action action)
		{
#if UNITY_EDITOR
			action();
			return;
#else
			if (Debug.isDebugBuild)
			{
				action();
				return;
			}
#endif
		}

		private static void InnerCaller(Action action)
		{
			if (ActiveDebugInProduction)
			{
				action();
				return;
			}
			DoWhenInEditorOrDebugBuild(action);
		}

		public static void Log(object message)
		{
			InnerCaller(() => Debug.Log(message));
		}
		public static void Log(object message, Object2 context)
		{
			InnerCaller(() => Debug.Log(message, context));
		}

		public static void LogFormat(string format, params object[] args)
		{
			InnerCaller(() => Debug.LogFormat(format, args));
		}

		public static void LogFormat(Object2 context, string format, params object[] args)
		{
			InnerCaller(() => Debug.LogFormat(context, format, args));
		}

		public static void LogFormat(LogType logType, LogOption logOptions, Object2 context, string format, params object[] args)
		{
			InnerCaller(() => LogFormat(logType, logOptions, context, format, args));
		}

		public static void LogError(object message)
		{
			InnerCaller(() => Debug.LogError(message));
		}

		public static void LogError(object message, Object2 context)
		{
			InnerCaller(() => Debug.LogError(message, context));
		}

		public static void LogErrorFormat(string format, params object[] args)
		{
			InnerCaller(() => Debug.LogErrorFormat(format, args));
		}

		public static void LogErrorFormat(Object2 context, string format, params object[] args)
		{
			InnerCaller(() => Debug.LogErrorFormat(context, format, args));
		}

		public static void LogWarning(object message)
		{
			InnerCaller(() => Debug.LogWarning(message));
		}
		public static void LogWarning(object message, Object2 context)
		{
			InnerCaller(() => Debug.LogWarning(message, context));
		}

		public static void LogWarningFormat(string format, params object[] args)
		{
			InnerCaller(() => Debug.LogWarningFormat(format, args));
		}

		public static void LogWarningFormat(Object2 context, string format, params object[] args)
		{
			InnerCaller(() => Debug.LogWarningFormat(context, format, args));
		}

		public static void Assert(bool condition)
		{
			InnerCaller(() => Debug.Assert(condition));
		}

		public static void Assert(bool condition, Object2 context)
		{
			InnerCaller(() => Debug.Assert(condition, context));
		}

		public static void Assert(bool condition, object message)
		{
			InnerCaller(() => Debug.Assert(condition, message));
		}

		public static void Assert(bool condition, string message)
		{
			InnerCaller(() => Debug.Assert(condition, message));
		}

		public static void Assert(bool condition, object message, Object2 context)
		{
			InnerCaller(() => Debug.Assert(condition, message, context));
		}

		public static void Assert(bool condition, string message, Object2 context)
		{
			InnerCaller(() => Debug.Assert(condition, message, context));
		}

		public static void AssertFormat(bool condition, string format, params object[] args)
		{
			InnerCaller(() => Debug.AssertFormat(condition, format, args));
		}

		public static void AssertFormat(bool condition, Object2 context, string format, params object[] args)
		{
			InnerCaller(() => Debug.AssertFormat(condition, context, format, args));
		}

		public static void LogAssertion(object message)
		{
			InnerCaller(() => Debug.LogAssertion(message));
		}

		public static void LogAssertion(object message, Object2 context)
		{
			InnerCaller(() => Debug.LogAssertion(message, context));
		}

		public static void LogAssertionFormat(string format, params object[] args)
		{
			InnerCaller(() => Debug.LogAssertionFormat(format, args));
		}

		public static void LogAssertionFormat(Object2 context, string format, params object[] args)
		{
			InnerCaller(() => Debug.LogAssertionFormat(context, format, args));
		}

		public static void LogException(Exception exception)
		{
			InnerCaller(() => Debug.LogException(exception));
		}
	}
}
