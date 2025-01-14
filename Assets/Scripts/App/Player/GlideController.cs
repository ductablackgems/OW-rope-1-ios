using App.Player.Definition;
using UnityEngine;

namespace App.Player
{
	public class GlideController : MonoBehaviour, ICharacterModule
	{
		private const float StartGlideSpeed = -10f;

		private const float MaxGlideMoveForce = 1200f;

		private const float MaxForwardForceCoeff = 3f;

		private const float TresholdForwardSpeed = 20f;

		private const float MaxUpForceCoeff = 3f;

		private const float TresholdDownSpeed = 40f;

		private Rigidbody _rigidbody;

		private PlayerAnimatorHandler animatorHandler;

		private WingsAnimationHandler wingsAnimatorHandler;

		private CharacterControl characterControl;

		private DurationTimer stopTimer = new DurationTimer();

		public bool Runnable()
		{
			RaycastHit hitInfo;
			if (_rigidbody.velocity.y < -10f)
			{
				return !characterControl.IsGroundedRaw(out hitInfo);
			}
			return false;
		}

		public bool Crashable()
		{
			if (!animatorHandler.GlideState.Running)
			{
				return animatorHandler.RollFromGlideState.Running;
			}
			return true;
		}

		public void Run()
		{
			if (!Running() && Runnable())
			{
				animatorHandler.Glide = true;
				animatorHandler.GlideY.ForceValue(1f);
			}
		}

		public bool Running()
		{
			return animatorHandler.Glide;
		}

		public void Stop()
		{
			animatorHandler.Glide = false;
			stopTimer.Stop();
		}

		private void Awake()
		{
			_rigidbody = this.GetComponentSafe<Rigidbody>();
			animatorHandler = this.GetComponentSafe<PlayerAnimatorHandler>();
			wingsAnimatorHandler = GetComponent<WingsAnimationHandler>();
			characterControl = this.GetComponentSafe<CharacterControl>();
		}

		public void Control(Vector3 move)
		{
			if (!animatorHandler.Glide)
			{
				return;
			}
			if (characterControl.IsGroundedRaw(out RaycastHit _))
			{
				Stop();
				return;
			}
			if (_rigidbody.velocity.y > -1f)
			{
				if (!stopTimer.Running())
				{
					stopTimer.Run(0.4f);
				}
				else if (stopTimer.Done())
				{
					Stop();
					return;
				}
			}
			else
			{
				stopTimer.Stop();
			}
			Vector3 vector = base.transform.InverseTransformDirection(move);
			float num = (vector.z > 0f) ? vector.z : 0f;
			animatorHandler.GlideY.BlendTo(num);
			Vector3 vector2 = base.transform.InverseTransformDirection(_rigidbody.velocity);
			vector2.x *= 0.8f;
			if (vector.z < 0f)
			{
				vector2.z *= 0.8f;
			}
			else
			{
				vector2.z *= 0.94f + num * 0.06f;
			}
			float num2 = (vector2.y > 0f) ? 0f : (0f - vector2.y);
			float t = Mathf.Clamp((40f - num2) / 40f, 0f, 1f);
			float d = Mathf.Lerp(3f, 1f, t);
			float num3 = (vector2.z > 0f) ? vector2.z : 0f;
			float t2 = Mathf.Clamp((20f - num3) / 20f, 0f, 1f);
			float d2 = Mathf.Lerp(1f, 3f, t2);
			_rigidbody.velocity = base.transform.TransformDirection(vector2);
			_rigidbody.AddForce(Vector3.Lerp(base.transform.forward * d2, base.transform.up * d, 0.3f) * 1200f * num);
			animatorHandler.GlideY.Update(Time.fixedDeltaTime);
			if ((bool)wingsAnimatorHandler)
			{
				wingsAnimatorHandler.GlideY = animatorHandler.GlideY.TargetValue;
			}
		}
	}
}
