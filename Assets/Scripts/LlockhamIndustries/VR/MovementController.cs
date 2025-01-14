using UnityEngine;

namespace LlockhamIndustries.VR
{
	public class MovementController : VRController
	{
		public enum MovementMode
		{
			Immediate,
			Combat
		}

		public VRPlayspace playspace;

		public GameObject movementHandle;

		public float cooldown = 0.5f;

		public float speed = 0.08f;

		public MovementMode movementMode;

		public float combatLimit = 3f;

		private float downtime;

		private Vector3 targetVelocity;

		private Vector3 targetPoint;

		private Vector3 HandleOffset
		{
			get
			{
				Vector3 result = base.transform.position - playspace.Position;
				result.y = 0f;
				return result;
			}
		}

		private void Start()
		{
			if (!movementHandle.activeInHierarchy)
			{
				movementHandle = Object.Instantiate(movementHandle, base.transform.parent);
			}
		}

		private void LateUpdate()
		{
			if (playspace == null || movementHandle == null)
			{
				UnityEngine.Debug.LogWarning("Please assign a valid playspace and handle to your movement controller.");
			}
			else if (downtime > 0f)
			{
				downtime = Mathf.MoveTowards(downtime, 0f, Time.deltaTime);
			}
			else if (base.Grip)
			{
				Vector3 a = base.transform.forward;
				a.y = 0f;
				a = a.normalized;
				float d = Vector3.Dot(base.transform.forward, a * combatLimit);
				Vector3 handleOffset = HandleOffset;
				Vector3 target = playspace.Position + a * d;
				targetPoint = Vector3.SmoothDamp(targetPoint, target, ref targetVelocity, speed);
				Vector3 vector = playspace.TrialPosition(targetPoint);
				UpdateHandle(vector, handleOffset);
				if (base.Trigger)
				{
					downtime = cooldown;
					playspace.Position = vector;
					DisableHandle();
				}
			}
			else
			{
				DisableHandle();
			}
		}

		private void UpdateHandle(Vector3 targetPosition, Vector3 handleOffset)
		{
			if (!movementHandle.activeInHierarchy)
			{
				movementHandle.SetActive(value: true);
			}
			movementHandle.transform.position = targetPosition + handleOffset;
		}

		private void DisableHandle()
		{
			if (movementHandle.activeInHierarchy)
			{
				movementHandle.SetActive(value: false);
			}
			targetPoint = playspace.Position;
			targetVelocity = Vector3.zero;
		}
	}
}
