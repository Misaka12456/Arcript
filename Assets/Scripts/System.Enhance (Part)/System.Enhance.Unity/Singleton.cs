using UnityEngine;
using UnityEngine.EventSystems;

namespace System.Enhance.Unity
{
	/// <summary>
	/// 单例模式的类型
	/// </summary>
	public enum SingletonPattern
	{
		Hungry, // 饿汉模式(在类加载时就初始化)
		Lazy, // 懒汉模式(在第一次使用时才初始化)
	}

	/// <summary>
	/// Represents a singleton. A singleton is a class that can only have one instance.
	/// </summary>
	/// <typeparam name="T">The type of the singleton. It should be the class of which you want to create a singleton (eq. itself).</typeparam>
	public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		/// <summary>
		/// The pattern of this singleton.
		/// </summary>
		public static SingletonPattern Pattern { get; } = SingletonPattern.Hungry; // 默认采用饿汉模式(Hungry Man Pattern)
		protected bool AllowRepeatInit { get; set; } = false; // 是否允许重复初始化(对于某些能够多次进入的场景，可能需要重复初始化)
																 // 仅在Pattern为SingletonPattern.Hungry时有效
		protected static T instance;

		/// <summary>
		/// Get the only instance of this singleton.
		/// </summary>
		public static T Instance
		{
			get
			{
				if (Pattern == SingletonPattern.Hungry)
				{
					return instance;
				}
				else
				{
					// Pattern == SingletonPattern.Lazy
					if (instance == null)
					{
						instance = FindObjectOfType<T>();
						if (instance == null)
						{
							var obj = new GameObject()
							{
								name = typeof(T).Name
							};
							instance = obj.AddComponent<T>();
						}
					}
					return instance;
				}
			}
		}

		protected virtual void Awake()
		{
			SingletonAwake();
			if (Pattern == SingletonPattern.Hungry)
			{
				if (instance == null)
				{
					instance = this as T;
					if (!AllowRepeatInit)
					{
						DontDestroyOnLoad(gameObject);
					}
				}
				else
				{
					if (AllowRepeatInit)
					{
						instance = null;
						instance = this as T;
					}
					else
					{
						Destroy(gameObject);
					}
				}
			}
		}

		/// <summary>
		/// Called when loading the singleton script.
		/// </summary>
		protected virtual void SingletonAwake() { }
	}
}
