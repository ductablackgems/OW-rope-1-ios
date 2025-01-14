using App.GUI;
using App.Util;
using App.Vehicles;
using App.Vehicles.Car.Navigation;
using UnityEngine;

namespace App.Player
{
	public class CarDriver : AbstractVehicleDriver
	{
		private AnimatorEventBugFix eventBugFix = new AnimatorEventBugFix(3);

		public bool StayInCar
		{
			get;
			private set;
		}

		protected override void ClearBeforeRun()
		{
			base.ClearBeforeRun();
			eventBugFix.Clear();
		}

		public void OnOpenDoor()
		{
			if (eventBugFix.SetFiredAndTest(2))
			{
				vehicleComponents.carSounds.openDoor.Play();
			}
		}

		public void OnCloseDoor()
		{
			int firedAndTest = animatorHandler.CloseCarDoorState.Running ? 1 : 0;
			if (eventBugFix.SetFiredAndTest(firedAndTest))
			{
				vehicleComponents.carSounds.closeDoor.Play();
				if (animatorHandler.CloseCarDoorState.Running)
				{
					(isPassenger ? vehicleComponents.passengerDoor : vehicleComponents.door).localRotation = Quaternion.identity;
				}
			}
		}

		public void RunPassenger(bool stayInCar)
		{
			Run(onlyThrowOutDriver: false, useTargetTrigger: true, isPassenger: true);
			StayInCar = stayInCar;
		}

		public override void Stop()
		{
			base.Stop();
			StayInCar = false;
		}

		protected override AnimatorState GetSittingState()
		{
			return animatorHandler.SittingInCarState;
		}

		protected override AnimatorState GetThrowOutDriverState()
		{
			return animatorHandler.ThrowOutDriverState;
		}

		protected override bool GetUseVehicleParameter()
		{
			return animatorHandler.UseCar;
		}

		protected override VehicleType GetVehicleType()
		{
			return VehicleType.Car;
		}

		protected override void SetUseVehicleParameter(bool useVehicle)
		{
			animatorHandler.UseCar = useVehicle;
			SetDefaultWeaponVisibility();
		}

		protected override void OnResetStates()
		{
			base.OnResetStates();
			StayInCar = false;
		}

		private void SetDefaultWeaponVisibility()
		{
			if ((bool)shotController)
			{
				if (animatorHandler.UseCar)
				{
					shotController.HideDefaultWeapon();
				}
				else
				{
					shotController.ShowDefaultWeapon();
				}
			}
		}

		private void Update()
		{
			if (!running)
			{
				return;
			}
			if (moveTowardVehicleHandle)
			{
				Transform transform = isPassenger ? vehicleComponents.passengerHandleTrigger : vehicleComponents.handleTrigger;
				base.transform.position = Vector3.MoveTowards(base.transform.position, transform.position, Time.deltaTime * 3f);
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, transform.rotation, Time.deltaTime * 250f);
				if (base.transform.position == transform.position && base.transform.rotation == transform.rotation)
				{
					moveTowardVehicleHandle = false;
					animatorHandler.UseCar = true;
					SetDefaultWeaponVisibility();
				}
			}
			else if (animatorHandler.OpenCarDoorState.Running)
			{
				if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.6f)
				{
					if (isPassenger)
					{
						vehicleComponents.passengerDoor.localRotation = Quaternion.RotateTowards(vehicleComponents.passengerDoor.localRotation, Quaternion.Euler(0f, -45f, 0f), Time.deltaTime * 300f);
					}
					else
					{
						vehicleComponents.door.localRotation = Quaternion.RotateTowards(vehicleComponents.door.localRotation, Quaternion.Euler(0f, 45f, 0f), Time.deltaTime * 300f);
					}
				}
			}
			else if (animatorHandler.ThrowOutDriverState.Running)
			{
				if (onlyThrowOutDriver)
				{
					Stop();
				}
			}
			else if (animatorHandler.GetInCarState.Running)
			{
				if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.3f)
				{
					Transform transform2 = isPassenger ? vehicleComponents.passengerSitPoint : vehicleComponents.sitPoint;
					base.transform.position = Vector3.MoveTowards(base.transform.position, transform2.position, Time.deltaTime * 3f);
					base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, transform2.rotation, Time.deltaTime * 250f);
				}
				else
				{
					base.transform.localRotation = Quaternion.RotateTowards(base.transform.localRotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * 500f);
				}
			}
			else if (animatorHandler.CloseCarDoorFromInsideState.Running)
			{
				if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f)
				{
					Transform obj = isPassenger ? vehicleComponents.passengerDoor : vehicleComponents.door;
					obj.localRotation = Quaternion.RotateTowards(obj.localRotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * 250f);
				}
			}
			else if (animatorHandler.SittingInCarState.Running)
			{
				if (isPassenger)
				{
					vehicleComponents.passenger = base.transform;
					vehicleComponents.passengerDoorReservator.ReleaseReservation(base.transform);
				}
				else
				{
					if (vehicleComponents.vehicleModesHandler.mode == VehicleMode.Player || vehicleComponents.vehicleModesHandler.mode == VehicleMode.AI)
					{
						return;
					}
					vehicleComponents.driver = base.transform;
					vehicleComponents.vehicleModesHandler.SetMode(isPlayer ? VehicleMode.Player : VehicleMode.AI);
					if (isPlayer)
					{
						cameraManager.SetVehicleCamera(vehicleComponents);
						panelsManager.ShowPanel(PanelType.Vehicle);
						if (vehicleComponents.playerGarbageTruckControl != null)
						{
							GarbageCapacity.Instance.Activate();
						}
					}
					if (vehicleComponents.name != "stolen")
					{
						vehicleComponents.name = "stolen";
					}
					vehicleComponents.doorReservator.ReleaseReservation(base.transform);
				}
			}
			else if (animatorHandler.GetOutOffCarState.Running)
			{
				if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.3f)
				{
					Transform transform3 = isPassenger ? vehicleComponents.passengerHandleTrigger : vehicleComponents.handleTrigger;
					base.transform.position = Vector3.MoveTowards(base.transform.position, transform3.position, Time.deltaTime * 3f);
					base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, transform3.rotation, Time.deltaTime * 250f);
				}
				else if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.1f)
				{
					if (isPassenger)
					{
						vehicleComponents.passengerDoor.localRotation = Quaternion.RotateTowards(vehicleComponents.passengerDoor.localRotation, Quaternion.Euler(0f, -45f, 0f), Time.deltaTime * 250f);
					}
					else
					{
						vehicleComponents.door.localRotation = Quaternion.RotateTowards(vehicleComponents.door.localRotation, Quaternion.Euler(0f, 45f, 0f), Time.deltaTime * 250f);
					}
				}
			}
			else if (animatorHandler.CloseCarDoorState.Running)
			{
				if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.2f)
				{
					Transform obj2 = isPassenger ? vehicleComponents.passengerDoor : vehicleComponents.door;
					obj2.localRotation = Quaternion.RotateTowards(obj2.localRotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * 250f);
				}
				if (isPlayer)
				{
					cameraManager.SetPlayerCamera();
					panelsManager.ShowPanel(PanelType.Game);
				}
			}
			else if (!animatorHandler.UseCar)
			{
				SetVehicled(vehicled: false);
			}
		}
	}
}
