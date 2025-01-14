using UnityEngine;

namespace App.Vehicles.Mech
{
	public class MechManager : MonoBehaviour
	{
		private Vector3 driverScale;

		private MechController controller;

		private MechCabinRotator cabinRotator;

		public GameObject Owner
		{
			get;
			private set;
		}

		public bool IsActive => Owner != null;

		private void Awake()
		{
			controller = GetComponent<MechController>();
			cabinRotator = GetComponentInChildren<MechCabinRotator>();
		}

		public void Activate(GameObject owner)
		{
			driverScale = owner.transform.localScale;
			owner.transform.localScale = new Vector3(0f, 0f, 0f);
			controller.IsActive = true;
			Owner = owner;
			cabinRotator.Type = MechCabinRotator.ControlType.PlayerControl;
		}

		public void Deactivate()
		{
			if (!(Owner == null))
			{
				ExitVehicle();
				controller.IsActive = false;
				Owner = null;
				cabinRotator.Type = MechCabinRotator.ControlType.Stay;
			}
		}

		public void ExitVehicle()
		{
			Owner.transform.localScale = driverScale;
		}
	}
}
