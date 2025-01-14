using UnityEngine;

namespace MagicalFXMagicalEffect
{
	public class FX_RandomRotation : MonoBehaviour
	{
		public Vector3 Rotation;

		private void Start()
		{
			base.transform.Rotate(new Vector3(UnityEngine.Random.Range(0f - Rotation.x, Rotation.x), UnityEngine.Random.Range(0f - Rotation.y, Rotation.y), UnityEngine.Random.Range(0f - Rotation.z, Rotation.z)));
		}
	}
}
