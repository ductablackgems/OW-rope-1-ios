using UnityEngine;

namespace MagicalFXMagicalEffect
{
	public class FX_ShakeCamera : MonoBehaviour
	{
		public Vector3 Power = Vector3.up;

		public float ShakeRate;

		private float timeTmp;

		private void Start()
		{
			timeTmp = Time.time;
			CameraEffect.Shake(Power);
		}

		private void Update()
		{
			if (ShakeRate > 0f && Time.time > timeTmp + ShakeRate)
			{
				timeTmp = Time.time;
				CameraEffect.Shake(Power);
			}
		}
	}
}
