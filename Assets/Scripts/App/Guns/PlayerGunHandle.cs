using App.Util;
using App.Vehicles;
using App.Vehicles.Helicopter;
using App.Vehicles.Mech;
using UnityEngine;

namespace App.Guns
{
	public class PlayerGunHandle : MonoBehaviour
	{
		public string buttonName = "MiniGunButton";

		public VehicleType VehicleType;

		[Space]
		public bool limitFixAngle;

		public float maxFixAngle = 5f;

		private WeaponController weaponController;

		private HelicopterManager helicopterManager;

		private TankManager _tankManager;

		private MechManager mechManager;

		private Transform virtualTargetTransform;

		protected void Awake()
		{
			weaponController = this.GetComponentInChildrenSafe<WeaponController>();
			if (VehicleType == VehicleType.Helicopter)
			{
				helicopterManager = this.GetComponentInParentsSafe<HelicopterManager>();
			}
			else if (VehicleType == VehicleType.Tank)
			{
				_tankManager = this.GetComponentInParentsSafe<TankManager>();
			}
			else if (VehicleType == VehicleType.Mech)
			{
				mechManager = this.GetComponentInParentsSafe<MechManager>();
			}
			virtualTargetTransform = ServiceLocator.GetGameObject("VirtualTarget").transform;
			weaponController.enabled = false;
		}

		private void OnEnable()
		{
			weaponController.enabled = true;
		}

		private void OnDisable()
		{
			weaponController.enabled = false;
		}

		protected void Update()
		{
			if ((VehicleType != VehicleType.Helicopter || helicopterManager.active) && (VehicleType != VehicleType.Tank || _tankManager.Active) && (VehicleType != VehicleType.Mech || mechManager.IsActive) && InputUtils.GetVehicleButton(VehicleType, buttonName))
			{
				base.transform.LookAt(virtualTargetTransform);
				if (limitFixAngle)
				{
					base.transform.localRotation = Quaternion.RotateTowards(Quaternion.identity, base.transform.localRotation, maxFixAngle);
				}
				weaponController.LaunchWeapon();
			}
		}
	}
}
