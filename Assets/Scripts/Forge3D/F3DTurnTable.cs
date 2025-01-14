using UnityEngine;

namespace Forge3D
{
	public class F3DTurnTable : MonoBehaviour
	{
		public float speed;

		private void Start()
		{
		}

		private void Update()
		{
			base.transform.rotation = base.transform.rotation * Quaternion.Euler(0f, speed * Time.deltaTime, 0f);
		}
	}
}
