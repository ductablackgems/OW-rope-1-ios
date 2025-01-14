using App.Vehicles;
using System;
using UnityEngine;

namespace App.Settings
{
	public class GameSettings : ScriptableObject
	{
		[Serializable]
		public class VehicleIcon
		{
			public VehicleType Type;

			public Sprite Icon;
		}

		public ControlMode ControlMode = ControlMode.touch;

		public VehicleIcon[] VehicleIcons;

		public Sprite GetVehicleIcon(VehicleType type)
		{
			for (int i = 0; i < VehicleIcons.Length; i++)
			{
				VehicleIcon vehicleIcon = VehicleIcons[i];
				if (vehicleIcon != null && vehicleIcon.Type == type)
				{
					return vehicleIcon.Icon;
				}
			}
			return null;
		}
	}
}
