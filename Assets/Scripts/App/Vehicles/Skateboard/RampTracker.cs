using App.Skatepark;
using UnityEngine;

namespace App.Vehicles.Skateboard
{
	public class RampTracker : MonoBehaviour
	{
		public float minVertOffset = 0.2f;

		public float maxVertOffset = 0.5f;

		public float maxSpeed = 25f;

		public float minVertY;

		private Rigidbody _rigidbody;

		private Transform helperTransform;

		private WheelHit hit;

		private VertRamp ramp;

		private Collider _collider;

		public void Clear()
		{
			ramp = null;
			_collider = null;
		}

		public bool RampTracked()
		{
			return ramp != null;
		}

		public void MarkHit(WheelHit hit)
		{
			this.hit = hit;
			if (!hit.collider.Equals(_collider))
			{
				_collider = hit.collider;
				ramp = hit.collider.GetComponent<VertRamp>();
			}
		}

		public VertJumpData ResolveJump()
		{
			VertJumpData vertJumpData = default(VertJumpData);
			if (ramp == null)
			{
				return vertJumpData;
			}
			vertJumpData.vertNormal = ramp.TranslateVertNormal();
			helperTransform.position = ramp.TranslateCopingPosition();
			helperTransform.forward = vertJumpData.vertNormal;
			Vector3 vector = helperTransform.InverseTransformDirection(_rigidbody.velocity);
			float a = ramp.TranslateMinVertY();
			float b = ramp.TranslateVertY(minVertY);
			float num = Mathf.Lerp(a, b, (0f - vector.z) / maxSpeed);
			vertJumpData.isVertJump = (base.transform.position.y > num);
			vector.z = 0f;
			vertJumpData.jumpDirection = helperTransform.TransformDirection(vector).normalized;
			vertJumpData.isCorner = ramp.rampData.isCorner;
			return vertJumpData;
		}

		public Vector3 GetVertAirPosition()
		{
			Vector3 vector = helperTransform.InverseTransformPoint(base.transform.position);
			vector.z = Mathf.Clamp(vector.z, minVertOffset, maxVertOffset);
			return helperTransform.TransformPoint(vector);
		}

		public bool IsAboveRamp()
		{
			return base.transform.position.y > ramp.TranslateCopingPosition().y;
		}

		private void Awake()
		{
			_rigidbody = this.GetComponentSafe<Rigidbody>();
		}

		private void Start()
		{
			helperTransform = new GameObject("_RampTrackerHelper_").transform;
		}

		private void OnDestroy()
		{
			if (helperTransform != null)
			{
				UnityEngine.Object.Destroy(helperTransform.gameObject);
			}
		}
	}
}
