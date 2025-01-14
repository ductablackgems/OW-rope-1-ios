using App.AI;
using App.Vehicles.Car.Navigation.Modes.Curve;
using App.Vehicles.Car.Navigation.Roads;
using System;
using UnityEngine;

namespace App.Vehicles.Car.Navigation
{
	public class CurveVehicleController : AbstractAIScript
	{
		private const float MinSlowSpeedAngle = 40f;

		private const float MinSlowSpeedRouteDistance = 2.5f;

		private const float UpdateInterval = 2f;

		private const float MaxStraightOffset = 0.35f;

		private const float MaxStraightAngle = 1.5f;

		private const float MinStraightAngle = 1.5f;

		private const float MaxRouteOffset = 0.2f;

		public float expectedSpeed = 25f;

		public float slowSpeed = 6f;

		public float speedChangeFactor = 0.5f;

		public float targetAngleFactor = 30f;

		[Space]
		public float steerSmoothnes = 20f;

		[Space]
		public float minSteerAdjustSpeed = 1.5f;

		public float maxSteerAdjustSpeed = 25f;

		public float minSteerAdjustCoeff = 0.2f;

		private Rigidbody _rigidbody;

		public IVehicleController vehicleController;

		private RoadSeeker roadSeeker;

		private VehicleStuckManager stuckManager;

		private AICarBoxSensor boxSensor;

		private float lastVerticalInput = 999f;

		private float lastHorizontalInput = 999f;

		private float lastBrakeInput;

		private bool drivingStraight;

		private DurationTimer controlUpdateTimer = new DurationTimer(useFixedTime: true);

		private DurationTimer preventStuckEventTimer = new DurationTimer();

		private StuckSensor stuckSensor = new StuckSensor();

		private Transform nextVehicle;

		private FrameCounter nextVehicleFrameCounter = new FrameCounter(40, 40);

		private FrameCounter sleepCounter = new FrameCounter(75);

		private FrameCounter updateSeekerCounter = new FrameCounter(5, 5);

		private bool sleeping;

		public event Action OnStuck;

		private void Awake()
		{
			_rigidbody = base.ComponentsRoot.GetComponentSafe<Rigidbody>();
			vehicleController = base.ComponentsRoot.GetComponentSafe<IVehicleController>();
			roadSeeker = this.GetComponentSafe<RoadSeeker>();
			stuckManager = this.GetComponentSafe<VehicleStuckManager>();
			boxSensor = base.ComponentsRoot.GetComponentInChildren<AICarBoxSensor>();
		}

		private void OnEnable()
		{
			controlUpdateTimer.FakeDone(2f);
			preventStuckEventTimer.Run(2f);
			stuckSensor.Clear();
			updateSeekerCounter.PrepareToTrueFetch();
		}

		private void OnDisable()
		{
			if (sleeping)
			{
				sleeping = false;
				stuckManager.MarkWakeUp();
			}
		}

