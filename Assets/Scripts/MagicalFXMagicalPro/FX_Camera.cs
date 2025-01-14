using UnityEngine;

namespace MagicalFXMagicalPro
{
	public class FX_Camera : MonoBehaviour
	{
		private Vector3 positionTemp;

		private Vector3 forcePower;

		private void Start()
		{
			CameraEffect.CameraFX = this;
			positionTemp = base.transform.position;
		}

		public void Shake(Vector3 power)
		{
			forcePower = -power;
		}

		private void Update()
		{
			forcePower = Vector3.Lerp(forcePower, Vector3.zero, Time.deltaTime * 5f);
			base.transform.position = positionTemp + new Vector3(Mathf.Cos(Time.time * 80f) * forcePower.x, Mathf.Cos(Time.time * 80f) * forcePower.y, Mathf.Cos(Time.time * 80f) * forcePower.z);
		}
	}
}
