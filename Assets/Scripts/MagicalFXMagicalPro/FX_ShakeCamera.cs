using UnityEngine;

namespace MagicalFXMagicalPro
{
	public class FX_ShakeCamera : MonoBehaviour
	{
		public Vector3 Power = Vector3.up;

		private void Start()
		{
			CameraEffect.Shake(Power);
		}
	}
}
