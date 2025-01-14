using App.Player.FightSystem;
using App.Player.GrenadeThrow;
using UnityEngine;

namespace App.Player
{
	public class CharacterModesHandler : MonoBehaviour
	{
		private PlayerController playerController;

		private BikeDriver bikeDriver;

		private CarDriver carDriver;

		private BicycleDriver bicycleDriver;

		private GyroboardDriver gyroboardDriver;

		private SkateboardDriver skateboardDriver;

		private HelicopterDriver helicopterDriver;

		private TankDriver tankDriver;

		private MechDriver mechDriver;

		private AirplaneDriver airplaneDriver;

		private AdvancedFightController advancedFightController;

		private ShotController shotController;

		private GrenadeThrowController grenadeThrowController;

		private GlideController glideController;

		private ClimbController climbController;

		private RopeController ropeController;

		private FlyController flyController;

		public CharacterMode GetRunningMode()
		{
			if (bikeDriver.Running())
			{
				return CharacterMode.Bike;
			}
			if (carDriver.Running())
			{
				return CharacterMode.Car;
			}
			if (bicycleDriver.Running())
			{
				return CharacterMode.Bicycle;
			}
			if (gyroboardDriver.Running())
			{
				return CharacterMode.Gyroboard;
			}
			if (skateboardDriver.Running())
			{
				return CharacterMode.Skateboard;
			}
			if (helicopterDriver != null && helicopterDriver.Running())
			{
				return CharacterMode.Helicopter;
			}
			if (tankDriver != null && tankDriver.Running())
			{
				return CharacterMode.Tank;
			}
			if (mechDriver != null && mechDriver.Running())
			{
				return CharacterMode.Mech;
			}
			if (airplaneDriver != null && airplaneDriver.Running())
			{
				return CharacterMode.Airplane;
			}
			if (advancedFightController != null && advancedFightController.Running())
			{
				return CharacterMode.AdvancedFight;
			}
			if (shotController.Running())
			{
				return CharacterMode.Gun;
			}
			if (grenadeThrowController != null && grenadeThrowController.Running())
			{
				return CharacterMode.Grenade;
			}
			if (glideController != null && glideController.Running())
			{
				return CharacterMode.Glide;
			}
			if (climbController != null && climbController.Running())
			{
				return CharacterMode.Climb;
			}
			if (ropeController != null && ropeController.Running())
			{
				return CharacterMode.Rope;
			}
			if (flyController != null && flyController.Running())
			{
				return CharacterMode.Fly;
			}
			if (!(playerController == null) && !playerController.controlled)
			{
				return CharacterMode.Uncontrolled;
			}
			return CharacterMode.Default;
		}

		public void TryStopAll()
		{
			if (bikeDriver.Running())
			{
				bikeDriver.Stop();
			}
			if (carDriver.Running())
			{
				carDriver.Stop();
			}
			if (bicycleDriver.Running())
			{
				bicycleDriver.Stop();
			}
			if (gyroboardDriver.Running())
			{
				gyroboardDriver.Stop();
			}
			if (skateboardDriver.Running())
			{
				skateboardDriver.Stop();
			}
			if (helicopterDriver != null && helicopterDriver.Running())
			{
				helicopterDriver.Stop();
			}
			if (advancedFightController != null && advancedFightController.Running())
			{
				advancedFightController.Stop();
			}
			if (shotController.Running())
			{
				shotController.Stop();
			}
			if (grenadeThrowController != null && grenadeThrowController.Running())
			{
				grenadeThrowController.Stop();
			}
			if (climbController != null && climbController.Running())
			{
				climbController.Stop();
			}
			if (ropeController != null && ropeController.Running())
			{
				ropeController.Stop();
			}
			if (flyController != null && flyController.Running())
			{
				flyController.Stop();
			}
		}

		protected void Awake()
		{
			playerController = GetComponent<PlayerController>();
			bikeDriver = this.GetComponentSafe<BikeDriver>();
			carDriver = this.GetComponentSafe<CarDriver>();
			bicycleDriver = this.GetComponentSafe<BicycleDriver>();
			gyroboardDriver = this.GetComponentSafe<GyroboardDriver>();
			skateboardDriver = this.GetComponentSafe<SkateboardDriver>();
			helicopterDriver = GetComponent<HelicopterDriver>();
			tankDriver = GetComponent<TankDriver>();
			mechDriver = GetComponent<MechDriver>();
			airplaneDriver = GetComponent<AirplaneDriver>();
			advancedFightController = GetComponent<AdvancedFightController>();
			shotController = this.GetComponentSafe<ShotController>();
			grenadeThrowController = GetComponent<GrenadeThrowController>();
			glideController = GetComponent<GlideController>();
			climbController = GetComponent<ClimbController>();
			ropeController = GetComponent<RopeController>();
			flyController = GetComponent<FlyController>();
		}
	}
}
