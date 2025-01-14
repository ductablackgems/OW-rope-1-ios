using UnityEngine;

namespace Forge3D
{
	public class F3DDummyEvent : MonoBehaviour
	{
		private void Start()
		{
			BroadcastMessage("OnSpawned", SendMessageOptions.DontRequireReceiver);
		}
	}
}
