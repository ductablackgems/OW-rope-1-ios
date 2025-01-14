using App.Player;
using UnityEngine;

namespace App.Vehicles.Motorbike
{
	public class MotorbikeCrasher : MonoBehaviour
	{
		public float commonCrashImpulse = 5000f;

		public float vehicleCrashImpulse = 4000f;

		private Rigidbody _rigidbody;

		private Vector3 lastVelocity;

		public VehicleComponents Components
		{
			get;
			private set;
		}

		private void Awake()
		{
			Components = this.GetComponentSafe<VehicleComponents>();
			_rigidbody = this.GetComponentSafe<Rigidbody>();
		}

		private void FixedUpdate()
		{
			lastVelocity = _rigidbody.velocity;
		}

		private void OnCollisionEnter(Collision collision)
		{
			Vector3 vector = base.transform.InverseTransformDirection(new Vector3(collision.impulse.x, 0f, collision.impulse.z));
			if ((vector.magnitude > commonCrashImpulse || (collision.collider.CompareTag("Vehicle") && vector.magnitude > vehicleCrashImpulse)) && Components.driver != null)
			{
				RagdollHelper componentSafe = Components.driver.GetComponentSafe<RagdollHelper>();
				Components.KickOffCurrentDriver(relocateCharacter: false);
				componentSafe.SetRagdollVelocity(lastVelocity);
			}
		}
	}
}
