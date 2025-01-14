using UnityEngine;

namespace App.Vehicles.Car.GarbageTruck
{
	public class AIPointController : MonoBehaviour
	{
		public Transform virtualBack;

		public Transform virtualLeft;

		public Transform virtualRight;

		public Transform finish;

		public bool IsFull
		{
			get;
			set;
		}

		private void Awake()
		{
			IsFull = false;
		}
	}
}
