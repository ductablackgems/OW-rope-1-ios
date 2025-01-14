using App.AI;
using App.Vehicles.Car.Navigation.Modes.Curve;
using App.Vehicles.Car.Navigation.Roads;
using System;
using UnityEngine;

namespace App.Vehicles.Car.Navigation
{
	public class TargettingVehicleController : AbstractAIScript
	{
		private const float ForceControlInterval = 2f;

		private const float SharpAngle = 30f;

		private const float SlowAngle = 30f;

		private const float CloseToRouteDistance = 5f;

		public float expectedSpeed = 14f;

		public float slowSpeed = 6f;

		public float reverseDuration = 2.5f;

		public float reverseSteerPerMinute = 30f;

		private Rigidbody _rigidbody;

		private IVehicleController vehicleController;

		private CarNavigationRaycasts raycasts;

		private SkidBrake skidBrake;

		private RoadSeeker roadSeeker;

		private VehicleStuckManager stuckManager;

		private float lastVerticalInput = 999f;

		private float lastHorizontalInput = 999f;

		private float lastHandbrakeInput;

		private float reverseHorizontalInput;

		private float escapeDirection;

		private TrafficRoute targetRoute;

		private RoadSegment targetSegment;

		private DurationTimer forceControlTimer = new DurationTimer(useFixedTime: true);

		private DurationTimer reverseTimer = new DurationTimer(useFixedTime: true);

		private StuckSensor stuckSensor = new StuckSensor();

		private FrameCounter updateSeekerCounter = new FrameCounter(12);

		public bool HasTarget
		{
			get;
			private set;
		}

		public Transform TargetTransform
		{
			get;
			private set;
		}

		public Vector3 Target
		{
			get;
			private set;
		}

		public TargetMode TargetMode
		{
			get;
			private set;
		}

		public event Action OnCloseToRoute;

		public void SetTarget(Transform targetTransform)
		{
			HasTarget = (targetTransform != null);
			TargetTransform = targetTransform;
			Target = Vector3.zero;
			TargetMode = TargetMode.Default;
			targetRoute = null;
			targetSegment = null;
		}

		public void SetTarget(Vector3 target, TargetMode targetMode, TrafficRoute targetRoute = null, RoadSegment targetSegment = null)
		{
			HasTarget = true;
			TargetTransform = null;
			Target = target;
			TargetMode = targetMode;
			updateSeekerCounter.Reset();
		}

		public void ClearTarget()
		{
			HasTarget = false;
			TargetTransform = null;
			Target = Vector3.zero;
		}

		private void Awake()
		{
			_rigidbody = base.ComponentsRoot.GetComponentSafe<Rigidbody>();
			vehicleController = base.ComponentsRoot.GetComponentSafe<IVehicleController>();
			raycasts = base.ComponentsRoot.GetComponentInChildrenSafe<CarNavigationRaycasts>();
			skidBrake = this.GetComponentSafe<SkidBrake>();
			roadSeeker = this.GetComponentSafe<RoadSeeker>();
			stuckManager = this.GetComponentSafe<VehicleStuckManager>();
		}

		private void OnEnable()
		{
			forceControlTimer.Run(2f);
			stuckSensor.Clear();
		}

		private void OnDisable()
		{
			stuckManager.ClearImobile();
		}

