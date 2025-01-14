using App.Camera;
using App.Spawn;
using App.Util;
using App.Weapons;
using com.ootii.Cameras;
using System.Collections.Generic;
using UnityEngine;

namespace App.Player
{
	[DefaultExecutionOrder(9999)]
	public class AimingController : MonoBehaviour, IResetable
	{
		private int lookDirectionAdjustFrameCount;

		private Vector3 aimTargetPosition;

		private int forwardHash;

		private IWeapon weapon;

		private IWeapon lastWeapon;

		private FpsWeapon fpsWeapon;

		private FpsWeaponManager fpsWeaponManager;

		private Transform crosshairTransform;

		private readonly List<Renderer> renderers = new List<Renderer>();

		private Transform cameraParentTransform;

		private UnityEngine.Camera mainCamera;

		private GunType lastGunType;

		private bool aimingRequested;

		private bool wasAiming;

		private ShotController shotController;

		private Transform virtualTarget;

		private AnimatorState gunReloadState;

		private Animator animator;

		private Health heath;

		private OotiiCamera ootiiCamera;

		private RagdollHelper ragdollHelper;

		private float defaultFieldOfView;

		public bool CanAim
		{
			get;
			private set;
		}

		public bool IsAiming
		{
			get;
			private set;
		}

		private void Awake()
		{
			forwardHash = Animator.StringToHash("Forward");
			ootiiCamera = ServiceLocator.Get<OotiiCamera>();
			fpsWeaponManager = ServiceLocator.Get<FpsWeaponManager>(showError: false);
			GameObject gameObject = ServiceLocator.GetGameObject("CrossHair", showError: false);
			if (gameObject != null)
			{
				crosshairTransform = gameObject.transform;
			}
			else
			{
				base.enabled = false;
			}
			cameraParentTransform = UnityEngine.Object.FindObjectOfType<CameraController>().transform;
			heath = GetComponent<Health>();
			animator = GetComponent<Animator>();
			shotController = GetComponent<ShotController>();
			virtualTarget = ServiceLocator.GetGameObject("VirtualTarget").transform;
			gunReloadState = new AnimatorState("TorsoGunLayer.GunReload", animator, 1);
			ragdollHelper = GetComponent<RagdollHelper>();
			mainCamera = UnityEngine.Camera.main;
			defaultFieldOfView = mainCamera.fieldOfView;
		}

		void IResetable.ResetStates()
		{
			StopAiming();
		}

		private static void SetMuzzleFlashActive(IWeapon weapon, bool active)
		{
			if (weapon == null)
			{
				return;
			}
			WeaponLauncher component = weapon.GetGameObject().GetComponent<WeaponLauncher>();
			if (!(component == null))
			{
				Transform[] missileOuter = component.MissileOuter;
				for (int i = 0; i < missileOuter.Length; i++)
				{
					missileOuter[i].gameObject.SetActive(active);
				}
			}
		}

		private static void EnableMuzzleFlash(IWeapon weapon)
		{
			SetMuzzleFlashActive(weapon, active: true);
		}

		private static void DisableMuzzleFlash(IWeapon weapon)
		{
			SetMuzzleFlashActive(weapon, active: false);
		}

		private void EnableRenderers()
		{
			foreach (Renderer renderer in renderers)
			{
				renderer.enabled = true;
			}
		}

		private void DisableRenderers()
		{
			renderers.Clear();
			Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>(includeInactive: false);
			foreach (Renderer renderer in componentsInChildren)
			{
				renderer.enabled = false;
				renderers.Add(renderer);
			}
		}

		private void Update()
		{
			if (InputUtils.ScopeAim.IsDown)
			{
				aimingRequested = !aimingRequested;
			}
		}

		private void LateUpdate()
		{
			if (heath.Dead())
			{
				StopAiming();
				CanAim = false;
				return;
			}
			if (!ootiiCamera.enabled)
			{
				StopAiming();
				CanAim = false;
				return;
			}
			float layerWeight = animator.GetLayerWeight(1);
			GunType gunType = shotController.gunType;
			if (gunType != lastGunType)
			{
				weapon = shotController.GetWeapon();
				fpsWeapon = fpsWeaponManager.FindWeapon(gunType);
			}
			CanAim = (fpsWeapon != null && lastGunType == gunType && layerWeight > 0.5f && !gunReloadState.Running && !ragdollHelper.Ragdolled && animator.GetFloat(forwardHash) < 1.5f);
			IsAiming = (CanAim && aimingRequested);
			if (!CanAim)
			{
				aimingRequested = false;
			}
			if (IsAiming && !wasAiming)
			{
				crosshairTransform.gameObject.SetActive(value: false);
				DisableRenderers();
				DisableMuzzleFlash(weapon);
				fpsWeapon.gameObject.SetActive(value: true);
				mainCamera.fieldOfView = fpsWeapon.fieldOfView;
				StartAdjustingLookDirection();
			}
			else if (!IsAiming && wasAiming)
			{
				if (CanAim)
				{
					Vector3 position = virtualTarget.position;
					position.y = base.transform.position.y;
					base.transform.LookAt(position);
				}
				StopAiming();
				StartAdjustingLookDirection();
			}
			else if (IsAiming)
			{
				mainCamera.fieldOfView = fpsWeapon.fieldOfView;
			}
			if (lookDirectionAdjustFrameCount > 0)
			{
				cameraParentTransform.transform.LookAt(aimTargetPosition);
				lookDirectionAdjustFrameCount--;
			}
			wasAiming = IsAiming;
			lastGunType = shotController.gunType;
			lastWeapon = weapon;
		}

		private void StartAdjustingLookDirection()
		{
			Vector3 position = virtualTarget.position;
			cameraParentTransform.LookAt(position);
			float magnitude = Vector3.Project(position - cameraParentTransform.position, cameraParentTransform.forward).magnitude;
			Vector3 b = Vector3.ProjectOnPlane(mainCamera.ScreenToWorldPoint(new Vector3(crosshairTransform.position.x, crosshairTransform.position.y, magnitude)) - position, cameraParentTransform.forward);
			aimTargetPosition = position - b;
			lookDirectionAdjustFrameCount = 5;
		}

		private void StopAiming()
		{
			if (wasAiming)
			{
				if (fpsWeaponManager != null)
				{
					fpsWeaponManager.HideWeapons();
				}
				crosshairTransform.gameObject.SetActive(value: true);
				EnableRenderers();
				EnableMuzzleFlash(lastWeapon);
				IsAiming = false;
				aimingRequested = false;
				wasAiming = false;
				if (mainCamera != null)
				{
					mainCamera.fieldOfView = defaultFieldOfView;
				}
			}
		}

		private void OnDestroy()
		{
			StopAiming();
		}
	}
}
