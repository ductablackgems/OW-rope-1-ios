using UnityEngine;

namespace MagicalFXMagicalEffect
{
	public class FX_Rotation : MonoBehaviour
	{
		public Vector3 Speed = Vector3.up;

		private void Start()
		{
		}

		private void FixedUpdate()
		{
			base.transform.Rotate(Speed);
		}
	}
}
