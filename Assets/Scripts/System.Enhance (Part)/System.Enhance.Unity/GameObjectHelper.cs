using System.Collections.Generic;
using UnityEngine;
using Resources2 = UnityEngine.Resources;
using UnityEditor;
using System.Text;
using System.Linq;
using Object2 = UnityEngine.Object;

namespace System.Enhance.Unity
{
	public static class GameObjectHelper
	{
		public static List<GameObject> FindAllIncludingInactive()
		{
			var list = new List<GameObject>();
			foreach (var go in Resources2.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
			{
#if !UNITY_EDITOR
				if (go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave)
					continue;
				list.Add(go);
#else
				if (!EditorUtility.IsPersistent(go.transform.root.gameObject))
				{
					list.Add(go);
				}
#endif
			}
			return list;
		}
		
		public static GameObject FindIncludesInactive(string nameOrPath)
		{
			var list = FindAllIncludingInactive();
			return list.FirstOrDefault(go => go.name == nameOrPath || go.GetPath() == nameOrPath);
		}

		public static string GetPath(this Transform transform)
		{
			var pathBuilder = new StringBuilder(transform.name);
			while (transform.parent != null)
			{
				transform = transform.parent;
				pathBuilder.Insert(0, transform.name + "/");
			}
			return pathBuilder.ToString();
		}

		public static string GetPath(this GameObject go)
		{
			return go.transform.GetPath();
		}

		public static int DestroyAllChildren(this GameObject go)
		{
			return go.transform.DestroyAllChildren();
		}
		
		public static int DestroyAllChildren(this Transform transform)
		{
			int count = 0;
			for (int i = transform.childCount - 1; i >= 0; i--)
			{
				var child = transform.GetChild(i);
				if (Application.isPlaying)
				{
					Object2.Destroy(child.gameObject);
				}
				else
				{
					Object2.DestroyImmediate(child.gameObject);
				}
				count++;
			}
			return count;
		}

		// predicate用物体名来筛选需要删除的物体
		public static int DestroyAllChildren(this Transform transform, Predicate<string> predicate)
		{
			int count = 0;
			for (int i = transform.childCount - 1; i >= 0; i--)
			{
				var child = transform.GetChild(i);
				if (!predicate(child.name)) continue;
				if (Application.isPlaying)
				{
					Object2.Destroy(child.gameObject);
				}
				else
				{
					Object2.DestroyImmediate(child.gameObject);
				}
				count++;
			}
			return count;
		}

		public static int DestroyAllChildren(this GameObject go, Predicate<string> predicate)
		{
			return go.transform.DestroyAllChildren(predicate);
		}

		public static bool RemoveComponent<T>(this GameObject go) where T : Component
		{
			try
			{
				var component = go.GetComponent<T>();
				if (component == null) return false;
#if UNITY_EDITOR
				if (Application.isPlaying)
				{
#endif
					Object2.Destroy(component);
#if UNITY_EDITOR
				}
				else
				{
					Object2.DestroyImmediate(component);
				}
#endif
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
