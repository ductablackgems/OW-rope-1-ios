using UnityEngine;

namespace App
{
	public class WhoIsInitializer : MonoBehaviour
	{
		private void Awake()
		{
			WhoIs.Initialize();
		}
	}
}
