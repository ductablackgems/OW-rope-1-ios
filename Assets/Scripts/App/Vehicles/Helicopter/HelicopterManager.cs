using App.Player;
using App.Util;
using UnityEngine;

namespace App.Vehicles.Helicopter
{
	public class HelicopterManager : MonoBehaviour
	{
		public bool active;

		public float minY;

		public float landDistance = 30f;

		public float raisingSpeed = 2f;

		public float landForce;

		public float landDuration = 3f;

		public GameObject areaCollider;

		public GameObject propelerBlur;

		public RotateObject propelerRotator;

		private Rigidbody _rigidbody;

		private Health health;

		private HelicopterController controller;

		private TiltController tiltController;

		private GameObject groundCollider;

		private bool raising;

		private bool landing;

		private DurationTimer landingStopTimer = new DurationTimer();

		private AudioSource rotorSound;

		public bool CanLand()
		{
			if (active && !raising && !landing && !health.Dead())
			{
				return HitLand();
			}
			return false;
		}

		public void Land()
		{
			if (CanLand())
			{
				landing = true;
				areaCollider.SetActive(value: false);
				groundCollider.SetActive(value: false);
				landingStopTimer.Run(landDuration);
			}
		}

		private bool HitLand()
		{
			RaycastHit hitInfo;
			return Physics.Raycast(new Ray(base.transform.position, Vector3.down), out hitInfo, landDistance, 1057793);
		}

		private void Awake()
		{
			_rigidbody = this.GetComponentSafe<Rigidbody>();
			health = this.GetComponentInChildrenSafe<Health>();
			controller = this.GetComponentSafe<HelicopterController>();
			tiltController = this.GetComponentInChildrenSafe<TiltController>();
			groundCollider = ServiceLocator.GetGameObject("HeliGroundCollider");
			rotorSound = this.GetComponentSafe<AudioSource>();
		}

		private void Update()
		{
			if (health.Dead())
			{
				areaCollider.SetActive(value: false);
				return;
			}
			if (active && !propelerRotator.enabled)
			{
				raising = true;
				if (!rotorSound.isPlaying)
				{
					rotorSound.Play();
				}
			}
			if (!landing)
			{
				areaCollider.SetActive(active);
			}
			propelerBlur.SetActive(active);
			propelerRotator.enabled = active;
			propelerRotator.GetComponent<Renderer>().enabled = !active;
			tiltController.enabled = active;
			if (!active)
			{
				raising = false;
				controller.enabled = false;
				_rigidbody.isKinematic = true;
				if (rotorSound.isPlaying)
				{
					rotorSound.Stop();
				}
			}
			if (landingStopTimer.Done())
			{
				landingStopTimer.Stop();
				active = false;
				landing = false;
			}
		}

		private void FixedUpdate()
		{
			if (health.Dead())
			{
				return;
			}
			if (raising)
			{
				if (minY <= base.transform.position.y)
				{
					raising = false;
					controller.enabled = active;
					_rigidbody.isKinematic = false;
					groundCollider.SetActive(value: true);
				}
				else
				{
					base.transform.position = Vector3.MoveTowards(base.transform.position, new Vector3(base.transform.position.x, minY, base.transform.position.z), Time.fixedDeltaTime * raisingSpeed);
				}
			}
			else if (landing)
			{
				_rigidbody.AddForce(Vector3.down * landForce);
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.DrawLine(base.transform.position, base.transform.position + Vector3.down * landDistance);
		}
	}
}
