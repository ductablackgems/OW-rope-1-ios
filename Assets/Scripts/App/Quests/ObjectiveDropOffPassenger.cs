using App.AI;
using App.Player;
using App.Spawn;
using App.Vehicles;
using UnityEngine;

namespace App.Quests
{
	public class ObjectiveDropOffPassenger : GameplayObjective
	{
		private enum DropOffState
		{
			Waiting,
			Moving,
			Finished
		}

		[Header("Objective Drop Off Passenger")]
		[SerializeField]
		private float minDistance = 2f;

		[SerializeField]
		private Transform passengerGoTo;

		[SerializeField]
		private bool runToPosition;

		[SerializeField]
		private bool despawnPassenger;

		private CarDriver passenger;

		private NavmeshWalker walker;

		private FreeWalkAIModule freeWalk;

		private DropOffState state;

		private HumanSpawner spawner;

		protected override void OnInitialized()
		{
			base.OnInitialized();
			spawner = ServiceLocator.Get<HumanSpawner>();
		}

		protected override void OnActivated()
		{
			base.OnActivated();
			VehicleComponents vehicle = GetVehicle();
			passenger = vehicle.passenger.GetComponent<CarDriver>();
			if (!(passengerGoTo == null))
			{
				walker = passenger.GetComponentInChildren<NavmeshWalker>();
				freeWalk = walker.GetComponent<FreeWalkAIModule>();
			}
		}

		protected override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			if (data.IsNPCKilled)
			{
				Fail();
				return;
			}
			if (data.IsVehicleKilled)
			{
				Fail();
				return;
			}
			switch (state)
			{
			case DropOffState.Waiting:
				UpdateDropOff();
				break;
			case DropOffState.Moving:
				UpdateMoving();
				break;
			case DropOffState.Finished:
				Finish();
				break;
			}
		}

		protected override void OnReset()
		{
			base.OnReset();
			state = DropOffState.Waiting;
			freeWalk = null;
			walker = null;
		}

		protected override void OnDeactivated()
		{
			base.OnDeactivated();
			if (freeWalk != null)
			{
				freeWalk.enabled = true;
				freeWalk = null;
			}
			if (despawnPassenger && passenger != null)
			{
				spawner.Despawn(passenger.gameObject);
			}
			walker = null;
			passenger = null;
		}

		private VehicleComponents GetVehicle()
		{
			VehicleComponents vehicle = base.Player.PlayerMonitor.GetVehicle();
			if (vehicle == null)
			{
				return null;
			}
			if (vehicle.driver == null)
			{
				return null;
			}
			return vehicle;
		}

		private void UpdateDropOff()
		{
			VehicleComponents vehicle = GetVehicle();
			if (!(vehicle == null) && !(Vector3.Distance(vehicle.transform.position, base.Position) > minDistance) && vehicle.OpenableVelocity())
			{
				passenger.Stop();
				state = ((!(passengerGoTo == null)) ? DropOffState.Moving : DropOffState.Finished);
			}
		}

		private void UpdateMoving()
		{
			if (walker.TargetTransform != passengerGoTo)
			{
				freeWalk.enabled = false;
				NavmeshWalkerSpeed speed = (!runToPosition) ? NavmeshWalkerSpeed.Walk : NavmeshWalkerSpeed.RunFast;
				walker.FollowTransform(this, passengerGoTo, speed);
			}
			if (!(Vector3.Distance(walker.transform.position, passengerGoTo.position) > 3f))
			{
				state = DropOffState.Finished;
			}
		}
	}
}
