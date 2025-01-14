using UnityEngine;

namespace App.Vehicles.Car
{
	public class TaxiSpots : MonoBehaviour
	{
		public TaxiSpot hospitalSpot;

		private Transform player;

		private TaxiSpot[] spots;

		private int missionKey;

		public TaxiSpot TargetSpot
		{
			get;
			private set;
		}

		public bool TargetIsDestination
		{
			get
			{
				if (TargetSpot != null)
				{
					return !TargetSpot.isWaiter;
				}
				return false;
			}
		}

		public TaxiSpot ActivateRandomWaiter(int missionKey, Rigidbody vehicleRigidbody, Transform doorHandle)
		{
			if (TargetSpot != null)
			{
				TargetSpot.enabled = false;
			}
			this.missionKey = missionKey;
			TargetSpot = GetRandomSpot(100f);
			TargetSpot.isWaiter = true;
			TargetSpot.missionKey = missionKey;
			TargetSpot.vehicleRigidbody = vehicleRigidbody;
			TargetSpot.doorHandle = doorHandle;
			TargetSpot.enabled = true;
			return TargetSpot;
		}

		public TaxiSpot ActivateRandomDestination(TaxiType type)
		{
			if (TargetSpot != null)
			{
				TargetSpot.enabled = false;
			}
			switch (type)
			{
			case TaxiType.Ambulance:
				TargetSpot = hospitalSpot;
				break;
			case TaxiType.Bus:
				TargetSpot = GetRandomSpot(100f);
				break;
			case TaxiType.Taxi:
				TargetSpot = GetRandomSpot(100f);
				break;
			}
			TargetSpot.isWaiter = false;
			TargetSpot.missionKey = missionKey;
			TargetSpot.enabled = true;
			return TargetSpot;
		}

		public void ReleaseTarget()
		{
			if (!(TargetSpot == null))
			{
				TargetSpot.enabled = false;
				TargetSpot = null;
			}
		}

		private TaxiSpot GetRandomSpot(float minimalDistace)
		{
			TaxiSpot taxiSpot = null;
			for (int i = 0; i < 5; i++)
			{
				int num = UnityEngine.Random.Range(0, spots.Length);
				if ((spots[num].transform.position - player.position).magnitude > minimalDistace)
				{
					taxiSpot = spots[num];
					break;
				}
			}
			if (taxiSpot == null)
			{
				for (int j = 0; j < spots.Length; j++)
				{
					int num2 = UnityEngine.Random.Range(0, spots.Length);
					if ((spots[num2].transform.position - player.position).magnitude > minimalDistace)
					{
						taxiSpot = spots[num2];
						break;
					}
				}
			}
			return taxiSpot;
		}

		private void Awake()
		{
			player = ServiceLocator.GetGameObject("Player").transform;
			spots = GetComponentsInChildren<TaxiSpot>();
		}

		private void Start()
		{
			for (int i = 0; i < spots.Length; i++)
			{
				spots[i].enabled = false;
			}
			hospitalSpot.enabled = false;
		}
	}
}
