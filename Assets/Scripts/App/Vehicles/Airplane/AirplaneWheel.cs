using App.Util;
using UnityEngine;

namespace App.Vehicles.Airplane
{
	public class AirplaneWheel : MonoBehaviour
	{
		private int layer;

		public Collider Collider
		{
			get;
			private set;
		}

		public WheelCollider WheelCollider
		{
			get;
			private set;
		}

		private void Start()
		{
			Collider = GetComponent<Collider>();
			WheelCollider = GetComponent<WheelCollider>();
			layer = LayerMask.GetMask("Ground");
		}

		public bool IsGrounded()
		{
			return !PhysicsUtils.IsValidPosition(base.transform.position, 1f, layer);
		}
	}
}
