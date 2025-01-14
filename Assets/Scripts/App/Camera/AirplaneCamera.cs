using UnityEngine;

namespace App.Camera
{
	public class AirplaneCamera : MonoBehaviour
	{
		[SerializeField]
		private float smooth = 10f;

		public Transform Target
		{
			get;
			set;
		}

		private void FixedUpdate()
		{
			base.transform.position = Vector3.Lerp(base.transform.position, Target.position, Time.fixedDeltaTime * smooth);
			base.transform.forward = Vector3.Lerp(base.transform.forward, Target.forward, Time.fixedDeltaTime * smooth);
		}
	}
}
