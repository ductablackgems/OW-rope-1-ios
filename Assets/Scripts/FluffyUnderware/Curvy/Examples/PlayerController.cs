using System.Collections;
using UnityEngine;

namespace FluffyUnderware.Curvy.Examples
{
	public class PlayerController : MonoBehaviour
	{
		public CurvySplineBase Spline;

		public float Speed = 20f;

		public float JumpSpeed = 20f;

		public float JumpDuration = 0.5f;

		public float Gravity = 15f;

		public float TF;

		private CharacterController mController;

		private Transform mTransform;

		private float mLastCurveY;

		private float mJumpDurationLeft;

		private bool mStopMoving;

		private IEnumerator Start()
		{
			mController = GetComponent<CharacterController>();
			mController.stepOffset = 0f;
			mTransform = base.transform;
			Physics.IgnoreCollision(GetComponentInChildren<CapsuleCollider>(), mController.GetComponent<Collider>());
			if ((bool)Spline)
			{
				while (!Spline.IsInitialized)
				{
					yield return new WaitForEndOfFrame();
				}
				Init();
			}
		}

		private void Update()
		{
			Vector3 vector = Vector3.zero;
			Vector3 position = mTransform.position;
			float tF = TF;
			float y = mLastCurveY;
			float axis = UnityEngine.Input.GetAxis("Horizontal");
			bool button = Input.GetButton("Jump");
			if (axis != 0f)
			{
				int direction = (!(axis > 0f)) ? 1 : (-1);
				Vector3 vector2 = Spline.MoveBy(ref TF, ref direction, Mathf.Abs(axis) * Speed * Time.smoothDeltaTime, CurvyClamping.Loop);
				vector.x = vector2.x - position.x;
				vector.z = vector2.z - position.z;
				y = vector2.y;
			}
			if (button && mJumpDurationLeft > 0f)
			{
				vector += new Vector3(0f, JumpSpeed * Time.smoothDeltaTime, 0f);
				mJumpDurationLeft -= Time.deltaTime;
			}
			else
			{
				vector += new Vector3(0f, (0f - Gravity) * Time.smoothDeltaTime, 0f);
			}
			if (position.y + vector.y < y)
			{
				vector.y = y - position.y;
				mJumpDurationLeft = JumpDuration;
			}
			if (vector != Vector3.zero)
			{
				if (mController.Move(vector) != 0)
				{
					if (mStopMoving)
					{
						mTransform.position = position;
						TF = tF;
						y = mLastCurveY;
					}
				}
				else
				{
					mStopMoving = false;
				}
				mTransform.rotation = Spline.GetOrientationFast(TF);
			}
			mLastCurveY = y;
		}

		private void OnControllerColliderHit(ControllerColliderHit hit)
		{
			mStopMoving = (hit.moveDirection.x != 0f);
		}

		private void Init()
		{
			if ((bool)Spline)
			{
				mTransform.position = Spline.Interpolate(TF);
				mTransform.rotation = Spline.GetOrientationFast(TF);
				mLastCurveY = mTransform.position.y;
			}
		}
	}
}
