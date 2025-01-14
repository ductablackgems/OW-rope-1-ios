using App.Player.Definition;
using App.Player.Rope;
using App.Util;
using App.Vehicles;
using App.Vehicles.Bicycle;
using App.Vehicles.Car;
using App.Weapons;
using UnityEngine;

namespace App.Player
{
	[RequireComponent(typeof(PlayerAnimatorHandler))]
	public class RopeController : MonoBehaviour, ICharacterModule
	{
		public float pullToRopeSpeed;

		public float pullHumanSpeed = 30f;

		public float pullStreetVehicleSpeed = 10f;

		public RopePullCurve pullCurve;

		public float maxTargetDistance;

		public AudioSource ropeAudioSource;

		public AudioClip[] ropeClips;

		public Transform ropeOuterPosition;

		public Transform headPosition;

		public Transform bodyCenter;

		public float rotationSpeed = 100f;

		public LayerMask targetMask;

		private Rigidbody _rigidbody;

		private PlayerAnimatorHandler animatorHandler;

		private RagdollHelper ragdollHelper;

		private CharacterRotator characterRotator;

		private LineRenderer lineRenderer;

		private Transform cameraPosition;

		private VirtualTarget virtualTarget;

		private RaycastHit hit;

		private Transform pullingBoneTransform;

		private Rigidbody pullingBoneRigidbody;

		private float pullRopeStopDistance;

		private float pullRopeSpeed;

		private Health health;

		private Vector3 landingAirPosition = new Vector3(90f, 0f, 0f);

		private bool fixRotationAroundHead;

		private Quaternion beforeRunRotation;

		private Quaternion targetRotation;

		private Transform rotationPoint;

		private DurationTimer rotationDelayTimer = new DurationTimer();

		private DurationTimer fakePullTimer = new DurationTimer();

		public Vector3 particleDir;

		private float t;

		private float distance;

		public bool FixingRotation
		{
			get;
			private set;
		}

		public void Run(RaycastHit hit)
		{
			this.hit = hit;
			Run();
		}

		public void Run()
		{
			WhoIsResult whoIsResult = WhoIs.Resolve(hit.collider, WhoIs.Masks.PullableByRope);
			float num = whoIsResult.IsEmpty ? 0f : (whoIsResult.transform.position - base.transform.position).magnitude;
			if (!whoIsResult.IsEmpty && whoIsResult.Compare(WhoIs.Entities.RagdollableEnemy) && whoIsResult.transform.parent != null)
			{
				WhoIsResult whoIsResult2 = WhoIs.ResolveGameObject(whoIsResult.transform.root.gameObject, WhoIs.Masks.AllVehicles);
				if (!whoIsResult2.IsEmpty)
				{
					whoIsResult = whoIsResult2;
				}
			}
			bool flag = false;
			if (!whoIsResult.IsEmpty && num <= 30f && whoIsResult.Compare(WhoIs.Entities.Vehicle))
			{
				VehicleComponents component = whoIsResult.transform.GetComponent<VehicleComponents>();
				if (component != null && component.type == VehicleType.Tank)
				{
					flag = true;
				}
			}
			if ((whoIsResult.IsEmpty | flag) || (num > 30f && whoIsResult.Compare(WhoIs.Entities.Vehicle)))
			{
				animatorHandler.PullToRope = true;
				lineRenderer.SetPosition(1, hit.point);
			}
			else
			{
				RagdollHelper ragdollHelper = null;
				if (whoIsResult.Compare(WhoIs.Masks.AllStreetVehicles))
				{
					ragdollHelper = HandleStreetVehiclePullStart(whoIsResult);
				}
				else if (whoIsResult.Compare(WhoIs.Entities.Vehicle))
				{
					ragdollHelper = HandleVehiclePullStart(whoIsResult);
				}
				else
				{
					ragdollHelper = whoIsResult.gameObject.GetComponentSafe<RagdollHelper>();
					ragdollHelper.Ragdolled = true;
				}
				if (ragdollHelper == null)
				{
					pullRopeStopDistance = 5f;
					pullRopeSpeed = pullStreetVehicleSpeed;
				}
				else
				{
					pullingBoneTransform = ragdollHelper.HipsTransform;
					float totalDistance = Vector3.Distance(pullingBoneTransform.position, ropeOuterPosition.position);
					pullRopeStopDistance = pullCurve.GetHumanStopDistance(totalDistance);
					pullRopeSpeed = pullHumanSpeed;
					ragdollHelper.GetComponentSafe<Health>().ApplyDamage(5f, 1, base.gameObject);
				}
				animatorHandler.PullRope = true;
				pullingBoneRigidbody = pullingBoneTransform.GetComponentSafe<Rigidbody>();
				lineRenderer.SetPosition(1, pullingBoneTransform.position);
			}
			_rigidbody.velocity = Vector3.zero;
			lineRenderer.enabled = true;
			base.transform.LookAt(new Vector3(hit.point.x, base.transform.position.y, hit.point.z));
			animatorHandler.Roll = false;
			characterRotator.Interrupt();
			if (animatorHandler.PullToRope)
			{
				_rigidbody.useGravity = false;
				t = 0f;
				pullToRopeSpeed = 20f;
				beforeRunRotation = base.transform.rotation;
				rotationDelayTimer.Run(0.1f);
				float x = Vector3.Angle(Vector3.up, hit.point - base.transform.position);
				targetRotation = Quaternion.Euler(x, base.transform.rotation.eulerAngles.y, 0f);
				rotationPoint = bodyCenter;
				FixingRotation = true;
				fixRotationAroundHead = false;
			}
			ropeAudioSource.PlayOneShot(ropeClips[Random.Range(0, ropeClips.Length)]);
		}

