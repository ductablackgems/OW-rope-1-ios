using UnityEngine;

namespace MagicalFXMagicalPro
{
	public class FX_MoverRandom : MonoBehaviour
	{
		public float Speed = 1f;

		public Vector3 Noise = Vector3.zero;

		private void Start()
		{
		}

		private void FixedUpdate()
		{
			base.transform.position += base.transform.forward * Speed * Time.fixedDeltaTime;
			base.transform.position += new Vector3(UnityEngine.Random.Range(0f - Noise.x, Noise.x), UnityEngine.Random.Range(0f - Noise.y, Noise.y), UnityEngine.Random.Range(0f - Noise.z, Noise.z)) * Time.fixedDeltaTime;
		}
	}
}