		private void FixedUpdate()
		{
			if ((TargetMode == TargetMode.RoadPoint || TargetMode == TargetMode.Road) && updateSeekerCounter.Fetch())
			{
				roadSeeker.UpdateStates();
				if (roadSeeker.Route.GetRelativeDistance(roadSeeker.RoadSegment, Target, roadSeeker.referencePoint.position).z < 3f)
				{
					Target = roadSeeker.GetForwardPosition(5f, out TrafficRoute _, out RoadSegment _);
				}
			}
			Vector3 vector = (TargetTransform == null) ? Target : TargetTransform.position;
			float num = Vector3.Angle(base.ComponentsRoot.forward, vector - base.ComponentsRoot.position);
			float num2 = num;
			if (base.ComponentsRoot.InverseTransformPoint(vector).x < 0f)
			{
				num *= -1f;
			}
			float num3 = (TargetMode != 0 || num2 > 30f) ? slowSpeed : expectedSpeed;
			float z = base.ComponentsRoot.InverseTransformDirection(_rigidbody.velocity).z;
			float num4 = (z < num3) ? 0.8f : 0f;
			float num5 = (!(z < num3)) ? (-1) : 0;
			int num6 = (skidBrake.IsBreaking() || !HasTarget) ? 1 : 0;
			raycasts.SetDelay(Mathf.Lerp(1.2f, 0f, z / 15f));
			if (Mathf.Abs(num3 - z) < 0.3f || skidBrake.IsBreaking())
			{
				num4 = 0f;
			}
			float num7 = 0f;
			if (!reverseTimer.InProgress() && z < 0.1f && raycasts.frontMiddle.Hit() && raycasts.frontMiddle.GetHitDistance() < 1.2f)
			{
				reverseTimer.Run(reverseDuration);
				reverseHorizontalInput = 0f;
			}
			if (stuckSensor.Stuck())
			{
				stuckSensor.Clear();
				reverseTimer.Run(reverseDuration);
			}
			float absoluteSpeed = Mathf.Abs(z);
			if (reverseTimer.InProgress())
			{
				stuckSensor.Clear();
			}
			else
			{
				stuckSensor.Update(num4, absoluteSpeed);
			}
			if (reverseTimer.InProgress())
			{
				if (MathHelper.HitProbabilityPerMinute(reverseSteerPerMinute, Time.fixedDeltaTime))
				{
					reverseHorizontalInput = UnityEngine.Random.Range(-1, 1);
				}
				if (raycasts.backMiddle.Hit())
				{
					num4 = 0f;
					num5 = -1f;
				}
				else if (z < -5f)
				{
					num4 = 0f;
					num5 = -0.3f;
				}
				else
				{
					num4 = -1f;
				}
				num7 = reverseHorizontalInput;
			}
			else
			{
				num7 = GetForwardSteer(num, z, out bool slowDown);
				if (slowDown)
				{
					num4 = 0f;
					num5 = -1f;
				}
				if (raycasts.frontMiddle.Hit() || raycasts.frontLeft0.GetHitDistance() < 0.4f || raycasts.frontRight0.GetHitDistance() < 0.4f)
				{
					if (raycasts.frontMiddle.GetHitDistance() < 1.2f)
					{
						num4 = 0f;
						num5 = -1f;
					}
					else if (raycasts.frontMiddle.GetHitDistance() < 3f && z > slowSpeed)
					{
						num4 = 0f;
						num5 = -1f;
					}
				}
			}
			stuckManager.UpdateStates(num4, absoluteSpeed);
			if (num4 != lastVerticalInput || Mathf.Abs(lastHorizontalInput - num7) > 0.1f || (float)num6 != lastHandbrakeInput || forceControlTimer.Done())
			{
				num5 = ((num6 > 0) ? (-1f) : num5);
				lastVerticalInput = num4;
				lastHorizontalInput = num7;
				lastHandbrakeInput = num6;
				vehicleController.Move(num7, num4, num5, num6);
				forceControlTimer.Run(2f);
			}
			else
			{
				vehicleController.SteerHelper();
			}
			if (this.OnCloseToRoute == null || (TargetMode != TargetMode.Road && TargetMode != TargetMode.RoadPoint) || !(roadSeeker.Route != null) || !(Mathf.Abs(roadSeeker.Route.GetSegmentRemainDistance(roadSeeker.RoadSegment, roadSeeker.referencePoint.position).x) < 5f))
			{
				return;
			}
			if (TargetMode == TargetMode.RoadPoint)
			{
				if (Vector3.Distance(roadSeeker.referencePoint.position, Target) < 5f && this.OnCloseToRoute != null)
				{
					this.OnCloseToRoute();
				}
			}
			else
			{
				this.OnCloseToRoute();
			}
		}

