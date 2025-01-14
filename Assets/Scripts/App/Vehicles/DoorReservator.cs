using UnityEngine;

namespace App.Vehicles
{
	public class DoorReservator : MonoBehaviour
	{
		private Transform reservator;

		public Transform Reservator => reservator;

		public bool Reserved => reservator != null;

		public bool Reserve(Transform reservator)
		{
			if (this.reservator == null)
			{
				this.reservator = reservator;
				return true;
			}
			return false;
		}

		public bool ReleaseReservation(Transform reservator)
		{
			if (reservator.Equals(this.reservator))
			{
				this.reservator = null;
				return true;
			}
			return false;
		}
	}
}
