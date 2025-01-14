using App.Abilities;
using System.Collections;
using UnityEngine;

namespace App.Vehicles.Mech
{
	public sealed class MechRoller : MechTechnology
	{
		private enum State
		{
			None,
			Activating,
			Activated,
			Deactivating
		}

		[SerializeField]
		private float m_ActivationDuration = 1f;

		[SerializeField]
		private float m_DeactivationDuration = 1f;

		private readonly int AnimRollerParamID = Animator.StringToHash("roller");

		private readonly int AnimTurnParamID = Animator.StringToHash("turn");

		private readonly int AnimSpeedParamID = Animator.StringToHash("speed");

		private State state;

		private Rigidbody rigidbody;

		private MechSounds sounds;

		public override bool CanMove => state == State.Activated;

		public override bool IsActive => state != State.None;

		public override bool CanActivate => state == State.None;

		protected override void OnInitialize()
		{
			base.OnInitialize();
			rigidbody = GetComponent<Rigidbody>();
			sounds = GetComponentInChildren<MechSounds>();
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			base.Animator.SetBool(AnimRollerParamID, value: true);
			StartCoroutine(Activation_Coroutine());
		}

		protected override void OnDeactivate()
		{
			base.OnDeactivate();
			base.Animator.SetBool(AnimRollerParamID, value: false);
			ActivateMoveSound(isActive: false);
			StartCoroutine(Deactivation_Coroutine());
		}

		protected override void OnMove(Vector2 move)
		{
			base.OnMove(move);
			if (state == State.Activated)
			{
				Vector3 a = mechController.MoveAccordingToCameraDiraction(Time.deltaTime);
				float num = a.magnitude * base.MaxForwardSpeed;
				Vector3 velocity = a * num;
				velocity.y = 0f;
				rigidbody.velocity = velocity;
				ActivateMoveSound(Mathf.Abs(num) > 0f);
			}
		}

		protected override void OnTurn(float turn)
		{
			base.OnTurn(turn);
			Vector3 forward = base.transform.forward;
			forward.y = 0f;
			Vector3 forward2 = mechController.Cabine.transform.forward;
			forward2.y = 0f;
			float num = Vector3.Dot(forward.normalized, forward2.normalized);
			bool flag = Mathf.Abs(num) < 0.1f;
			turn *= (flag ? 0f : ((num > 0f) ? 1f : (-1f)));
			base.Animator.SetFloat(AnimTurnParamID, turn);
		}

		protected override bool OnIsAbilitySupported(Ability ability)
		{
			return !(ability is AbilityAbsorbShield);
		}

		private IEnumerator Activation_Coroutine()
		{
			state = State.Activating;
			yield return new WaitForSeconds(m_ActivationDuration);
			state = State.Activated;
		}

		private IEnumerator Deactivation_Coroutine()
		{
			state = State.Deactivating;
			yield return new WaitForSeconds(m_DeactivationDuration);
			state = State.None;
		}

		private void ActivateMoveSound(bool isActive)
		{
			AudioSource audioSource = (sounds != null) ? sounds.Roller : null;
			if (audioSource == null)
			{
				return;
			}
			if (isActive)
			{
				if (!audioSource.isPlaying)
				{
					audioSource.Play();
				}
			}
			else
			{
				audioSource.Stop();
			}
		}
	}
}
