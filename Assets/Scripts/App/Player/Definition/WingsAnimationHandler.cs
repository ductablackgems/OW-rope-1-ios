using UnityEngine;

namespace App.Player.Definition
{
	public class WingsAnimationHandler : MonoBehaviour
	{
		public Animator wingsAnimator;

		private DurationTimer RandomTimer = new DurationTimer();

		private int flyXHash = Animator.StringToHash("FlyX");

		private int flyZHash = Animator.StringToHash("FlyZ");

		private int flyYHash = Animator.StringToHash("FlyY");

		private int flyUpDown = Animator.StringToHash("FlyUpDown");

		private int useCarHash = Animator.StringToHash("UseCar");

		private int glideHash = Animator.StringToHash("Glide");

		private int glideYHash = Animator.StringToHash("GlideY");

		private int flyHash = Animator.StringToHash("Fly");

		private int onGroundHash = Animator.StringToHash("OnGround");

		private int randomStateHash = Animator.StringToHash("RandomState");

		private int fwdGlideStateHash = Animator.StringToHash("FwdGlide");

		private const float IdleTimeScale = 1f;

		private const float OtherTimeScale = 1.5f;

		private const int RandomTimeDelay = 10;

		private bool gliding;

		private float glideSpeed = 2f;

		public float FlyX
		{
			get
			{
				return wingsAnimator.GetFloat(flyXHash);
			}
			set
			{
				wingsAnimator.SetFloat(flyXHash, value);
			}
		}

		public float FlyZ
		{
			get
			{
				return wingsAnimator.GetFloat(flyZHash);
			}
			set
			{
				wingsAnimator.SetFloat(flyZHash, value);
			}
		}

		public float FlyY
		{
			get
			{
				return wingsAnimator.GetFloat(flyYHash);
			}
			set
			{
				wingsAnimator.SetFloat(flyYHash, value);
			}
		}

		public float FlyUpDown
		{
			get
			{
				return wingsAnimator.GetFloat(flyUpDown);
			}
			set
			{
				wingsAnimator.SetFloat(flyUpDown, value);
			}
		}

		public bool UseCar
		{
			get
			{
				return wingsAnimator.GetBool(useCarHash);
			}
			set
			{
				wingsAnimator.SetBool(useCarHash, value);
			}
		}

		public bool Glide
		{
			get
			{
				return wingsAnimator.GetBool(glideHash);
			}
			set
			{
				wingsAnimator.SetBool(glideHash, value);
			}
		}

		public float GlideY
		{
			get
			{
				return wingsAnimator.GetFloat(glideYHash);
			}
			set
			{
				wingsAnimator.SetFloat(glideYHash, value);
			}
		}

		public bool Fly
		{
			get
			{
				return wingsAnimator.GetBool(flyHash);
			}
			set
			{
				wingsAnimator.SetBool(flyHash, value);
			}
		}

		public bool OnGround
		{
			get
			{
				return wingsAnimator.GetBool(onGroundHash);
			}
			set
			{
				wingsAnimator.SetBool(onGroundHash, value);
			}
		}

		public int RandomState
		{
			get
			{
				return wingsAnimator.GetInteger(randomStateHash);
			}
			set
			{
				wingsAnimator.SetInteger(randomStateHash, value);
			}
		}

		public bool FwdGlide
		{
			get
			{
				return wingsAnimator.GetBool(fwdGlideStateHash);
			}
			set
			{
				wingsAnimator.SetBool(fwdGlideStateHash, value);
			}
		}

		private void Awake()
		{
			RandomTimer.Run(10f);
		}

		private void Update()
		{
			if (RandomTimer.Done())
			{
				RandomState = UnityEngine.Random.Range(0, 2);
				RandomTimer.Run(10f);
			}
			SetState();
			SetSpeed();
		}

		private void SetSpeed()
		{
			if (FlyX == 0f && FlyY == 0f && FlyZ == 0f)
			{
				if (wingsAnimator.speed != 1f)
				{
					wingsAnimator.speed = 1f;
				}
			}
			else if (wingsAnimator.speed != 1.5f)
			{
				wingsAnimator.speed = 1.5f;
			}
		}

		private void SetState()
		{
			if (FlyY != 0f)
			{
				FlyUpDown = 1f;
			}
			else
			{
				FlyUpDown = 0f;
			}
		}
	}
}
