//#if UNITY_EDITOR
//using UnityEditor;

//namespace System.Enhance.Unity
//{
//	[CustomEditor(typeof(VideoPlayerPlus))]
//	public class VideoPlayerPlusEditor : Editor
//	{
//		public override void OnInspectorGUI()
//		{
//			serializedObject.Update();

//			var targetVP = target as VideoPlayerPlus;
			
//			var properties = serializedObject.Prop

//			var propEnableClearColor = serializedObject.FindProperty("enableClearColor");
//			var propColorClear = serializedObject.FindProperty("colorClear");

//			string currLangCode = EditorPrefs.GetString("CurrentLanguage", "en");

//			if (currLangCode.InValues("zh", "zh-Hans", "zh-Hant", "zh-CN", "zh-HK", "zh-TW", "zh-MO", "zh-SG"))
//			{
//				currLangCode = "zh";
//			}
//			else if (currLangCode.InValues("ja", "ja-JP"))
//			{
//				currLangCode = "ja";
//			}
//			else
//			{
//				currLangCode = "en";
//			}

//			// 如果enableClearColor为false时，在enableColorClear上方添加一个warning
//			if (!propEnableClearColor.boolValue)
//			{
//				if (currLangCode == "zh")
//				{
//					EditorGUILayout.HelpBox("禁用clearColor将导致视频以\"仅深度\"模式渲染，这可能会导致意外的渲染效果。\n" +
//											"强烈建议启用clearColor，即使其值为Color.clear。", MessageType.Warning);
//				}
//				else if (currLangCode == "ja")
//				{
//					EditorGUILayout.HelpBox("clearColorを無効にすると、ビデオが深度のみでレンダリングされ、予期しないレンダリング効果が発生する可能性があります。\n" +
//											"clearColorを有効にすることを強くお勧めします。", MessageType.Warning);
//				}
//				else
//				{
//					EditorGUILayout.HelpBox("Disabling clearColor will cause the video to be rendered in depth-only mode, which may cause unexpected rendering effects.\n" +
//											"It is strongly recommended to enable clearColor, even if its value is Color.clear.", MessageType.Warning);
//				}
//			}

//			// 只在enableClearColor为true时在inspector中显示colorClear
//			EditorGUILayout.PropertyField(propEnableClearColor);
//			if (propEnableClearColor.boolValue)
//			{
//				EditorGUILayout.PropertyField(propColorClear);
//			}

//			serializedObject.ApplyModifiedProperties();
//		}
//	}
//}
//#endif