		public bool Running()
		{
			if (!animatorHandler.PullToRope)
			{
				return animatorHandler.PullRope;
			}
			return true;
		}

		public void Stop()
		{
			if (!Running())
			{
				return;
			}
			if (animatorHandler.PullToRope)
			{
				if (fixRotationAroundHead)
				{
					targetRotation = Quaternion.LookRotation(new Vector3(0f - hit.normal.x, 0f, 0f - hit.normal.z));
					rotationPoint = headPosition;
					FixingRotation = true;
				}
				else
				{
					targetRotation = beforeRunRotation;
					rotationPoint = bodyCenter;
					FixingRotation = true;
				}
			}
			animatorHandler.PullToRope = false;
			animatorHandler.PullRope = false;
			lineRenderer.enabled = false;
			_rigidbody.useGravity = true;
			pullingBoneTransform = null;
			pullingBoneRigidbody = null;
			fakePullTimer.Stop();
		}

		public bool TryFindTargetPosition(out RaycastHit hit)
		{
			bool result = virtualTarget.Hitting && (virtualTarget.transform.position - base.transform.position).magnitude < maxTargetDistance;
			hit = virtualTarget.Hit;
			return result;
		}

		private void Awake()
		{
			_rigidbody = this.GetComponentSafe<Rigidbody>();
			animatorHandler = this.GetComponentSafe<PlayerAnimatorHandler>();
			ragdollHelper = this.GetComponentSafe<RagdollHelper>();
			characterRotator = this.GetComponentSafe<CharacterRotator>();
			lineRenderer = this.GetComponentSafe<LineRenderer>();
			cameraPosition = ServiceLocator.GetGameObject("MainCamera").transform;
			virtualTarget = ServiceLocator.Get<VirtualTarget>();
			health = this.GetComponentSafe<Health>();
		}

		public RopeResult Control(bool ropeButtonPressedDown, bool climbEnabled)
		{
			if (ragdollHelper.Ragdolled && (Running() || FixingRotation))
			{
				Stop();
				FixingRotation = false;
			}
			if (FixingRotation && !rotationDelayTimer.InProgress())
			{
				Vector3 position = rotationPoint.position;
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
				base.transform.position = base.transform.position + position - rotationPoint.position;
				if (Quaternion.Angle(targetRotation, base.transform.rotation) < 1f)
				{
					base.transform.rotation = targetRotation;
					FixingRotation = false;
				}
			}
			if (!Running())
			{
				return default(RopeResult);
			}
			fixRotationAroundHead = false;
			if (ropeButtonPressedDown && (animatorHandler.PullToRopeState.Running || animatorHandler.PullRopeState.Running))
			{
				Stop();
				return default(RopeResult);
			}
			if (animatorHandler.PullToRopeState.Running)
			{
				if (Vector3.Distance(headPosition.position, hit.point) < 0.5f)
				{
					bool num = climbEnabled && hit.transform != null && hit.transform.gameObject.layer == 13 && hit.normal.y < 0.3f;
					if (num)
					{
						fixRotationAroundHead = true;
					}
					else
					{
						animatorHandler.Roll = true;
					}
					Stop();
					return new RopeResult(num, hit);
				}
			}
			else if (animatorHandler.PullRopeState.Running)
			{
				if (pullingBoneTransform == null || fakePullTimer.Done() || Vector3.Distance(ropeOuterPosition.position, pullingBoneTransform.position) < pullRopeStopDistance)
				{
					Stop();
					return default(RopeResult);
				}
				lineRenderer.SetPosition(1, pullingBoneTransform.position);
			}
			lineRenderer.SetPosition(0, ropeOuterPosition.position);
			return default(RopeResult);
		}

