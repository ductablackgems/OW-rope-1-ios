using FluffyUnderware.Curvy;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace App.Vehicles.Airplane.AI
{
	public class AirplaneAIController : MonoBehaviour, IAirplaneController
	{
		private FollowSpline follower;

		private AirTraffic path;

		private float maxSpeed;

		private bool isRunning;

		private Action<AirplaneAIController> pathFinished;

		public bool IsFinished => GetIsFinished();

		public bool SetOrientation
		{
			get
			{
				return follower.SetOrientation;
			}
			set
			{
				follower.SetOrientation = value;
			}
		}

		bool IAirplaneController.IsActive => isRunning;

		float IAirplaneController.EnginePower => GetSpeed();

		float IAirplaneController.MaxEnginePower => maxSpeed;

		float IAirplaneController.ForwardSpeed => GetSpeed();

		float IAirplaneController.Throttle => GetThrottle();

		public void Initialize()
		{
			follower = GetComponent<FollowSpline>();
			follower.enabled = false;
			maxSpeed = follower.Speed;
		}

		public void Run(AirTraffic path, Action<AirplaneAIController> pathFinished)
		{
			this.path = path;
			this.pathFinished = pathFinished;
			isRunning = true;
			follower.enabled = true;
			follower.Spline = path.Spline;
			follower.Clamping = (path.IsLoop ? CurvyClamping.Loop : CurvyClamping.Clamp);
			base.transform.rotation = path.Spline[0].transform.rotation;
		}

		public void Pause(bool isPaused)
		{
			follower.enabled = !isPaused;
		}

		public void Clean()
		{
			path = null;
			pathFinished = null;
			isRunning = false;
		}

		private void Update()
		{
			if (isRunning)
			{
				UpdateState();
				UpdateRotation();
				UpdateVelocity();
			}
		}

		private bool GetIsFinished()
		{
			if (path == null)
			{
				return true;
			}
			if (path.IsLoop)
			{
				return false;
			}
			List<CurvySplineSegment> segments = path.Spline.Segments;
			CurvySplineSegment y = segments[segments.Count - 1];
			if (follower.CurrentSegment != y)
			{
				return false;
			}
			return follower.CurrentTF == 1f;
		}

		private void UpdateState()
		{
			if (IsFinished)
			{
				pathFinished.SafeInvoke(this);
				Clean();
			}
		}

		private void UpdateRotation()
		{
			if (!(follower.CurrentSegment == null))
			{
				Quaternion orientationFast = follower.Spline.GetOrientationFast(follower.CurrentTF);
				Vector3 vector = follower.CurrentSegment.InterpolateUserValue(follower.CurrentSegmentF, 0);
				Vector3 eulerAngles = orientationFast.eulerAngles;
				eulerAngles.x = vector.x;
				base.transform.rotation = Quaternion.Euler(eulerAngles);
			}
		}

		private void UpdateVelocity()
		{
			if (!(follower.CurrentSegment == null))
			{
				float z = follower.CurrentSegment.InterpolateUserValue(follower.CurrentSegmentF, 1).z;
				float speed = (z > 0f) ? (z * maxSpeed) : maxSpeed;
				follower.Speed = speed;
			}
		}

		private float GetThrottle()
		{
			if (maxSpeed == 0f)
			{
				return 0f;
			}
			return Mathf.Clamp01(GetSpeed() / maxSpeed);
		}

		private float GetSpeed()
		{
			if (follower == null)
			{
				return 0f;
			}
			if (!follower.enabled)
			{
				return 0f;
			}
			return follower.Speed;
		}
	}
}
