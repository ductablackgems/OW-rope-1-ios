using UnityEngine;

namespace App.Enemies.Vehicles.Helicopter
{
	public class HelicopterAim : MonoBehaviour
	{
		private const float MaxXTopAngle = 30f;

		private const float MaxXBottomAngle = 45f;

		private const float MaxYAngle = 30f;

		public Transform virtualPlayerTransform;

		public Vector3 angles;

		private Vector3 initialEuler;

		protected void Awake()
		{
			initialEuler = base.transform.localRotation.eulerAngles;
		}

		protected void Update()
		{
			angles = Quaternion.LookRotation(virtualPlayerTransform.localPosition - base.transform.localPosition).eulerAngles;
			if (angles.x < 330f && angles.x > 180f)
			{
				angles.x = 330f;
			}
			else if (angles.x > 45f && angles.x < 180f)
			{
				angles.x = 45f;
			}
			if (angles.y < 330f && angles.y > 180f)
			{
				angles.y = 330f;
			}
			else if (angles.y > 30f && angles.y < 180f)
			{
				angles.y = 30f;
			}
			base.transform.localRotation = Quaternion.Euler(angles.x, angles.y, initialEuler.z);
		}
	}
}
