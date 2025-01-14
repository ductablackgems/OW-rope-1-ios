using App.GUI;
using App.Util;
using App.Vehicles;
using App.Vehicles.Car.Navigation;
using UnityEngine;

namespace App.Player
{
	public class BikeDriver : AbstractVehicleDriver
	{
		public void OnGetOffBike()
		{
		}

		protected override AnimatorState GetSittingState()
		{
			return animatorHandler.SittingOnBikeState;
		}

		protected override AnimatorState GetThrowOutDriverState()
		{
			return animatorHandler.ThrowOutBikeDriverState;
		}

		protected override bool GetUseVehicleParameter()
		{
			return animatorHandler.UseBike;
		}

		protected override VehicleType GetVehicleType()
		{
			return VehicleType.Bike;
		}

		protected override void SetUseVehicleParameter(bool useVehicle)
		{
			animatorHandler.UseBike = useVehicle;
		}

		private void Update()
		{
			if (!running)
			{
				return;
			}
			animatorHandler.SItOnBicycleBlend = vehicleComponents.sittingBlend;
			if (moveTowardVehicleHandle)
			{
				base.transform.position = Vector3.MoveTowards(base.transform.position, vehicleComponents.handleTrigger.position, Time.deltaTime * 3f);
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, vehicleComponents.handleTrigger.rotation, Time.deltaTime * 250f);
				if (base.transform.position == vehicleComponents.handleTrigger.position && base.transform.rotation == vehicleComponents.handleTrigger.rotation)
				{
					moveTowardVehicleHandle = false;
					animatorHandler.UseBike = true;
				}
			}
			else if (animatorHandler.ThrowOutBikeDriverState.Running)
			{
				if (onlyThrowOutDriver)
				{
					Stop();
				}
			}
			else if (animatorHandler.GetOnBikeState.Running)
			{
				if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.4f)
				{
					base.transform.position = Vector3.MoveTowards(base.transform.position, vehicleComponents.sitPoint.position, Time.deltaTime * 3f);
					base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, vehicleComponents.sitPoint.rotation, Time.deltaTime * 250f);
				}
			}
			else if (animatorHandler.SittingOnBikeState.Running)
			{
				if (vehicleComponents.vehicleModesHandler.mode != VehicleMode.Player && vehicleComponents.vehicleModesHandler.mode != VehicleMode.AI)
				{
					vehicleComponents.vehicleModesHandler.SetMode(isPlayer ? VehicleMode.Player : VehicleMode.AI);
					vehicleComponents.driver = base.transform;
					if (isPlayer)
					{
						cameraManager.SetVehicleCamera(vehicleComponents);
						panelsManager.ShowPanel(PanelType.Vehicle);
					}
					if (vehicleComponents.name != "stolen")
					{
						vehicleComponents.name = "stolen";
					}
				}
				vehicleComponents.doorReservator.ReleaseReservation(base.transform);
			}
			else if (animatorHandler.GetOffBikeState.Running)
			{
				if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.4f)
				{
					base.transform.position = Vector3.MoveTowards(base.transform.position, vehicleComponents.handleTrigger.position, Time.deltaTime * 3f);
					base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, vehicleComponents.handleTrigger.rotation, Time.deltaTime * 250f);
				}
				vehicleComponents.vehicleModesHandler.SetMode(VehicleMode.Empty);
				if (isPlayer)
				{
					cameraManager.SetPlayerCamera();
					panelsManager.ShowPanel(PanelType.Game);
				}
			}
			else if (!animatorHandler.UseBike)
			{
				SetVehicled(vehicled: false);
			}
		}

		protected override void SetVehicled(bool vehicled, VehicleComponents vehicleComponents = null, bool fixRotation = true)
		{
			if (vehicled)
			{
				animatorHandler.SItOnBicycleBlend = vehicleComponents.sittingBlend;
			}
			base.SetVehicled(vehicled, vehicleComponents, fixRotation);
		}

		private void OnAnimatorIK(int layerIndex)
		{
			if (running && animatorHandler.SittingOnBikeState.Running)
			{
				animatorHandler.Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
				animatorHandler.Animator.SetIKPosition(AvatarIKGoal.LeftHand, vehicleComponents.leftHandle.position);
				animatorHandler.Animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
				animatorHandler.Animator.SetIKRotation(AvatarIKGoal.LeftHand, vehicleComponents.leftHandle.rotation);
				animatorHandler.Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
				animatorHandler.Animator.SetIKPosition(AvatarIKGoal.RightHand, vehicleComponents.rightHandle.position);
				animatorHandler.Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
				animatorHandler.Animator.SetIKRotation(AvatarIKGoal.RightHand, vehicleComponents.rightHandle.rotation);
				animatorHandler.Animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
				animatorHandler.Animator.SetIKPosition(AvatarIKGoal.LeftFoot, vehicleComponents.leftPedal.position);
				animatorHandler.Animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
				animatorHandler.Animator.SetIKPosition(AvatarIKGoal.RightFoot, vehicleComponents.rightPedal.position);
			}
		}
	}
}
