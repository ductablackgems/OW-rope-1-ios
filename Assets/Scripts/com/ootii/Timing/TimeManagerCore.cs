using System.Collections;
using UnityEngine;

namespace com.ootii.Timing
{
	public class TimeManagerCore : MonoBehaviour
	{
		private void Awake()
		{
			Object.DontDestroyOnLoad(base.gameObject);
			TimeManager.Initialize();
		}

		public IEnumerator Start()
		{
			WaitForEndOfFrame lWaitForEndOfFrame = new WaitForEndOfFrame();
			while (true)
			{
				yield return lWaitForEndOfFrame;
				TimeManager.Update();
			}
		}
	}
}