		private void OnDrawGizmos()
		{
			if (TargetMode != 0)
			{
				Transform transform = null;
				Gizmos.color = ((transform == null || !transform.Equals(base.ComponentsRoot)) ? Color.white : Color.yellow);
				Gizmos.DrawSphere(Target, 0.5f);
			}
		}

		private float GetForwardSteer(float angle, float currentSpeed, out bool slowDown)
		{
			float result = 0f;
			slowDown = false;
			if (raycasts.frontMiddle.Hit() && raycasts.frontMiddle.GetHitDistance() < 2.5f)
			{
				bool flag = !raycasts.frontRight0.Hit() && !raycasts.frontRight1.Hit();
				bool flag2 = flag || ((!raycasts.frontRight0.Hit() || raycasts.frontRight0.GetHitDistance() > 4f) && (!raycasts.frontRight1.Hit() || raycasts.frontRight1.GetHitDistance() > 2f));
				bool flag3 = !raycasts.frontLeft0.Hit() && !raycasts.frontLeft1.Hit();
				bool flag4 = flag3 || ((!raycasts.frontLeft0.Hit() || raycasts.frontLeft0.GetHitDistance() > 4f) && (!raycasts.frontLeft1.Hit() || raycasts.frontLeft1.GetHitDistance() > 2f));
				if (escapeDirection > 0f && flag2)
				{
					result = 1f;
				}
				else if (escapeDirection < 0f && flag4)
				{
					result = -1f;
				}
				else if (angle > 0f && flag)
				{
					result = 1f;
				}
				else if (angle < 0f && flag3)
				{
					result = -1f;
				}
				else if (angle > -25f && flag)
				{
					result = 1f;
				}
				else if (angle < 25f && flag3)
				{
					result = -1f;
				}
				else if (angle > 0f && flag2)
				{
					result = 1f;
				}
				else if (angle < 0f && flag4)
				{
					result = -1f;
				}
				else if (angle > -25f && flag2)
				{
					result = 1f;
				}
				else if (angle < 25f && flag4)
				{
					result = -1f;
				}
				else if (flag2)
				{
					result = 1f;
				}
				else if (flag4)
				{
					result = -1f;
				}
				else
				{
					float num = Mathf.Min(raycasts.frontLeft0.GetHitDistance(), raycasts.frontLeft1.GetHitDistance());
					float num2 = Mathf.Min(raycasts.frontRight0.GetHitDistance(), raycasts.frontRight1.GetHitDistance());
					if (num2 > num && num2 > 0.8f)
					{
						result = 1f;
					}
					else if (num > 0.8f)
					{
						result = -1f;
					}
					if (currentSpeed > slowSpeed)
					{
						slowDown = true;
					}
				}
				escapeDirection = result;
			}
			else if ((raycasts.frontLeft0.Hit() || raycasts.frontLeft1.Hit()) && (!raycasts.frontRight0.Hit() || !raycasts.frontRight1.Hit()))
			{
				escapeDirection = 1f;
				result = ((raycasts.frontLeft1.Hit() && raycasts.frontLeft1.GetHitDistance() > 1.5f) ? 0.6f : 1f);
			}
			else if ((raycasts.frontRight0.Hit() || raycasts.frontRight1.Hit() || raycasts.frontMiddle.Hit()) && (!raycasts.frontLeft0.Hit() || !raycasts.frontLeft1.Hit()))
			{
				escapeDirection = -1f;
				result = ((raycasts.frontRight1.Hit() && raycasts.frontLeft1.GetHitDistance() > 1.5f) ? (-0.6f) : (-1f));
			}
			else
			{
				escapeDirection = 0f;
				result = Mathf.Lerp(-1f, 1f, (angle + 30f) / 30f / 2f);
			}
			return result;
		}
	}
}
