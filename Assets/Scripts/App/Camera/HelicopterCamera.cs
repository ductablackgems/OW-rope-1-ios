using UnityEngine;

namespace App.Camera
{
	public class HelicopterCamera : MonoBehaviour
	{
		public float smooth = 10f;

		public Transform targetPosition;

		private void FixedUpdate()
		{
			base.transform.position = Vector3.Lerp(base.transform.position, targetPosition.position, Time.fixedDeltaTime * smooth);
			base.transform.forward = Vector3.Lerp(base.transform.forward, targetPosition.forward, Time.fixedDeltaTime * smooth);
		}
	}
}
