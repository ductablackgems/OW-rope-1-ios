using App.Player.Definition;
using App.Player.FightSystem;
using App.Spawn;
using App.Util;
using System;
using UnityEngine;

namespace App.Player
{
	public class RagdollInteractor : MonoBehaviour, IResetable
	{
		public AudioClip[] deathSoundClips;

		[NonSerialized]
		public bool standUpBlocked;

		private PlayerAnimatorHandler playerAnimatorHandler;

		private RagdollHelper ragdollHelper;

		private Health health;

		private HitHandler hitHandler;

		private Rigidbody _rigidbody;

		private Rigidbody hipsRigidbody;

		private GlideController playerGlideController;

		private DurationTimer standUpTimer = new DurationTimer();

		private AudioSource audio;

		private bool scream;

		private bool isPlayer;

		public void ResetStates()
		{
			standUpTimer.Stop();
			standUpBlocked = false;
		}

		protected void Awake()
		{
			playerAnimatorHandler = this.GetComponentSafe<PlayerAnimatorHandler>();
			ragdollHelper = this.GetComponentSafe<RagdollHelper>();
			health = this.GetComponentSafe<Health>();
			hitHandler = this.GetComponentSafe<HitHandler>();
			_rigidbody = this.GetComponentSafe<Rigidbody>();
			hipsRigidbody = this.GetComponentSafe<Animator>().GetBoneTransform(HumanBodyBones.Hips).GetComponentSafe<Rigidbody>();
			isPlayer = CompareTag("Player");
			if (isPlayer)
			{
				playerGlideController = this.GetComponentSafe<GlideController>();
			}
			else
			{
				playerGlideController = ServiceLocator.GetGameObject("Player").GetComponentSafe<GlideController>();
			}
			ragdollHelper.OnRaggdolled += OnRagdolled;
			audio = this.GetComponentSafe<AudioSource>();
			audio.clip = deathSoundClips[UnityEngine.Random.Range(0, deathSoundClips.Length)];
		}

		protected void OnDestroy()
		{
			ragdollHelper.OnRaggdolled -= OnRagdolled;
		}

		private void OnDisable()
		{
			scream = false;
		}

		protected void Update()
		{
			if (health.Dead())
			{
				if (!hitHandler.WillRagdoll())
				{
					ragdollHelper.Ragdolled = true;
				}
				if (!scream && !audio.isPlaying)
				{
					audio.Play();
					scream = true;
				}
			}
			else
			{
				if (standUpBlocked)
				{
					return;
				}
				if (standUpTimer.Done())
				{
					if (ragdollHelper.Ragdolled)
					{
						if (hipsRigidbody.velocity.magnitude > 0.2f)
						{
							standUpTimer.Run(2f);
						}
						ragdollHelper.Ragdolled = false;
					}
					else if (!ragdollHelper.StandingUp)
					{
						standUpTimer.Stop();
					}
				}
				if (!standUpTimer.Running() && ragdollHelper.Ragdolled)
				{
					standUpTimer.Run(2f);
				}
			}
		}

		protected void OnCollisionStay(Collision collision)
		{
			OnCollisionEnter(collision);
		}

		protected void OnCollisionEnter(Collision collision)
		{
			if (collision.gameObject.CompareTag("Vehicle") || collision.gameObject.CompareTag("Bicycle") || collision.gameObject.CompareTag("Gyroboard") || collision.gameObject.CompareTag("Skateboard"))
			{
				float num = collision.rigidbody.velocity.magnitude * 3.6f;
				if ((!(num > 20f) && (!isPlayer || !playerGlideController.Crashable())) || ragdollHelper.Ragdolled || ragdollHelper.StandingUp)
				{
					return;
				}
				ragdollHelper.Ragdolled = true;
				if (collision.gameObject.CompareTag("Bicycle") || collision.gameObject.CompareTag("Gyroboard") || collision.gameObject.CompareTag("Skateboard"))
				{
					health.ApplyDamage(15f, 3, collision.gameObject);
					ragdollHelper.SetRagdollVelocity(Vector3.Lerp(_rigidbody.velocity, collision.rigidbody.velocity, 0.5f));
				}
				else
				{
					if (num < 45f)
					{
						health.ApplyDamage(40f, 3, collision.gameObject);
					}
					else if (num < 65f)
					{
						health.ApplyDamage(60f, 3, collision.gameObject);
					}
					else
					{
						health.ApplyDamage(100f, 3, collision.gameObject);
					}
					ragdollHelper.SetRagdollVelocity(Vector3.Lerp(_rigidbody.velocity, collision.rigidbody.velocity, 0.8f));
				}
				if (!audio.isPlaying)
				{
					audio.Play();
				}
			}
			else if (!isPlayer && WhoIs.CompareCollision(collision, WhoIs.Entities.Player) && playerGlideController.Crashable())
			{
				ragdollHelper.Ragdolled = true;
				health.ApplyDamage(15f, 6, collision.gameObject);
				ragdollHelper.SetRagdollVelocity(Vector3.Lerp(_rigidbody.velocity, collision.rigidbody.velocity, 0.5f));
			}
			else if (isPlayer && WhoIs.CompareCollision(collision, WhoIs.Entities.Enemy) && playerGlideController.Crashable())
			{
				ragdollHelper.Ragdolled = true;
				health.ApplyDamage(15f, 6, collision.gameObject);
				ragdollHelper.SetRagdollVelocity(Vector3.Lerp(_rigidbody.velocity, collision.rigidbody.velocity, 0.5f));
			}
		}

		private void OnRagdolled(bool ragdolled)
		{
			if (ragdolled && !health.Dead())
			{
				standUpTimer.Run(2f);
			}
		}
	}
}