		private void FixedUpdate()
		{
			if (sleeping && !sleepCounter.Fetch())
			{
				return;
			}
			float z = base.ComponentsRoot.InverseTransformDirection(_rigidbody.velocity).z;
			float num = Mathf.Abs(z);
			Vector3 vector = roadSeeker.GetSegmentRemainDistance();
			if (updateSeekerCounter.Fetch() || sleeping || vector.z < 0f)
			{
				vector = roadSeeker.UpdateStates(vector);
			}
			float targetSpeed = GetTargetSpeed(vector);
			float num2 = 1001f;
			if (sleeping)
			{
				if (num > 0.05f)
				{
					sleeping = false;
				}
				else
				{
					UpdateNextVehicle();
					nextVehicleFrameCounter.Reset();
					num2 = ((nextVehicle == null) ? 999f : Vector3.Distance(nextVehicle.position, base.ComponentsRoot.position));
					if (targetSpeed != 0f && !boxSensor.Hit())
					{
						sleeping = false;
					}
				}
				if (sleeping)
				{
					stuckManager.MarkSleeping();
					return;
				}
				stuckManager.MarkWakeUp();
			}
			else
			{
				if (num < 0.01f && (targetSpeed == 0f || boxSensor.Hit()))
				{
					sleepCounter.Reset();
					sleeping = true;
					stuckSensor.Clear();
					stuckManager.MarkSleeping();
					UpdateVehicleController(0f, 0f, -1f);
					return;
				}
				if (!preventStuckEventTimer.InProgress() && stuckSensor.Stuck())
				{
					stuckSensor.Clear();
					TryInvokeDistantStuck();
					return;
				}
			}
			float num3 = (z < targetSpeed) ? 0.8f : 0f;
			if (nextVehicleFrameCounter.Fetch())
			{
				UpdateNextVehicle();
			}
			if (num2 > 1000f)
			{
				num2 = ((nextVehicle == null) ? 999f : Vector3.Distance(nextVehicle.position, base.ComponentsRoot.position));
			}
			float num4 = (z >= targetSpeed) ? (-0.4f) : 0f;
			float distanceAngle = roadSeeker.GetDistanceAngle(base.ComponentsRoot.forward);
			float num5 = Mathf.Abs(distanceAngle);
			if (((num2 < 20f || num5 > 40f || Mathf.Abs(vector.x) > 2.5f) && z > slowSpeed) || (boxSensor != null && boxSensor.Hit()))
			{
				num3 = 0f;
				num4 = -1f;
			}
			else if (Mathf.Abs(targetSpeed - z) < 0.3f)
			{
				num3 = 0f;
				num4 = 0f;
			}
			float num6 = 0f;
			bool flag = false;
			if (drivingStraight)
			{
				if (Mathf.Abs(vector.x) > 0.35f)
				{
					drivingStraight = false;
					vehicleController.UpdateOldRotation();
				}
				else if (num5 > 1.5f)
				{
					drivingStraight = false;
					vehicleController.UpdateOldRotation();
				}
				else if (num5 < 1.5f)
				{
					Vector3 eulerAngles = base.ComponentsRoot.transform.eulerAngles;
					if (roadSeeker.Route.polarity == TrafficRoutePolarity.Positive)
					{
						eulerAngles.y = roadSeeker.RoadSegment.transform.eulerAngles.y;
					}
					else
					{
						eulerAngles.y = 180f + roadSeeker.RoadSegment.transform.eulerAngles.y;
					}
					base.ComponentsRoot.transform.eulerAngles = eulerAngles;
				}
			}
			else if (Mathf.Abs(distanceAngle) < 1.5f && Mathf.Abs(vector.x) < 0.35f)
			{
				drivingStraight = true;
				Vector3 eulerAngles2 = base.ComponentsRoot.transform.eulerAngles;
				if (roadSeeker.Route.polarity == TrafficRoutePolarity.Positive)
				{
					eulerAngles2.y = roadSeeker.RoadSegment.transform.eulerAngles.y;
				}
				else
				{
					eulerAngles2.y = 180f + roadSeeker.RoadSegment.transform.eulerAngles.y;
				}
				base.ComponentsRoot.transform.eulerAngles = eulerAngles2;
				flag = true;
			}
			else
			{
				float num7 = 1f;
				if (z > maxSteerAdjustSpeed)
				{
					num7 = minSteerAdjustCoeff;
				}
				else if (z > minSteerAdjustSpeed)
				{
					num7 = Mathf.Lerp(minSteerAdjustCoeff, 1f, (maxSteerAdjustSpeed - z) / (maxSteerAdjustSpeed - minSteerAdjustSpeed));
				}
				float allowedAngle = GetAllowedAngle(vector, num7);
				if (distanceAngle < allowedAngle)
				{
					num6 = Mathf.Lerp(0f, 1f, (allowedAngle - distanceAngle) / steerSmoothnes);
				}
				else if (distanceAngle > allowedAngle)
				{
					num6 = Mathf.Lerp(0f, -1f, (distanceAngle - allowedAngle) / steerSmoothnes);
				}
				num6 *= num7;
			}
			stuckSensor.Update(num3, num);
			if ((vehicleController.MoveEachFrame() | flag) || num3 != lastVerticalInput || num4 != lastBrakeInput || Mathf.Abs(lastHorizontalInput - num6) > 0.08f || controlUpdateTimer.Done())
			{
				UpdateVehicleController(num3, num6, num4);
			}
			if (!drivingStraight)
			{
				vehicleController.SteerHelper();
			}
		}

		private void UpdateVehicleController(float v, float h, float brakeInput)
		{
			lastVerticalInput = v;
			lastHorizontalInput = h;
			lastBrakeInput = brakeInput;
			vehicleController.Move(h, v, brakeInput, 0f);
			controlUpdateTimer.Run(2f);
		}

		private float GetTargetSpeed(Vector3 remainDistance)
		{
			float tresholdSpeed = roadSeeker.GetTresholdSpeed(expectedSpeed);
			float nextTresholdSpeed = roadSeeker.GetNextTresholdSpeed(expectedSpeed);
			if (nextTresholdSpeed >= tresholdSpeed)
			{
				return tresholdSpeed;
			}
			float value = nextTresholdSpeed + remainDistance.z * speedChangeFactor;
			value = Mathf.Clamp(value, nextTresholdSpeed, tresholdSpeed);
			if (value < 1f)
			{
				value = ((!(remainDistance.z < 1f)) ? 1f : 0f);
			}
			return value;
		}

		private float GetAllowedAngle(Vector3 remainDistance, float steerCoeff)
		{
			if (Mathf.Abs(remainDistance.x) < 0.2f)
			{
				return 0f;
			}
			if (remainDistance.x < 0f)
			{
				return Mathf.Clamp((remainDistance.x + 0.2f) * (0f - targetAngleFactor) * steerCoeff, -80f, 80f);
			}
			return Mathf.Clamp((remainDistance.x - 0.2f) * (0f - targetAngleFactor) * steerCoeff, -80f, 80f);
		}

		private void TryInvokeDistantStuck()
		{
			if (this.OnStuck != null)
			{
				this.OnStuck();
			}
		}

		private void UpdateNextVehicle()
		{
			nextVehicle = roadSeeker.GetNextVehicle(out RoadSeeker nextSeeker);
			roadSeeker.RouteReservationEnabled = (!sleeping || nextSeeker == null || !roadSeeker.RoadSegment.Equals(nextSeeker.RoadSegment));
		}
	}
}
