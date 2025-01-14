using App.Util;
using UnityEngine;

namespace App.Enemies.Vehicles.Helicopter
{
	[RequireComponent(typeof(HelicopterAIConfig))]
	public class HelicopterAI : MonoBehaviour
	{
		public WeaponController midlleWeaponController;

		public WeaponController leftWeaponController;

		public WeaponController rightWeaponController;

		private HelicopterAIConfig config;

		private EnemyGunRaycast gunRaycast;

		private Transform playerTransform;

		private Health playerHealth;

		private DurationTimer delayAfterShootingTimer = new DurationTimer();

		private DurationTimer delayAfterRocketTimer = new DurationTimer();

		private float shootDuration;

		protected void Awake()
		{
			config = this.GetComponentSafe<HelicopterAIConfig>();
			gunRaycast = this.GetComponentInChildrenSafe<EnemyGunRaycast>();
		}

		protected void Start()
		{
			playerTransform = ServiceLocator.GetGameObject("Player").transform;
			playerHealth = playerTransform.GetComponentInChildrenSafe<Health>();
		}

		protected void Update()
		{
			if (playerHealth.GetCurrentHealth() == 0f)
			{
				return;
			}
			bool flag = gunRaycast.HitPlayer(config.maxAimDistance);
			if (delayAfterShootingTimer.Running())
			{
				if (delayAfterShootingTimer.Done())
				{
					delayAfterShootingTimer.Stop();
				}
			}
			else if (flag)
			{
				shootDuration += Time.deltaTime;
				midlleWeaponController.transform.LookAt(playerTransform.position + Vector3.up);
				midlleWeaponController.LaunchWeapon();
				if (shootDuration > config.maxShootingDuration)
				{
					delayAfterShootingTimer.Run(config.delayAfterShooting);
					shootDuration = 0f;
				}
			}
			else
			{
				shootDuration -= Time.deltaTime;
				if (shootDuration < 0f)
				{
					shootDuration = 0f;
				}
			}
			if (flag && MathHelper.HitProbabilityPerMinute(config.rocketsPerMinute, Time.deltaTime))
			{
				leftWeaponController.transform.LookAt(playerTransform.position + Vector3.up);
				leftWeaponController.LaunchWeapon();
				rightWeaponController.transform.LookAt(playerTransform.position + Vector3.up);
				rightWeaponController.LaunchWeapon();
			}
		}
	}
}
