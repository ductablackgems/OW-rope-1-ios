using App.Player;
using App.Player.FightSystem;
using App.Vehicles;
using App.Vehicles.Tank;
using com.ootii.Cameras;
using UnityEngine;

namespace App.Camera
{
	public class OotiiCamera : MonoBehaviour
	{
		public CameraController cameraController;

		public float followMotorDelay = 1.5f;

		public float vehicleMinPitch = -7f;

		public float vehicleMaxPitch = 30f;

		private CameraMotor aimingMotor;

		private YawPitchMotor mainMotor;

		private OrbitFollowMotor followMotor;

		private CameraMotor kinematicMotor;

		private TransitionMotor targetInMotor;

		private TransitionMotor targetOutMotor;

		private UnityEngine.Camera mainCamera;

		private CameraAnchor anchor;

		private PlayerModel player;

		private AnimationSimulator animationSimulator;

		private AimingController playerAimingController;

		private Transform aimingAnchorTransform;

		private OotiiCameraMode mode;

		private bool firstFrame = true;

		private bool wasAiming;

		private bool touched;

		private VehicleComponents vehicle;

		private WreckHelper wreck;

		private DurationTimer followMotorTimer = new DurationTimer();

		private DurationTimer clearYawPitchTimer = new DurationTimer();

		private float minCameraDistance;

		private float maxCameraDistance;

		private float minPitch;

		private float maxPitch;

		public void SetVehicle(VehicleComponents vehicle)
		{
			mode = OotiiCameraMode.Vehicle;
			this.vehicle = vehicle;
			float distance = vehicle.cameraViewSetting.distance;
			cameraController.MinCollisionDistance = distance;
			mainMotor.Distance = distance;
			mainMotor.MaxDistance = distance;
			followMotor.Distance = distance;
			followMotor.MaxDistance = distance;
			mainMotor.MinPitch = vehicleMinPitch;
			mainMotor.MaxPitch = vehicleMaxPitch;
			anchor.Set(vehicle.transform, new Vector3(0f, vehicle.cameraViewSetting.height));
			followMotor.SetTargetYawPitch(0f, 5f, 300f);
			clearYawPitchTimer.Run(1.5f);
		}

		public void ClearVehicle()
		{
			if (mode != 0)
			{
				mode = OotiiCameraMode.Player;
				vehicle = null;
				wreck = null;
				cameraController.MinCollisionDistance = minCameraDistance;
				mainMotor.MaxDistance = maxCameraDistance;
				if (mainMotor.Distance < minCameraDistance)
				{
					mainMotor.Distance = minCameraDistance;
				}
				else if (mainMotor.Distance > maxCameraDistance)
				{
					mainMotor.Distance = maxCameraDistance;
				}
				mainMotor.MinPitch = minPitch;
				mainMotor.MaxPitch = maxPitch;
			}
		}

		public void SetWreck(WreckHelper wreck)
		{
			mode = OotiiCameraMode.Wreck;
			this.wreck = wreck;
			anchor.Set(wreck.transform, anchor.Offset);
		}

		public bool TargetingWreck()
		{
			return mode == OotiiCameraMode.Wreck;
		}

		private void Awake()
		{
			mainCamera = UnityEngine.Camera.main;
			player = ServiceLocator.GetPlayerModel();
			playerAimingController = player.Transform.GetComponent<AimingController>();
			aimingAnchorTransform = new GameObject().transform;
			aimingAnchorTransform.SetParent(player.Transform);
			aimingAnchorTransform.localPosition = Vector3.zero;
			aimingAnchorTransform.localRotation = Quaternion.identity;
			animationSimulator = ServiceLocator.Get<AnimationSimulator>(showError: false);
			GameObject gameObject = new GameObject("CameraAnchor");
			anchor = gameObject.AddComponent<CameraAnchor>();
			if (cameraController != null)
			{
				cameraController._Camera = mainCamera;
				cameraController.enabled = true;
				aimingMotor = cameraController.GetMotor<CameraMotor>("1st Person View");
				mainMotor = cameraController.GetMotor<YawPitchMotor>("3rd Person Fixed");
				followMotor = cameraController.GetMotor<OrbitFollowMotor>("3rd Person Follow");
				kinematicMotor = cameraController.GetMotor<CameraMotor>("3rd Person Fixed Kinematic");
				targetInMotor = cameraController.GetMotor<TransitionMotor>("Targeting In");
				targetOutMotor = cameraController.GetMotor<TransitionMotor>("Targeting Out");
				cameraController.enabled = false;
				minCameraDistance = cameraController.MinCollisionDistance;
				maxCameraDistance = mainMotor.MaxDistance;
				minPitch = mainMotor.MinPitch;
				maxPitch = mainMotor.MaxPitch;
				followMotor.MinPitch = vehicleMinPitch;
				followMotor.MaxPitch = vehicleMaxPitch;
			}
		}

