using System.Collections.Generic;
using UnityEngine;

namespace App.Vehicles.Car.Navigation
{
	public class TrafficTesterNode : MonoBehaviour
	{
		private const float MovingVehicleTreshold = 2f;

		private int layerMask = 7168;

		private HashSet<Transform> checkedTransforms = new HashSet<Transform>();

		public bool HitObstacle
		{
			get;
			private set;
		}

		public bool HitVehicle
		{
			get;
			private set;
		}

		public bool HitMovingVehicle
		{
			get;
			private set;
		}

		public void ResetStates()
		{
			HitObstacle = false;
			HitVehicle = false;
			HitMovingVehicle = false;
			checkedTransforms.Clear();
		}

		public void FakeMovingVehicleHit()
		{
			HitObstacle = true;
			HitVehicle = true;
			HitMovingVehicle = true;
			base.gameObject.SetActive(value: false);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (HitMovingVehicle || ((1 << other.gameObject.layer) & layerMask) == 0)
			{
				return;
			}
			HitObstacle = true;
			WhoIsResult whoIsResult = WhoIs.Resolve(other, WhoIs.Entities.Vehicle, checkedTransforms);
			if (!whoIsResult.IsEmpty)
			{
				HitVehicle = true;
				if (whoIsResult.transform.GetComponentSafe<Rigidbody>().velocity.magnitude > 2f)
				{
					HitMovingVehicle = true;
					base.gameObject.SetActive(value: false);
				}
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = (HitObstacle ? Color.blue : Color.red);
			Matrix4x4 matrix4x = Matrix4x4.TRS(base.transform.position, base.transform.rotation, base.transform.lossyScale * 1.01f);
			Matrix4x4 matrix = Gizmos.matrix;
			Gizmos.matrix *= matrix4x;
			Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
			Gizmos.matrix = matrix;
		}
	}
}
