using App.GUI;
using App.Player.Definition;
using App.SaveSystem;
using App.Spawn;
using App.Weapons;
using System.Collections.Generic;
using UnityEngine;

namespace App.Player
{
	public class StickyRocketController : MonoBehaviour, IResetable
	{
		public StickyRocket stickyRocketPrefab;

		public GameObject previewPrefab;

		private Transform crosshairTransform;

		private GameObject crosshairGameobject;

		private PlayerAnimatorHandler animatorHandler;

		private GameObject preview;

		private UnityEngine.Camera camera;

		private ShotController shotController;

		private PlayerController playerController;

		private GunSaveEntity save;

		private SharedGui sharedGui;

		private bool isStickingAnimationRunning;

		private readonly List<StickyRocket> unlaunchedRockets = new List<StickyRocket>();

		public int AvailableRocketCount
		{
			get;
			private set;
		}

		private void Awake()
		{
			camera = UnityEngine.Camera.main;
			crosshairGameobject = ServiceLocator.GetGameObject("CrossHair", showError: false);
			if (crosshairGameobject != null)
			{
				crosshairTransform = crosshairGameobject.transform;
			}
			else
			{
				base.enabled = false;
			}
			shotController = this.GetComponentSafe<ShotController>();
			animatorHandler = this.GetComponentSafe<PlayerAnimatorHandler>();
			playerController = this.GetComponentSafe<PlayerController>();
			sharedGui = ServiceLocator.Get<SharedGui>();
			PlayerSaveEntity playerSave = ServiceLocator.Get<SaveEntities>().PlayerSave;
			save = playerSave.GetGunSave(GunType.StickyRocket);
			AvailableRocketCount = save.ammo;
			preview = UnityEngine.Object.Instantiate(previewPrefab);
			preview.SetActive(value: false);
		}

		public void ResetStates()
		{
			foreach (StickyRocket unlaunchedRocket in unlaunchedRockets)
			{
				if (!(unlaunchedRocket == null) && !unlaunchedRocket.Launched)
				{
					UnityEngine.Object.Destroy(unlaunchedRocket.gameObject);
				}
			}
			unlaunchedRockets.Clear();
		}

		private void LateUpdate()
		{
			if (isStickingAnimationRunning)
			{
				if (!animatorHandler.GrenadeThrowState.Running)
				{
					isStickingAnimationRunning = false;
					return;
				}
				animatorHandler.GrenadeLayerWeight = 1f;
				base.transform.rotation = Quaternion.Euler(0f, camera.transform.rotation.eulerAngles.y, 0f);
				shotController.Stop();
			}
		}

		private void Update()
		{
			if (animatorHandler.CarMachine.Running)
			{
				preview.SetActive(value: false);
				return;
			}
			Vector3 position = crosshairTransform.position;
			Ray ray = camera.ScreenPointToRay(new Vector3(position.x, position.y, 0f));
			bool controlled = playerController.Controlled;
			bool flag = controlled && unlaunchedRockets.Count > 0;
			if (flag && ETCInput.GetButtonDown("LaunchRocketsButton"))
			{
				foreach (StickyRocket unlaunchedRocket in unlaunchedRockets)
				{
					if (!(unlaunchedRocket == null))
					{
						unlaunchedRocket.Launch();
					}
				}
				unlaunchedRockets.Clear();
			}
			bool num = AvailableRocketCount > 0;
			Vector3 point = Vector3.zero;
			Vector3 direction = Vector3.zero;
			bool flag2 = ((num && crosshairGameobject.activeInHierarchy) & controlled) && StickyRocket.CanStickRocket(ray, 8f, out point, out direction);
			if (flag2)
			{
				preview.SetActive(value: true);
				preview.transform.position = point;
				preview.transform.rotation = Quaternion.LookRotation(direction);
				if (ETCInput.GetButtonDown("StickRocketButton"))
				{
					StickyRocket stickyRocket = StickyRocket.TryStickRocket(ray, 8f, stickyRocketPrefab);
					if (stickyRocket == null)
					{
						return;
					}
					unlaunchedRockets.Add(stickyRocket);
					AvailableRocketCount--;
					save.ammo = AvailableRocketCount;
					save.Save();
					shotController.Stop();
					animatorHandler.GrenadeLayerWeight = 1f;
					isStickingAnimationRunning = true;
					animatorHandler.TriggerThrowGrenade();
				}
			}
			else
			{
				preview.SetActive(value: false);
			}
			if ((bool)sharedGui.stickRocketButton)
			{
				sharedGui.stickRocketButton.SetActive(flag2);
				sharedGui.launchRocketsButton.SetActive(flag);
			}
		}

		public void RefreshRocketCount()
		{
			if (save != null)
			{
				AvailableRocketCount = save.ammo;
			}
		}
	}
}
