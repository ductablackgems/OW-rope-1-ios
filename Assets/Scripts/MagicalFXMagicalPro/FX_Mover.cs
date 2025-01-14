using UnityEngine;

namespace MagicalFXMagicalPro
{
	[RequireComponent(typeof(Rigidbody))]
	public class FX_Mover : MonoBehaviour
	{
		public float Speed = 1f;

		public Vector3 Noise = Vector3.zero;

		public float Damping = 0.3f;

		private Quaternion direction;

		private void Start()
		{
			direction = Quaternion.LookRotation(base.transform.forward * 1000f);
			base.transform.Rotate(new Vector3(UnityEngine.Random.Range(0f - Noise.x, Noise.x), UnityEngine.Random.Range(0f - Noise.y, Noise.y), UnityEngine.Random.Range(0f - Noise.z, Noise.z)));
		}

		private void LateUpdate()
		{
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, direction, Damping);
			base.transform.position += base.transform.forward * Speed * Time.deltaTime;
		}
	}
}
