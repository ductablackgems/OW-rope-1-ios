using App.Player.Definition;
using App.Util;
using App.Vehicles;
using UnityEngine;

namespace App.Player
{
	[RequireComponent(typeof(PlayerAnimatorHandler))]
	public class FightController : MonoBehaviour, ICharacterModule
	{
		public float stayFightingDuration = 2f;

		public float punchDamage = 10f;

		public float kickDamage = 100f;

		private PlayerAnimatorHandler animatorHandler;

		private Animator animator;

		private AttackZone attackZone;

		private DurationTimer fightTimer = new DurationTimer();

		public Transform kickPosition;

		public AudioSource zdrojZvuku;

		public AudioClip[] klipy;

		private Vector3 attackZoneCurrentPos;

		private Vector3 attackZoneKickScale = new Vector3(3f, 3f, 3f);

		private Vector3 attackZoneNormalScale = new Vector3(1.4f, 1.4f, 1.4f);

		private ShotController shotController;

		public void Run()
		{
			fightTimer.Run(stayFightingDuration);
			animatorHandler.Fight = true;
		}

		public bool Running()
		{
			return animatorHandler.Fight;
		}

		public void Stop()
		{
			animatorHandler.Fight = false;
			animatorHandler.Punch = false;
			fightTimer.Stop();
		}

		public void OnPunch()
		{
			if (attackZone.IsIn)
			{
				if (zdrojZvuku != null)
				{
					zdrojZvuku.PlayOneShot(klipy[Random.Range(0, klipy.Length)]);
				}
				attackZone.TargetHealth.ApplyDamage(punchDamage, 1);
			}
		}

		public void OnKick()
		{
			attackZone.gameObject.transform.localPosition = kickPosition.localPosition;
			attackZone.gameObject.transform.localScale = attackZoneKickScale;
			zdrojZvuku.PlayOneShot(klipy[Random.Range(0, klipy.Length)]);
			if (!attackZone.IsIn)
			{
				return;
			}
			if (attackZone.TargetHealth.CompareTag("Bicycle") || attackZone.TargetHealth.CompareTag("Gyroboard") || attackZone.TargetHealth.CompareTag("Skateboard"))
			{
				VehicleComponents component = attackZone.TargetHealth.GetComponent<VehicleComponents>();
				if (component.driver != null)
				{
					RagdollHelper componentSafe = component.driver.GetComponentSafe<RagdollHelper>();
					component.KickOffCurrentDriver(relocateCharacter: false);
					Vector3 a = componentSafe.transform.position - base.transform.position;
					a.y = 0f;
					a.Normalize();
					componentSafe.SetRagdollVelocity(a * 20f);
					componentSafe.GetComponentSafe<Health>().ApplyDamage(kickDamage, 1);
				}
				return;
			}
			attackZone.TargetHealth.SendMessage("AddForceVehicle", 10000f, SendMessageOptions.DontRequireReceiver);
			if (attackZone.TargetHealth.GetComponent<RagdollHelper>() != null)
			{
				attackZone.TargetHealth.GetComponent<RagdollHelper>().AddForceBodyLife(100f);
			}
			if (attackZone.TargetHealth.GetComponent<VehicleExplosion>() != null)
			{
				VehicleComponents componentSafe2 = attackZone.TargetHealth.GetComponentSafe<VehicleComponents>();
				attackZone.TargetHealth.ApplyDamage(kickDamage * 1.5f, 1);
				if (componentSafe2.type == VehicleType.Bike && componentSafe2.driver != null)
				{
					RagdollHelper componentSafe3 = componentSafe2.driver.GetComponentSafe<RagdollHelper>();
					componentSafe2.KickOffCurrentDriver(relocateCharacter: false);
					Vector3 a2 = componentSafe3.transform.position - base.transform.position;
					a2.y = 0f;
					a2.Normalize();
					componentSafe3.SetRagdollVelocity(a2 * 20f);
					componentSafe3.GetComponentSafe<Health>().ApplyDamage(kickDamage, 1);
				}
			}
			else
			{
				attackZone.TargetHealth.ApplyDamage(kickDamage, 1);
			}
		}

		public void OffKick()
		{
			attackZone.gameObject.transform.localPosition = attackZoneCurrentPos;
			attackZone.gameObject.transform.localScale = attackZoneNormalScale;
		}

		public void Control(bool attackPressed)
		{
			if (animatorHandler.Fight)
			{
				if (attackPressed || animatorHandler.PunchState.Running)
				{
					fightTimer.Run(stayFightingDuration);
				}
				animatorHandler.Fight = fightTimer.InProgress();
				animatorHandler.Punch = (animatorHandler.Fight && attackPressed);
				if (fightTimer.Done())
				{
					fightTimer.Stop();
				}
			}
		}

		public void kick(bool attackPressed)
		{
			if (shotController.gunType != 0)
			{
				animatorHandler.Kick = attackPressed;
			}
			if (animatorHandler.Fight)
			{
				if (attackPressed || animatorHandler.PunchState.Running)
				{
					fightTimer.Run(stayFightingDuration);
				}
				animatorHandler.Fight = fightTimer.InProgress();
				animatorHandler.Kick = (animatorHandler.Fight && attackPressed);
			}
		}

		protected void Awake()
		{
			animatorHandler = this.GetComponentSafe<PlayerAnimatorHandler>();
			animator = this.GetComponentSafe<Animator>();
			attackZone = this.GetComponentInChildrenSafe<AttackZone>();
			attackZoneCurrentPos = attackZone.gameObject.transform.localPosition;
			shotController = this.GetComponentSafe<ShotController>();
		}
	}
}
