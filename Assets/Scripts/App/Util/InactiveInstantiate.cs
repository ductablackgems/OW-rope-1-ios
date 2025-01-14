using UnityEngine;

namespace App.Util
{
	public class InactiveInstantiate : MonoBehaviour
	{
		private Transform inactiveParent;

		private bool initialized;

		public GameObject InstantiateInactive(GameObject original)
		{
			if (!initialized)
			{
				Init();
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(original, inactiveParent);
			gameObject.SetActive(value: false);
			gameObject.transform.parent = null;
			return gameObject;
		}

		protected void Awake()
		{
			if (!initialized)
			{
				Init();
			}
		}

		private void Init()
		{
			initialized = true;
			GameObject gameObject = new GameObject("~InactiveParent~");
			gameObject.SetActive(value: false);
			inactiveParent = gameObject.transform;
		}
	}
}
