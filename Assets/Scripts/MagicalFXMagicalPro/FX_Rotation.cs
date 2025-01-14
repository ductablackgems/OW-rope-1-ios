using UnityEngine;

namespace MagicalFXMagicalPro
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
