using App.Util;
using App.Vehicles;
using FluffyUnderware.Curvy;
using UnityEngine;

namespace App.Player
{
	public class HelicopterCrashAnimation : MonoBehaviour
	{
		private const float ExplosionImpuls = 5f;

		public float rigidbodyDrag = 0.1f;

		public float rigidbodyAngularDrag = 0.1f;

		public float explosionDelay = 5f;

		public GameObject rotorFireGO;

		private Rigidbody _rigidbody;

		private AudioSource audioSource;

		private Health health;

		private IExplosion explosion;

		private VehicleComponents vehicleComponents;

		private WreckDestroyer wreckDestroyer;

		private DurationTimer explosionTimer = new DurationTimer();

		private bool running;

		private bool exploded;

		public bool Running()
		{
			return running;
		}

		protected void Awake()
		{
			_rigidbody = this.GetComponentSafe<Rigidbody>();
			audioSource = this.GetComponentSafe<AudioSource>();
			health = this.GetComponentInChildrenSafe<Health>();
			explosion = this.GetComponentSafe<IExplosion>();
			vehicleComponents = GetComponent<VehicleComponents>();
			wreckDestroyer = GetComponentInChildren<WreckDestroyer>(includeInactive: true);
			wreckDestroyer.enabled = false;
		}

		protected void Update()
		{
			if (!running && health.GetCurrentHealth() == 0f)
			{
				running = true;
				FollowSpline component = GetComponent<FollowSpline>();
				if (component != null)
				{
					component.enabled = false;
				}
				if (base.tag == "Player")
				{
					ServiceLocator.GetGameObject("HeliGroundCollider").SetActive(value: false);
				}
				rotorFireGO.SetActive(value: true);
				_rigidbody.useGravity = true;
				_rigidbody.drag = rigidbodyDrag;
				_rigidbody.angularDrag = rigidbodyAngularDrag;
				_rigidbody.AddRelativeTorque(base.transform.up * 100f, ForceMode.Acceleration);
				_rigidbody.freezeRotation = false;
				if (vehicleComponents != null)
				{
					vehicleComponents.handleTrigger.gameObject.SetActive(value: false);
				}
				explosionTimer.Run(explosionDelay);
			}
			if (explosionTimer.Done())
			{
				explosionTimer.Stop();
				Explode();
			}
		}

		private void Explode()
		{
			exploded = true;
			if (vehicleComponents != null)
			{
				Transform driver = vehicleComponents.driver;
				vehicleComponents.KickOffCurrentDriver();
				if (driver != null)
				{
					driver.GetComponentSafe<Health>().Kill();
				}
			}
			explosion.Explode();
			wreckDestroyer.enabled = true;
			audioSource.enabled = false;
		}

		protected void OnCollisionEnter(Collision collision)
		{
			if (running && !exploded && collision.impulse.magnitude / _rigidbody.mass > 5f)
			{
				explosionTimer.Stop();
				Explode();
			}
		}
	}
}
