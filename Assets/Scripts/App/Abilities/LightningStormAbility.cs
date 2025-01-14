using App.Player;
using App.Player.SkeletonEffect;
using App.Util;
using App.Vehicles;
using System.Collections.Generic;
using UnityEngine;

namespace App.Abilities
{
	public class LightningStormAbility : Ability
	{
		private const float HumanHeatSpeed = 0.5f;

		private const float VehicleDamagePerSec = 100f;

		private const float LifeCheckIntrval = 0.5f;

		[Header("Damage")]
		[Tooltip("Procentualni DMG za vterinu")]
		[SerializeField]
		private float DamagePercent = 0.25f;

		[Header("Effect")]
		[SerializeField]
		private GameObject effect;

		private HashSet<Transform> checkedTransforms = new HashSet<Transform>();

		private DurationTimer lifeCheckTimer = new DurationTimer(useFixedTime: true);

		private bool hasTarget;

		private RagdollHelper ragdollHelper;

		private SphereCollider targetDetector;

		public void ActivateEffect()
		{
			effect.SetActive(value: true);
			lifeCheckTimer.Run(0.5f);
			targetDetector.enabled = true;
		}

		protected override void OnInitialized()
		{
			base.OnInitialized();
			ragdollHelper = base.Owner.GetComponent<RagdollHelper>();
			effect.SetActive(value: false);
		}

		protected override void OnActivated(Vector3 position)
		{
			base.OnActivated(position);
			effect.SetActive(value: false);
			ragdollHelper.OnRaggdolled += OnPlayerRagdollEvent;
		}

		protected override void OnDeactivated()
		{
			base.OnDeactivated();
			effect.SetActive(value: false);
			targetDetector.enabled = false;
			ragdollHelper.OnRaggdolled -= OnPlayerRagdollEvent;
		}

		private void LateUpdate()
		{
			if (base.IsRunning && targetDetector.enabled && lifeCheckTimer.Done())
			{
				if (hasTarget)
				{
					lifeCheckTimer.Run(0.5f);
					hasTarget = false;
				}
				else
				{
					Deactivate();
				}
			}
		}

		private void Awake()
		{
			targetDetector = GetComponent<SphereCollider>();
			targetDetector.radius = Radius;
			targetDetector.enabled = false;
		}

		private void FixedUpdate()
		{
			checkedTransforms.Clear();
		}

		private void OnTriggerStay(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				return;
			}
			WhoIsResult whoIsResult = WhoIs.Resolve(other, WhoIs.Masks.Flamethrower, checkedTransforms);
			if (whoIsResult.IsEmpty || whoIsResult.gameObject == base.Owner)
			{
				return;
			}
			if (whoIsResult.Compare(WhoIs.Masks.AllRagdollableHumans))
			{
				GameObject gameObject = whoIsResult.gameObject;
				Health componentSafe = gameObject.GetComponentSafe<Health>();
				float damage = componentSafe.maxHealth * DamagePercent * Time.fixedDeltaTime;
				RagdollHelper component = gameObject.GetComponent<RagdollHelper>();
				if (component != null)
				{
					component.Ragdolled = true;
				}
				HumanFireManager component2 = gameObject.GetComponent<HumanFireManager>();
				if (component2 != null)
				{
					component2.StartElectricity(base.Owner, damage);
				}
				ReportTarget(componentSafe);
			}
			else
			{
				if (!whoIsResult.Compare(WhoIs.Masks.AllVehicles))
				{
					return;
				}
				Health component3 = whoIsResult.gameObject.GetComponent<Health>();
				if (component3 != null)
				{
					if (!whoIsResult.Compare(WhoIs.Entities.Vehicle))
					{
						float fixedDeltaTime = Time.fixedDeltaTime;
					}
					else
					{
						float fixedDeltaTime2 = Time.fixedDeltaTime;
					}
					component3.ApplyDamage(Time.fixedDeltaTime * 100f, 0, base.Owner);
				}
				VehicleComponents component4 = whoIsResult.gameObject.GetComponent<VehicleComponents>();
				if (component4 == null)
				{
					return;
				}
				GameObject personByCollider = component4.GetPersonByCollider(other);
				if (personByCollider != null)
				{
					HumanFireManager component5 = personByCollider.GetComponent<HumanFireManager>();
					if (component5 != null)
					{
						component5.Heat(Time.fixedDeltaTime * 0.5f, base.Owner);
					}
				}
			}
		}

		private void OnPlayerRagdollEvent(bool isRagdoll)
		{
			if (isRagdoll)
			{
				Deactivate();
			}
		}

		private void ReportTarget(Health health)
		{
			if (!(health == null) && health.GetCurrentHealth() > 0f)
			{
				hasTarget = true;
			}
		}
	}
}
