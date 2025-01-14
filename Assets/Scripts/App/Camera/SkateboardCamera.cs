using App.Vehicles.Bicycle;
using App.Vehicles.Skateboard;
using System.Linq;
using UnityEngine;

namespace App.Camera
{
	public class SkateboardCamera : MonoBehaviour
	{
		public const float BackWallOffset = 0.05f;

		public float minDistance = 1f;

		public float maxDistance = 5f;

		public float changeDistanceSpeed = 15f;

		public LayerMask mask;

		public float castRadius = 0.3f;

		public float yOffset;

		public float maxAirYDirection = 0.7f;

		public float maxYSpeed = 10f;

		public float maxSpeed = 20f;

		public float rotationSpeed = 100f;

		public OotiiInputSource inputSource;

		public float maxXAngle = 45f;

		public float minXAngle = -45f;

		private Transform targetHelper;

		private Transform rotator;

		private RaycastHit[] hits = new RaycastHit[15];

		private float currentDistance;

		private Transform target;

		private Collider[] targetColliders;

		private PlayerSkateboardController skateboardController;

		private Rigidbody targetRigidbody;

		private bool padTouched;

		public void ConnectSkateboard(GameObject skateboard)
		{
			target = skateboard.transform;
			targetColliders = skateboard.GetComponentSafe<StreetVehicleModesHelper>().playerStateColliders.GetComponentsInChildren<Collider>();
			skateboardController = skateboard.GetComponentSafe<PlayerSkateboardController>();
			targetRigidbody = skateboard.GetComponentSafe<Rigidbody>();
		}

		private void Awake()
		{
			targetHelper = new GameObject("_AdaptiveCameraHelper_").transform;
			rotator = new GameObject("_Rotator_").transform;
			rotator.parent = targetHelper;
		}

		private void Start()
		{
			ETCTouchPad controlTouchPad = ETCInput.GetControlTouchPad("CameraPad");
			if (controlTouchPad != null)
			{
				controlTouchPad.onTouchStart.AddListener(delegate
				{
					padTouched = true;
				});
				controlTouchPad.onTouchUp.AddListener(delegate
				{
					padTouched = false;
				});
			}
		}

		private void OnEnable()
		{
			targetHelper.position = target.position;
			targetHelper.rotation = base.transform.rotation;
			rotator.localRotation = Quaternion.identity;
			currentDistance = Vector3.Distance(rotator.TransformPoint(Vector3.up * yOffset), base.transform.position);
		}

		private void LateUpdate()
		{
			if (target == null)
			{
				return;
			}
			UpdateRotator();
			Quaternion b;
			if (skateboardController.IsVertJumping)
			{
				b = Quaternion.LookRotation(Vector3.down, skateboardController.VertNormal);
			}
			else if (skateboardController.InAir)
			{
				Vector3 normalized = targetRigidbody.velocity.normalized;
				normalized.y = Mathf.Lerp(0f - maxAirYDirection, maxAirYDirection, (targetRigidbody.velocity.y + maxYSpeed) / maxYSpeed / 2f);
				Vector3 a = targetRigidbody.velocity;
				a.y = 0f;
				if (a.magnitude < 0.05f)
				{
					a = target.forward * skateboardController.Direction;
					a.y *= 0.5f;
				}
				a = Vector3.Lerp(a, normalized, targetRigidbody.velocity.magnitude / maxSpeed);
				Vector3 up = target.up;
				up.y /= 0.5f;
				b = Quaternion.LookRotation(a.normalized, up.normalized);
			}
			else
			{
				Vector3 vector = target.forward * skateboardController.Direction;
				vector.y *= 0.5f;
				Vector3 up2 = target.up;
				up2.y /= 0.5f;
				b = Quaternion.LookRotation(vector.normalized, up2.normalized);
			}
			targetHelper.rotation = Quaternion.Slerp(targetHelper.rotation, b, rotationSpeed * Time.deltaTime);
			targetHelper.position = target.position;
			float num = CountTargetDistance();
			if (num < currentDistance)
			{
				currentDistance = num;
			}
			else
			{
				currentDistance = Mathf.MoveTowards(currentDistance, num, changeDistanceSpeed * Time.deltaTime);
			}
			base.transform.position = rotator.TransformPoint(Vector3.back * currentDistance + Vector3.up * yOffset);
			base.transform.rotation = rotator.rotation;
		}

		private void UpdateRotator()
		{
			if (padTouched)
			{
				Vector3 localEulerAngles = rotator.localEulerAngles;
				localEulerAngles.y += inputSource.ViewX;
				float num = RangeEulerAngle(localEulerAngles.x);
				num -= inputSource.ViewY;
				num = (localEulerAngles.x = Mathf.Clamp(num, minXAngle, maxXAngle));
				rotator.localEulerAngles = localEulerAngles;
			}
			else
			{
				Vector3 localEulerAngles2 = rotator.localEulerAngles;
				float a = RangeEulerAngle(localEulerAngles2.x);
				a = Mathf.Lerp(a, 0f, rotationSpeed * Time.deltaTime);
				float a2 = RangeEulerAngle(localEulerAngles2.y);
				a2 = Mathf.Lerp(a2, 0f, rotationSpeed * Time.deltaTime);
				rotator.localRotation = Quaternion.Euler(a, a2, 0f);
			}
		}

		private float CountTargetDistance()
		{
			float num = 0.05f - castRadius;
			float num2 = maxDistance + num;
			int num3 = Physics.SphereCastNonAlloc(rotator.TransformPoint(Vector3.up * yOffset), castRadius, rotator.forward * -1f, hits, num2, mask, QueryTriggerInteraction.Ignore);
			if (num3 == 0)
			{
				return maxDistance;
			}
			float num4 = num2;
			for (int i = 0; i < num3; i++)
			{
				RaycastHit raycastHit = hits[i];
				if (!targetColliders.Contains(raycastHit.collider) && raycastHit.distance < num4)
				{
					num4 = raycastHit.distance;
				}
			}
			if (num4 < minDistance + num)
			{
				num4 = minDistance + num;
			}
			return num4 - num;
		}

		private float RangeEulerAngle(float eulerAngle)
		{
			if (eulerAngle > 180f)
			{
				eulerAngle -= 360f;
			}
			else if (eulerAngle < -180f)
			{
				eulerAngle += 360f;
			}
			return eulerAngle;
		}
	}
}
