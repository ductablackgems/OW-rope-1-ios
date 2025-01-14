using App.Abilities;
using UnityEngine;

namespace App.Vehicles.Mech
{
	public abstract class MechTechnology : MonoBehaviour
	{
		[SerializeField]
		private float m_MaxForwardSpeed = 10f;

		[SerializeField]
		private float m_MaxBackwardSpeed = 6f;

		[SerializeField]
		private AudioClip activationSound;

		[SerializeField]
		private AudioClip deactivationSound;

		protected AudioSource audioSource;

		protected bool isInitialized;

		protected MechController mechController;

		public float MaxForwardSpeed => m_MaxForwardSpeed;

		public float MaxBackwardSpeed => m_MaxBackwardSpeed;

		public virtual bool CanActivate => true;

		public virtual bool CanMove => true;

		public virtual bool IsActive => false;

		public virtual bool IsFlying => false;

		public virtual bool CanFly => false;

		protected Animator Animator => mechController.Animator;

		protected virtual void OnInitialize()
		{
		}

		protected virtual void OnActivate()
		{
		}

		protected virtual void OnDeactivate()
		{
		}

		protected virtual void OnMove(Vector2 dir)
		{
		}

		protected virtual void OnTurn(float turn)
		{
		}

		protected virtual void OnMoveUp()
		{
		}

		protected virtual void OnMoveDown()
		{
		}

		protected virtual bool OnIsAbilitySupported(Ability ability)
		{
			return true;
		}

		public void Activate()
		{
			OnActivate();
			PlayOneShot(activationSound);
		}

		public void Deactivate()
		{
			OnDeactivate();
			PlayOneShot(deactivationSound);
		}

		public void Move(Vector2 dir)
		{
			OnMove(dir);
		}

		public void Turn(float turn)
		{
			OnTurn(turn);
		}

		public void MoveUp()
		{
			OnMoveUp();
		}

		public void MoveDown()
		{
			OnMoveDown();
		}

		public void Initialize(MechController controller)
		{
			mechController = controller;
			audioSource = controller.Sounds.Effects;
			OnInitialize();
			isInitialized = true;
		}

		public bool IsAbilitySupported(Ability ability)
		{
			return OnIsAbilitySupported(ability);
		}

		protected void PlayOneShot(AudioClip clip)
		{
			if (!(audioSource == null))
			{
				audioSource.PlayOneShot(clip);
			}
		}
	}
}
