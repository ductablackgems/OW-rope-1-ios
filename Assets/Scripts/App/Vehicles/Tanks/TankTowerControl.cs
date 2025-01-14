using App.Vehicles.Tank;
using UnityEngine;

namespace App.Vehicles.Tanks
{
	public class TankTowerControl : MonoBehaviour, ITankTowerControl
	{
		public float horizontalSpeed = 1.5f;

		public float verticalSpeed = 3f;

		public float minCannonAngle = -10f;

		public float maxCannonAngle = 60f;

		public Transform cannonTransform;

		public Transform localVirtualTarget;

		private Transform virtualTarget;

		private Vector3 initialEuler;

		private Vector3 initialCannonEuler;

		public TankTowerState currentState = TankTowerState.PlayerControl;

		public void SetState(TankTowerState state)
		{
			currentState = state;
		}

		public void RotateToTarget(Vector3 targetPos)
		{
			SetState(TankTowerState.Target);
			localVirtualTarget.position = targetPos;
		}

		protected void Start()
		{
			virtualTarget = ServiceLocator.GetGameObject("VirtualTarget").transform;
			initialEuler = base.transform.localRotation.eulerAngles;
			initialCannonEuler = cannonTransform.localRotation.eulerAngles;
		}

		protected void Update()
		{
			if (currentState == TankTowerState.Stay)
			{
				return;
			}
			if (currentState == TankTowerState.Home)
			{
				base.transform.localRotation = Quaternion.RotateTowards(base.transform.localRotation, Quaternion.Euler(initialEuler), Time.deltaTime * horizontalSpeed);
				cannonTransform.localRotation = Quaternion.RotateTowards(cannonTransform.localRotation, Quaternion.Euler(initialCannonEuler), Time.deltaTime * verticalSpeed);
				return;
			}
			if (currentState == TankTowerState.PlayerControl)
			{
				localVirtualTarget.position = virtualTarget.position;
			}
			Vector3 eulerAngles = Quaternion.LookRotation(localVirtualTarget.localPosition - base.transform.localPosition).eulerAngles;
			Quaternion to = Quaternion.Euler(initialEuler.x, eulerAngles.y, initialEuler.z);
			base.transform.localRotation = Quaternion.RotateTowards(base.transform.localRotation, to, Time.deltaTime * horizontalSpeed);
			Quaternion to2 = Quaternion.Euler(eulerAngles.x, initialCannonEuler.y, initialCannonEuler.z);
			float num = to2.eulerAngles.x;
			if (num > 180f)
			{
				num = -360f + num;
			}
			if (num > maxCannonAngle)
			{
				to2 = Quaternion.Euler(maxCannonAngle, initialCannonEuler.y, initialCannonEuler.z);
			}
			else if (num < minCannonAngle)
			{
				to2 = Quaternion.Euler(minCannonAngle, initialCannonEuler.y, initialCannonEuler.z);
			}
			cannonTransform.localRotation = Quaternion.RotateTowards(cannonTransform.localRotation, to2, Time.deltaTime * verticalSpeed);
		}
	}
}
