using App.AI;
using App.Vehicles.Car.Navigation.Modes.Curve;
using App.Vehicles.Car.Navigation.Roads;
using UnityEngine;

namespace App.Vehicles.Car.Navigation
{
	public class AIVehicleController : AbstractAIScript
	{
		private AIVehicleModesHandler aiModesHandler;

		private CurveVehicleController curveController;

		private TargettingVehicleController targettingController;

		private RoadSeeker seeker;

		private VehicleStuckManager stuckManager;

		private VehiclesManager vehiclesManager;

		private void Awake()
		{
			aiModesHandler = this.GetComponentSafe<AIVehicleModesHandler>();
			curveController = this.GetComponentSafe<CurveVehicleController>();
			targettingController = this.GetComponentSafe<TargettingVehicleController>();
			seeker = this.GetComponentSafe<RoadSeeker>();
			stuckManager = this.GetComponentSafe<VehicleStuckManager>();
			vehiclesManager = ServiceLocator.Get<VehiclesManager>();
			curveController.OnStuck += OnCurveControllerStuck;
			targettingController.OnCloseToRoute += OnCloseToRoute;
			stuckManager.OnImobileObstacle += OnImobileObstacle;
			vehiclesManager.Register(seeker);
		}

		private void OnDestroy()
		{
			curveController.OnStuck -= OnCurveControllerStuck;
			targettingController.OnCloseToRoute -= OnCloseToRoute;
			stuckManager.OnImobileObstacle -= OnImobileObstacle;
			vehiclesManager.Unregister(seeker);
		}

		private void OnCurveControllerStuck()
		{
			OnImobileObstacle();
		}

		private void OnCloseToRoute()
		{
			aiModesHandler.SetMode(AIVehicleMode.Common);
		}

		private void OnImobileObstacle()
		{
			if (aiModesHandler.mode == AIVehicleMode.Common)
			{
				TrafficRoute route;
				RoadSegment segment;
				Vector3 forwardPosition = seeker.GetForwardPosition(20f, out route, out segment);
				targettingController.SetTarget(forwardPosition, TargetMode.RoadPoint);
				aiModesHandler.SetMode(AIVehicleMode.Target);
			}
		}
	}
}