		private void FixedUpdate()
		{
			if (!Running())
			{
				return;
			}
			if (health.Dead())
			{
				Stop();
			}
			if (animatorHandler.PullToRopeState.Running)
			{
				distance = Vector3.Distance(headPosition.position, hit.point);
				if (distance < 7f && distance > 0.1f && t == 0f)
				{
					pullToRopeSpeed = 8f;
				}
				else
				{
					t += 7f / distance * Time.deltaTime;
					pullToRopeSpeed = Mathf.Lerp(50f, 8f, t);
					if (pullToRopeSpeed > 35f)
					{
						particleDir = hit.point;
					}
				}
				base.transform.position = Vector3.Lerp(base.transform.position, hit.point - (headPosition.position - base.transform.position), pullToRopeSpeed * Time.fixedDeltaTime / distance);
			}
			else if (animatorHandler.PullRopeState.Running && !fakePullTimer.Running() && pullingBoneTransform != null)
			{
				pullingBoneRigidbody.velocity = (ropeOuterPosition.position - pullingBoneTransform.position).normalized * pullRopeSpeed;
			}
		}

		private RagdollHelper HandleStreetVehiclePullStart(WhoIsResult whoIs)
		{
			RagdollHelper result = null;
			VehicleComponents componentSafe = whoIs.gameObject.GetComponentSafe<VehicleComponents>();
			StreetVehicleModesHelper componentSafe2 = whoIs.gameObject.GetComponentSafe<StreetVehicleModesHelper>();
			if (componentSafe.driver == null)
			{
				pullingBoneTransform = whoIs.transform;
				componentSafe2.SetFreeState();
				whoIs.gameObject.GetComponentSafe<Rigidbody>().angularVelocity = new Vector3(UnityEngine.Random.Range(0, 3000), UnityEngine.Random.Range(0, 3000), UnityEngine.Random.Range(0, 3000));
			}
			else
			{
				result = componentSafe.driver.GetComponentSafe<RagdollHelper>();
				componentSafe.KickOffCurrentDriver(relocateCharacter: false);
				componentSafe2.SetFreeState();
			}
			return result;
		}

		private RagdollHelper HandleVehiclePullStart(WhoIsResult whoIs)
		{
			pullingBoneTransform = whoIs.transform;
			VehicleComponents component = whoIs.gameObject.GetComponent<VehicleComponents>();
			if (component != null)
			{
				if (component.driver != null)
				{
					RagdollHelper componentSafe = component.driver.GetComponentSafe<RagdollHelper>();
					component.KickOffCurrentDriver(component.type == VehicleType.Car);
					if (component.type == VehicleType.Bike)
					{
						return componentSafe;
					}
				}
				whoIs.gameObject.GetComponentSafe<CarStopper>().Paralyze();
			}
			float num = Mathf.Clamp(Vector3.Distance(whoIs.transform.position, ropeOuterPosition.position) / 30f, 0.2f, 0.8f);
			Vector3 a = Vector3.Lerp(Vector3.up, (base.transform.position - whoIs.transform.position).normalized, num);
			Rigidbody componentSafe2 = whoIs.gameObject.GetComponentSafe<Rigidbody>();
			componentSafe2.velocity = a * 13f;
			if (component != null && component.type == VehicleType.Bike)
			{
				float num2 = ((float)UnityEngine.Random.Range(0, 2) - 0.5f) * 2f;
				componentSafe2.angularVelocity = new Vector3(0f, (float)UnityEngine.Random.Range(300, 1000) * num2, 0f);
			}
			else
			{
				componentSafe2.angularVelocity = new Vector3(UnityEngine.Random.Range(300, 900), UnityEngine.Random.Range(300, 900), UnityEngine.Random.Range(300, 900));
			}
			fakePullTimer.Run(0.7f);
			return null;
		}
	}
}
