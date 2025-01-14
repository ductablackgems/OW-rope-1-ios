using UnityEngine;

namespace Forge3D
{
	public class F3DTurret : MonoBehaviour
	{
		[HideInInspector]
		public bool destroyIt;

		public GameObject Mount;

		public GameObject Swivel;

		private Vector3 defaultDir;

		private Quaternion defaultRot;

		private Transform headTransform;

		private Transform barrelTransform;

		public float HeadingTrackingSpeed = 2f;

		public float ElevationTrackingSpeed = 2f;

		private Vector3 targetPos;

		[HideInInspector]
		public Vector3 headingVetor;

		private float curHeadingAngle;

		private float curElevationAngle;

		public Vector2 HeadingLimit;

		public Vector2 ElevationLimit;

		public bool smoothControlling;

		public bool DebugDraw;

		private bool fullAccess;

		public Animator[] Animators;

		private void Awake()
		{
			headTransform = Swivel.GetComponent<Transform>();
			barrelTransform = Mount.GetComponent<Transform>();
		}

		public void PlayAnimation()
		{
			for (int i = 0; i < Animators.Length; i++)
			{
				Animators[i].SetTrigger("FireTrigger");
			}
		}

		public void PlayAnimationLoop()
		{
			for (int i = 0; i < Animators.Length; i++)
			{
				Animators[i].SetBool("FireLoopBool", value: true);
			}
		}

		public void StopAnimation()
		{
			for (int i = 0; i < Animators.Length; i++)
			{
				Animators[i].SetBool("FireLoopBool", value: false);
			}
		}

		private void Start()
		{
			targetPos = headTransform.transform.position + headTransform.transform.forward * 100f;
			defaultDir = Swivel.transform.forward;
			defaultRot = Quaternion.FromToRotation(base.transform.forward, defaultDir);
			if (HeadingLimit.y - HeadingLimit.x >= 359.9f)
			{
				fullAccess = true;
			}
			StopAnimation();
		}

		public void SetNewTarget(Vector3 _targetPos)
		{
			targetPos = _targetPos;
		}

		public float GetAngleToTarget()
		{
			return Vector3.Angle(Mount.transform.forward, targetPos - Mount.transform.position);
		}

		private void Update()
		{
			if (!smoothControlling)
			{
				if (barrelTransform != null)
				{
					headingVetor = Vector3.Normalize(F3DMath.ProjectVectorOnPlane(headTransform.up, targetPos - headTransform.position));
					float num = F3DMath.SignedVectorAngle(headTransform.forward, headingVetor, headTransform.up);
					float num2 = F3DMath.SignedVectorAngle(defaultRot * headTransform.forward, headingVetor, headTransform.up);
					float num3 = F3DMath.SignedVectorAngle(defaultRot * headTransform.forward, headTransform.forward, headTransform.up);
					float num4 = HeadingTrackingSpeed * Time.deltaTime;
					num4 = ((!(HeadingLimit.x <= -180f) || !(HeadingLimit.y >= 180f)) ? (num4 * Mathf.Sign(num2 - num3)) : (num4 * Mathf.Sign(num)));
					if (Mathf.Abs(num4) > Mathf.Abs(num))
					{
						num4 = num;
					}
					if ((curHeadingAngle + num4 > HeadingLimit.x && curHeadingAngle + num4 < HeadingLimit.y) || (HeadingLimit.x <= -180f && HeadingLimit.y >= 180f) || fullAccess)
					{
						curHeadingAngle += num4;
						headTransform.rotation *= Quaternion.Euler(0f, num4, 0f);
					}
					Vector3 otherVector = Vector3.Normalize(F3DMath.ProjectVectorOnPlane(headTransform.right, targetPos - barrelTransform.position));
					float num5 = F3DMath.SignedVectorAngle(barrelTransform.forward, otherVector, headTransform.right);
					float num6 = Mathf.Sign(num5) * ElevationTrackingSpeed * Time.deltaTime;
					if (Mathf.Abs(num6) > Mathf.Abs(num5))
					{
						num6 = num5;
					}
					if (curElevationAngle + num6 < ElevationLimit.y && curElevationAngle + num6 > ElevationLimit.x)
					{
						curElevationAngle += num6;
						barrelTransform.rotation *= Quaternion.Euler(num6, 0f, 0f);
					}
				}
			}
			else
			{
				Transform transform = barrelTransform;
				Transform transform2 = Swivel.transform;
				Quaternion b = Quaternion.LookRotation(targetPos - transform.transform.position, headTransform.up);
				transform.transform.rotation = Quaternion.Slerp(transform.transform.rotation, b, HeadingTrackingSpeed * Time.deltaTime);
				transform.transform.localEulerAngles = new Vector3(transform.transform.localEulerAngles.x, 0f, 0f);
				if (transform.transform.localEulerAngles.x >= 180f && transform.transform.localEulerAngles.x < 360f - ElevationLimit.y)
				{
					transform.transform.localEulerAngles = new Vector3(360f - ElevationLimit.y, 0f, 0f);
				}
				else if (transform.transform.localEulerAngles.x < 180f && transform.transform.localEulerAngles.x > 0f - ElevationLimit.x)
				{
					transform.transform.localEulerAngles = new Vector3(0f - ElevationLimit.x, 0f, 0f);
				}
				Vector3 a = targetPos;
				a.y = transform2.position.y;
				Quaternion b2 = Quaternion.LookRotation(a - transform2.position, transform2.transform.up);
				transform2.transform.rotation = Quaternion.Slerp(transform2.transform.rotation, b2, ElevationTrackingSpeed * Time.deltaTime);
				transform2.transform.localEulerAngles = new Vector3(0f, transform2.transform.localEulerAngles.y, 0f);
				if (!fullAccess)
				{
					if (transform2.transform.localEulerAngles.y >= 180f && transform2.transform.localEulerAngles.y < 360f - HeadingLimit.y)
					{
						transform2.transform.localEulerAngles = new Vector3(0f, 360f - HeadingLimit.y, 0f);
					}
					else if (transform2.transform.localEulerAngles.y < 180f && transform2.transform.localEulerAngles.y > 0f - HeadingLimit.x)
					{
						transform2.transform.localEulerAngles = new Vector3(0f, 0f - HeadingLimit.x, 0f);
					}
				}
			}
			if (DebugDraw)
			{
				UnityEngine.Debug.DrawLine(barrelTransform.position, barrelTransform.position + barrelTransform.forward * Vector3.Distance(barrelTransform.position, targetPos), Color.red);
			}
		}
	}
}
