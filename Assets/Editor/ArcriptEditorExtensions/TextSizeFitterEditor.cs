using UnityEngine;
using UnityEditor;

namespace System.Enhance.Unity
{
	[CustomEditor(typeof(TextSizeFitter))]
	public class TextSizeFitterEditor : Editor
	{
		private TextSizeFitter currentTSF;

		private void OnEnable()
		{
			currentTSF = target as TextSizeFitter;
		}

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector(); // 可以看作相当于base.OnInspectorGUI()

			GUILayout.Space(20);

			var style = new GUIStyle(GUI.skin.button);
			style.alignment = TextAnchor.MiddleCenter;
			if (GUILayout.Button("Adjust Now", style, GUILayout.Width(100)))
			{
				currentTSF.Adjust();
			}
		}
	}
}
