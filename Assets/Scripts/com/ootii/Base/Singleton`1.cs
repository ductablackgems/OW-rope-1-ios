using UnityEngine;

namespace com.ootii.Base
{
	public class Singleton<T> : MonoBehaviour where T : Component
	{
		private static T m_Instance;

		public static T Instance
		{
			get
			{
				if ((Object)m_Instance == (Object)null)
				{
					m_Instance = UnityEngine.Object.FindObjectOfType<T>();
					if ((Object)m_Instance == (Object)null)
					{
						m_Instance = new GameObject
						{
							name = typeof(T).Name
						}.AddComponent<T>();
					}
				}
				return m_Instance;
			}
		}

		public virtual void Awake()
		{
			if ((Object)m_Instance == (Object)null)
			{
				m_Instance = (this as T);
				Object.DontDestroyOnLoad(base.gameObject);
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
}
