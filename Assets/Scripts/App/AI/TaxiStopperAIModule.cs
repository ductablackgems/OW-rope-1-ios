using App.Missions;
using App.Player;
using App.Spawn;
using App.Vehicles.Car;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace App.AI
{
	public class TaxiStopperAIModule : AbstractAIScript, IResetable
	{
		[NonSerialized]
		public Transform targetHandle;

		[NonSerialized]
		public Rigidbody carRigidbody;

		[NonSerialized]
		public int missionKey;

		private NavMeshAgent agent;

		private NavmeshWalker walker;

		private CarDriver carDriver;

		private VehicleHandleSensor vehicleHandleSensor;

		private Transform player;

		private TaxiSpots taxiSpots;

		private MissionManager missionManager;

		public void ResetStates()
		{
			targetHandle = null;
			carRigidbody = null;
		}

		private void Awake()
		{
			agent = base.ComponentsRoot.GetComponentSafe<NavMeshAgent>();
			walker = this.GetComponentSafe<NavmeshWalker>();
			carDriver = base.ComponentsRoot.GetComponentSafe<CarDriver>();
			vehicleHandleSensor = base.ComponentsRoot.GetComponentSafe<VehicleHandleSensor>();
			player = ServiceLocator.GetGameObject("Player").transform;
			taxiSpots = ServiceLocator.Get<TaxiSpots>();
			missionManager = ServiceLocator.Get<MissionManager>();
		}

		private void OnEnable()
		{
			vehicleHandleSensor.SetTargetHandle(targetHandle);
		}

		private void OnDisable()
		{
			if (carDriver.Running())
			{
				carDriver.Stop();
			}
			walker.Stop();
		}

		private void Update()
		{
			if (carRigidbody == null)
			{
				return;
			}
			if ((player.position - carRigidbody.transform.position).magnitude > 30f)
			{
				if (carDriver.Running())
				{
					carDriver.Stop();
				}
				carRigidbody = null;
				targetHandle = null;
			}
			else if (carDriver.Runnable(useTargetTrigger: true))
			{
				carDriver.Run(onlyThrowOutDriver: false, useTargetTrigger: true, isPassenger: true);
			}
			else if (carDriver.Running())
			{
				if (!missionManager.CompareMission(missionKey))
				{
					carDriver.Stop();
					carRigidbody = null;
					targetHandle = null;
				}
				else if (taxiSpots.TargetIsDestination && (taxiSpots.TargetSpot.transform.position - base.ComponentsRoot.position).magnitude < 10f && carRigidbody.velocity.magnitude * 3.6f < 4f)
				{
					carDriver.Stop();
					missionManager.FinishMission(missionKey, success: true);
				}
			}
			else if (!missionManager.CompareMission(missionKey))
			{
				carRigidbody = null;
				targetHandle = null;
			}
			else if ((taxiSpots.TargetSpot.transform.position - carRigidbody.transform.position).magnitude < 10f && carRigidbody.velocity.magnitude * 3.6f < 4f)
			{
				if (walker.TargetTransform != targetHandle)
				{
					walker.FollowTransform(this, targetHandle, NavmeshWalkerSpeed.Walk);
				}
			}
			else
			{
				walker.Stop();
			}
		}
	}
}
