using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityObj = UnityEngine.Object;

namespace System.Enhance.Unity
{
	public static class SceneHelper
	{
		public static GameObject[] GetDontDestroyOnLoadGameObjects()
		{
			var allGameObjects = new List<GameObject>();
			allGameObjects.AddRange(UnityObj.FindObjectsOfType<GameObject>());
			//移除所有场景包含的对象
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				var scene = SceneManager.GetSceneAt(i);
				var objs = scene.GetRootGameObjects();
				for (int j = 0; j < objs.Length; j++)
				{
					allGameObjects.Remove(objs[j]);
				}
			}
			//移除父级不为null的对象
			int k = allGameObjects.Count;
			while (--k >= 0)
			{
				if (allGameObjects[k].transform.parent != null)
				{
					allGameObjects.RemoveAt(k);
				}
			}
			return allGameObjects.ToArray();
		}
	}
}
