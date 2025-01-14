using UnityEngine;

namespace App.Vehicles.Airplane
{
	public class AirplaneManager : MonoBehaviour
	{
		private AirplaneController controller;

		public GameObject Owner
		{
			get;
			private set;
		}

		public bool IsActive => Owner != null;

		private void Awake()
		{
			controller = GetComponent<AirplaneController>();
		}

		public void Activate(GameObject owner)
		{
			controller.IsActive = true;
			Owner = owner;
		}

		public void Deactivate()
		{
			if (!(Owner == null))
			{
				controller.IsActive = false;
				Owner = null;
			}
		}
	}
}
