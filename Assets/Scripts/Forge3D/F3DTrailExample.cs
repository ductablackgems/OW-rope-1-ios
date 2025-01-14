using UnityEngine;

namespace Forge3D
{
	public class F3DTrailExample : MonoBehaviour
	{
		public float Mult;

		public float TimeMult;

		private Vector3 defaultPos;

		private void Start()
		{
			defaultPos = base.transform.position;
		}

		private void Update()
		{
			base.transform.position = defaultPos + new Vector3(Mathf.Sin(Time.time * TimeMult) * Mult, 0f, Mathf.Cos(Time.time * TimeMult) * Mult);
		}
	}
}
