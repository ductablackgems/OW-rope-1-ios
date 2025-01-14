using App.Player;
using App.Util;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace App.Vehicles.Bicycle
{
	public class StreetVehicleCrasher : MonoBehaviour
	{
		private const float CriticalDamage = 50f;

		public float minImpulseMultiplier = 1f;

		private StreetVehicleModesHelper streetVehicleModesHelper;

		private NavMeshAgent agent;

		private Rigidbody _rigidbody;

		private Health health;

		private GlideController playerGlideController;

		private Vector3 lastVelocity;

		private Vector3 previousVelocity;

		private DurationTimer preventCrashTimer = new DurationTimer();

		public VehicleComponents Components
		{
			get;
			private set;
		}

		public event Action OnCrash;

		public void PreventCrash(float duration)
		{
			preventCrashTimer.Run(duration);
		}

		public void Crash()
		{
			if (Components.driver != null)
			{
				RagdollHelper componentSafe = Components.driver.GetComponentSafe<RagdollHelper>();
				Components.KickOffCurrentDriver(relocateCharacter: false);
				componentSafe.SetRagdollVelocity(lastVelocity);
				_rigidbody.velocity = lastVelocity;
				if (this.OnCrash != null)
				{
					this.OnCrash();
				}
			}
			streetVehicleModesHelper.SetFreeState();
		}

		private void Awake()
		{
			streetVehicleModesHelper = this.GetComponentSafe<StreetVehicleModesHelper>();
			Components = this.GetComponentSafe<VehicleComponents>();
			agent = this.GetComponentSafe<NavMeshAgent>();
			_rigidbody = this.GetComponentSafe<Rigidbody>();
			health = this.GetComponentSafe<Health>();
			playerGlideController = ServiceLocator.GetGameObject("Player").GetComponentSafe<GlideController>();
			health.OnDamage += OnDamage;
		}

		private void OnDestroy()
		{
			health.OnDamage -= OnDamage;
		}

		private void FixedUpdate()
		{
			previousVelocity = lastVelocity;
			lastVelocity = ((streetVehicleModesHelper.Mode == StreetVehicleMode.AI) ? agent.velocity : _rigidbody.velocity);
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (!base.enabled || preventCrashTimer.InProgress())
			{
				return;
			}
			if (streetVehicleModesHelper.Mode != StreetVehicleMode.AI)
			{
				Vector3 vector = base.transform.InverseTransformDirection(collision.impulse);
				vector.y = 0f;
				float num = Mathf.Lerp(1f, minImpulseMultiplier, Vector3.Angle(base.transform.up, Vector3.up) / 130f);
				if (!(vector.magnitude > 800f * num) && (!collision.gameObject.CompareTag("Vehicle") || !(vector.magnitude > 600f * num)))
				{
					return;
				}
				if (Components.driver != null)
				{
					RagdollHelper componentSafe = Components.driver.GetComponentSafe<RagdollHelper>();
					Components.KickOffCurrentDriver(relocateCharacter: false);
					componentSafe.SetRagdollVelocity(lastVelocity);
					_rigidbody.velocity = lastVelocity;
					if (this.OnCrash != null)
					{
						this.OnCrash();
					}
				}
				streetVehicleModesHelper.SetFreeState();
			}
			else if (collision.gameObject.CompareTag("Vehicle"))
			{
				if (!((agent.velocity - collision.rigidbody.velocity).magnitude * 3.6f > 7f))
				{
					return;
				}
				if (Components.driver != null)
				{
					RagdollHelper componentSafe2 = Components.driver.GetComponentSafe<RagdollHelper>();
					Components.KickOffCurrentDriver(relocateCharacter: false);
					componentSafe2.SetRagdollVelocity(lastVelocity);
					_rigidbody.velocity = lastVelocity;
					if (this.OnCrash != null)
					{
						this.OnCrash();
					}
				}
				streetVehicleModesHelper.SetFreeState();
			}
			else if (WhoIs.CompareCollision(collision, WhoIs.Entities.Player) && playerGlideController.Crashable())
			{
				RagdollHelper componentSafe3 = Components.driver.GetComponentSafe<RagdollHelper>();
				Components.KickOffCurrentDriver(relocateCharacter: false);
				Vector3 vector2 = Vector3.Lerp(lastVelocity, collision.rigidbody.velocity, 0.5f);
				componentSafe3.SetRagdollVelocity(vector2);
				_rigidbody.velocity = vector2;
				if (this.OnCrash != null)
				{
					this.OnCrash();
				}
				streetVehicleModesHelper.SetFreeState();
			}
		}

		private void OnDamage(float damage, int damageType, GameObject agressor)
		{
			if (!(damage > 50f))
			{
				return;
			}
			Vector3 a = health.CompareTag("Player") ? Vector3.up : (base.transform.position - ServiceLocator.GetGameObject("Player").transform.position).normalized;
			if (Components.driver != null)
			{
				Health componentSafe = Components.driver.GetComponentSafe<Health>();
				Components.KickOffCurrentDriver(relocateCharacter: false);
				componentSafe.ApplyDamage(damage, damageType, agressor);
				componentSafe.GetComponentSafe<RagdollHelper>().SetRagdollVelocity(a * 15f);
				if (this.OnCrash != null)
				{
					this.OnCrash();
				}
			}
			_rigidbody.AddForce(a * 50f, ForceMode.Impulse);
		}
	}
}
