using UnityEngine;

namespace Forge3D
{
	public class F3DRift : MonoBehaviour
	{
		public float RotationSpeed;

		public float MorphSpeed;

		public float MorphFactor;

		private Vector3 dScale;

		private void Start()
		{
			dScale = base.transform.localScale;
		}

		private void Update()
		{
			base.transform.rotation = base.transform.rotation * Quaternion.Euler(0f, 0f, RotationSpeed * Time.deltaTime);
			base.transform.localScale = new Vector3(dScale.x, dScale.y, dScale.z + Mathf.Sin(Time.time * MorphSpeed) * MorphFactor);
		}
	}
}
