using App.Util;
using UnityEngine;

namespace App.Vehicles.Skateboard
{
	public class SkateboardAnimatorHandler : MonoBehaviour
	{
		private int inAirHash = Animator.StringToHash("InAir");

		private int startHash = Animator.StringToHash("Start");

		private int pushHash = Animator.StringToHash("Push");

		public Animator Animator
		{
			get;
			private set;
		}

		public bool InAir
		{
			get
			{
				return Animator.GetBool(inAirHash);
			}
			set
			{
				Animator.SetBool(inAirHash, value);
			}
		}

		public AutoBlend Direction
		{
			get;
			private set;
		}

		public AnimatorState GroundedState
		{
			get;
			private set;
		}

		public AnimatorState GetOnState
		{
			get;
			private set;
		}

		public AnimatorState JumpState
		{
			get;
			private set;
		}

		public AnimatorState FallState
		{
			get;
			private set;
		}

		public AnimatorState LandState
		{
			get;
			private set;
		}

		public void TriggerStart()
		{
			Animator.SetTrigger(startHash);
		}

		public void TriggerPush()
		{
			Animator.SetTrigger(pushHash);
		}

		private void Awake()
		{
			Animator = this.GetComponentInChildrenSafe<Animator>();
			Direction = new AutoBlend(Animator.StringToHash("Direction"), 2f, Animator);
			GroundedState = new AnimatorState("Base Layer.Grounded", Animator);
			GetOnState = new AnimatorState("Base Layer.GetOn", Animator);
			JumpState = new AnimatorState("Base Layer.Jump", Animator);
			FallState = new AnimatorState("Base Layer.Fall", Animator);
			LandState = new AnimatorState("Base Layer.Land", Animator);
		}
	}
}
