using App.Vehicles;
using UnityEngine;

namespace App.Util
{
	public class FireManager : MonoBehaviour
	{
		private const float MinimalEstinguishTime = 3f;

		public GameObject wreckFire;

		private VehicleExplosion vehicleExplosion;

		private float currentExtinguishDuration = -1f;

		public bool IsWreck()
		{
			if (vehicleExplosion != null)
			{
				return vehicleExplosion.IsExploded;
			}
			return wreckFire != null;
		}

		public bool IsInFire()
		{
			if (vehicleExplosion == null)
			{
				return wreckFire.activeSelf;
			}
			return vehicleExplosion.IsInFire();
		}

		public bool CoolDown(float deltaTime)
		{
			currentExtinguishDuration = ((currentExtinguishDuration < 0f) ? deltaTime : (currentExtinguishDuration + deltaTime));
			if (currentExtinguishDuration < 3f)
			{
				return false;
			}
			currentExtinguishDuration = -1f;
			if (vehicleExplosion == null)
			{
				wreckFire.SetActive(value: false);
			}
			else
			{
				vehicleExplosion.CoolDownFire();
			}
			return true;
		}

		private void Awake()
		{
			vehicleExplosion = GetComponent<VehicleExplosion>();
		}

		private void Update()
		{
			if (!IsInFire())
			{
				currentExtinguishDuration = -1f;
			}
		}
	}
}