		private void Start()
		{
			ETCTouchPad controlTouchPad = ETCInput.GetControlTouchPad("CameraPad");
			if (controlTouchPad != null)
			{
				controlTouchPad.onTouchStart.AddListener(delegate
				{
					touched = true;
					if (base.enabled && vehicle != null)
					{
						followMotorTimer.Stop();
						if (cameraController.ActiveMotor.Equals(followMotor))
						{
							cameraController.ActivateMotor(mainMotor);
						}
					}
				});
				controlTouchPad.onTouchUp.AddListener(delegate
				{
					touched = false;
					followMotorTimer.Run(followMotorDelay);
				});
			}
		}

		private void OnEnable()
		{
			cameraController.transform.position = base.transform.position;
			cameraController.transform.rotation = base.transform.rotation;
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			cameraController.Anchor = anchor.transform;
			UpdateAnchor();
			cameraController.enabled = true;
		}

		private void OnDisable()
		{
			cameraController.enabled = false;
		}

		private void Update()
		{
			UpdateAnchor();
		}

		private void UpdateAnchor()
		{
			Vector3 position = cameraController.transform.position;
			Quaternion rotation = cameraController.transform.rotation;
			bool flag = false;
			if (clearYawPitchTimer.Done())
			{
				clearYawPitchTimer.Stop();
				followMotor.ClearTargetYawPitch();
			}
			if (mode == OotiiCameraMode.Vehicle || mode == OotiiCameraMode.Wreck)
			{
				if (vehicle != null)
				{
					anchor.transform.eulerAngles = new Vector3(0f, vehicle.transform.eulerAngles.y, 0f);
				}
				if (followMotorTimer.Done())
				{
					followMotorTimer.Stop();
					if (cameraController.ActiveMotor.Equals(mainMotor))
					{
						cameraController.ActivateMotor(followMotor);
					}
				}
				if (!touched && !followMotorTimer.Running() && cameraController.ActiveMotor.Equals(mainMotor))
				{
					cameraController.ActivateMotor(followMotor);
				}
			}
			else if (player.RagdollHelper.Ragdolled)
			{
				if (anchor.Target != player.PelvisTransform)
				{
					anchor.Set(player.PelvisTransform, Vector3.zero);
				}
			}
			else if (player.ClimbController == null)
			{
				if (anchor.Target != player.Transform)
				{
					anchor.Set(player.Transform, Vector3.up);
				}
			}
			else if (!firstFrame && player.ClimbController.Running())
			{
				anchor.Set(player.ClimbController.bodyCenter, Vector3.down * 0.2f);
			}
			else if (!firstFrame && player.GlideController.Running())
			{
				anchor.Set(player.Transform, Vector3.up * 0.3f);
			}
			else if (!firstFrame && player.FightController.RunningCustomMovement())
			{
				anchor.Set(animationSimulator.cameraAnchor, Vector3.up);
				flag = true;
			}
			else
			{
				anchor.Set(player.ClimbController.bodyCenter, Vector3.zero);
			}
			bool flag2 = false;
			if (playerAimingController != null && playerAimingController.IsAiming)
			{
				anchor.Set(aimingAnchorTransform, Vector3.up * -0.2f);
				cameraController.ActivateMotor(aimingMotor);
				flag2 = true;
			}
			if (!flag2 && wasAiming)
			{
				anchor.TeleportToTarget();
			}
			wasAiming = flag2;
			if (!firstFrame && mode == OotiiCameraMode.Player)
			{
				if (flag)
				{
					if (cameraController.ActiveMotor.Equals(mainMotor))
					{
						cameraController.ActivateMotor(targetOutMotor);
					}
				}
				else if (cameraController.ActiveMotor.Equals(kinematicMotor))
				{
					cameraController.ActivateMotor(targetInMotor);
				}
				else if (cameraController.ActiveMotor.Equals(followMotor) || (!flag2 && cameraController.ActiveMotor == aimingMotor))
				{
					cameraController.ActivateMotor(mainMotor);
				}
			}
			firstFrame = false;
		}
	}
}
