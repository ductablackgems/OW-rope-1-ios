using App.AI.Scanner;
using App.Player.Definition;
using App.Weapons;
using UnityEngine;

namespace App.Player
{
	[RequireComponent(typeof(PlayerAnimatorHandler))]
	public class ShotController : MonoBehaviour, ICharacterModule
	{
		public FightType defaultFightType;

		public GameObject defaultWeapon;

		public string[] targetTags = new string[1]
		{
			"Enemy"
		};

		private float layerBlendDuration = 0.25f;

		public GunType gunType;

		public bool aimOnVirtualTarget = true;

		[Space(10f)]
		public GunIKMappingsScriptableObject ikMappings;

		public Transform rightHandIkTarget;

		private IWeapon[] weapons;

		private PlayerAnimatorHandler animatorHandler;

		private Transform virtualTarget;

		private DurationTimer layerBlendTimer = new DurationTimer();

		private GunIKMapping gunIKMapping;

		private bool isPlayer;

		private bool running;

		public GunType[] GunTypes
		{
			get;
			private set;
		}

		public bool Debugging
		{
			get;
			set;
		}

		public void Run()
		{
			running = true;
			layerBlendTimer.Run(layerBlendDuration);
			base.enabled = true;
		}

		public bool Running()
		{
			return running;
		}

		public void Stop()
		{
			running = false;
			IWeapon[] array = weapons;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].GetGameObject().SetActive(value: false);
			}
			ShowDefaultWeapon();
			layerBlendTimer.Run(layerBlendDuration);
		}

		public bool HasGuns()
		{
			return weapons.Length != 0;
		}

		public IWeapon GetWeapon(GunType gunType)
		{
			IWeapon[] array = weapons;
			foreach (IWeapon weapon in array)
			{
				if (weapon.GetGunType() == gunType)
				{
					return weapon;
				}
			}
			return null;
		}

		public IWeapon GetWeapon()
		{
			IWeapon[] array = weapons;
			foreach (IWeapon weapon in array)
			{
				if (weapon.GetGunType() == gunType)
				{
					return weapon;
				}
			}
			return null;
		}

		public IWeapon[] GetAllWeapons()
		{
			return weapons;
		}

		public void Control(bool attackPressed, bool attackDownPressed)
		{
			animatorHandler.Gun = gunType;
			animatorHandler.LoopGunFire = false;
			if (running)
			{
				IWeapon[] array = weapons;
				foreach (IWeapon weapon in array)
				{
					weapon.GetGameObject().SetActive(weapon.GetGunType() == gunType);
				}
				SetDefaultWeapon();
			}
			if (ikMappings != null && (gunIKMapping.gunType != gunType || Debugging) && ikMappings.TryGetMapping(gunType, out gunIKMapping) && gunIKMapping.useRightHandIK)
			{
				rightHandIkTarget.localPosition = gunIKMapping.rightHandIKLocalPosition;
				rightHandIkTarget.localRotation = gunIKMapping.rightHandIKLocalRotation;
			}
			IWeapon weapon2 = GetWeapon();
			if (attackPressed && weapon2 != null && (weapon2.IsAutomat() | attackDownPressed))
			{
				if (aimOnVirtualTarget)
				{
					Vector3 worldPosition = new Vector3(virtualTarget.position.x, base.transform.position.y, virtualTarget.position.z);
					base.transform.LookAt(worldPosition);
					weapon2.FocusMissileOuterTo(virtualTarget.position);
				}
				weapon2.Shoot();
			}
			if (weapon2 != null)
			{
				animatorHandler.LoopGunFire = weapon2.Shooting();
			}
		}

		public void OnShot()
		{
		}

		private void Awake()
		{
			animatorHandler = this.GetComponentSafe<PlayerAnimatorHandler>();
			weapons = GetComponentsInChildren<IWeapon>();
			virtualTarget = ServiceLocator.GetGameObject("VirtualTarget").transform;
			layerBlendTimer.FakeDone(layerBlendDuration);
			isPlayer = CompareTag("Player");
			GunTypes = new GunType[weapons.Length];
			for (int i = 0; i < weapons.Length; i++)
			{
				IWeapon weapon = weapons[i];
				GunTypes[i] = weapon.GetGunType();
				weapon.RegisterOnShot(_OnShot);
				weapon.RegisterOnReload(OnReload);
				weapon.SetOwner(base.gameObject);
			}
		}

		private void Update()
		{
			animatorHandler.TorsoGunLayerWeight = (running ? layerBlendTimer.GetProgress() : (1f - layerBlendTimer.GetProgress()));
			if (!running && !layerBlendTimer.InProgress())
			{
				base.enabled = false;
			}
		}

		private void OnAnimatorIK(int layerIndex)
		{
			if (!(animatorHandler.TorsoGunLayerWeight > 0f))
			{
				return;
			}
			IWeapon weapon = GetWeapon(gunType);
			if (weapon == null || !(weapon.GetLeftHandPosition() != null))
			{
				return;
			}
			Vector3 point = Vector3.zero;
			if ((bool)ikMappings && gunIKMapping.gunType != 0)
			{
				point = ((!gunIKMapping.useLeftHandIKOffset) ? Vector3.zero : gunIKMapping.leftHandOffset);
				if (gunIKMapping.useRightHandIK)
				{
					animatorHandler.Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, animatorHandler.TorsoGunLayerWeight);
					animatorHandler.Animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandIkTarget.position);
					animatorHandler.Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, animatorHandler.TorsoGunLayerWeight);
					animatorHandler.Animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandIkTarget.rotation);
				}
			}
			animatorHandler.Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, animatorHandler.TorsoGunLayerWeight);
            animatorHandler.Animator.SetIKPosition(AvatarIKGoal.LeftHand, weapon.GetLeftHandPosition().position + weapon.GetLeftHandPosition().rotation * point);
        }

		private void OnDestroy()
		{
			IWeapon[] array = weapons;
			foreach (IWeapon obj in array)
			{
				obj.UnregisterOnShot(_OnShot);
				obj.UnregisterOnReload(OnReload);
			}
		}

		private void Start()
		{
			IWeapon[] array = weapons;
			foreach (IWeapon weapon in array)
			{
				weapon.GetGameObject().SetActive(value: false);
				if (weapon.GetType() == typeof(WeaponLauncher))
				{
					((WeaponLauncher)weapon).TargetTag = targetTags;
				}
			}
		}

		private void _OnShot()
		{
			animatorHandler.TriggerShot();
			if (isPlayer)
			{
				AIScanner.LastNoiseTime = Time.time;
			}
		}

		private void OnReload()
		{
			animatorHandler.TriggerReload();
		}

		private void SetDefaultWeapon()
		{
			if (gunType != 0 && defaultFightType != 0 && defaultWeapon.activeSelf)
			{
				defaultWeapon.SetActive(value: false);
			}
		}

		public void HideDefaultWeapon()
		{
			if (gunType == GunType.Unknown && isPlayer && defaultFightType != 0 && defaultWeapon.activeSelf)
			{
				defaultWeapon.SetActive(value: false);
			}
		}

		public void ShowDefaultWeapon()
		{
			if ((gunType == GunType.Unknown || WeaponNonActive()) && isPlayer && defaultFightType != 0 && !defaultWeapon.activeSelf)
			{
				defaultWeapon.SetActive(value: true);
			}
		}

		private bool WeaponNonActive()
		{
			IWeapon[] array = weapons;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].GetGameObject().activeSelf)
				{
					return false;
				}
			}
			return true;
		}
	}
}
