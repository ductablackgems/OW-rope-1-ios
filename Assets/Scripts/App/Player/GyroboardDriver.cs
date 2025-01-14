using App.GUI;
using App.Util;
using App.Vehicles;
using App.Vehicles.Bicycle;
using UnityEngine;

namespace App.Player
{
	public class GyroboardDriver : AbstractVehicleDriver
	{
		protected override AnimatorState GetSittingState()
		{
			return animatorHandler.StandingOnGyroboardState;
		}

		protected override AnimatorState GetThrowOutDriverState()
		{
			return animatorHandler.ThrowOutGyroboardDriverState;
		}

		protected override bool GetUseVehicleParameter()
		{
			return animatorHandler.UseGyroboard;
		}

		protected override VehicleType GetVehicleType()
		{
			return VehicleType.Gyroboard;
		}

		protected override void SetUseVehicleParameter(bool useVehicle)
		{
			animatorHandler.UseGyroboard = useVehicle;
		}

		private void Update()
		{
			if (!running)
			{
				return;
			}
			if (moveTowardVehicleHandle)
			{
				vehicleComponents.streetVehicleModesHelper.SetEmptyStanding();
				base.transform.position = Vector3.MoveTowards(base.transform.position, vehicleComponents.handleTrigger.position, Time.deltaTime * 3f);
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, vehicleComponents.handleTrigger.rotation, Time.deltaTime * 250f);
				if (base.transform.position == vehicleComponents.handleTrigger.position && base.transform.rotation == vehicleComponents.handleTrigger.rotation)
				{
					moveTowardVehicleHandle = false;
					animatorHandler.UseGyroboard = true;
				}
			}
			else if (animatorHandler.ThrowOutGyroboardDriverState.Running)
			{
				vehicleComponents.streetVehicleModesHelper.SetEmptyStanding();
				if (onlyThrowOutDriver)
				{
					Stop();
				}
			}
			else if (animatorHandler.GetOnGyroboardState.Running)
			{
				vehicleComponents.streetVehicleModesHelper.SetEmptyStanding();
				base.transform.position = Vector3.MoveTowards(base.transform.position, vehicleComponents.sitPoint.position, Time.deltaTime * 3f);
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, vehicleComponents.sitPoint.rotation, Time.deltaTime * 250f);
			}
			else if (animatorHandler.StandingOnGyroboardState.Running)
			{
				if (vehicleComponents.streetVehicleModesHelper.Mode != StreetVehicleMode.Player && vehicleComponents.streetVehicleModesHelper.Mode != StreetVehicleMode.AI)
				{
					vehicleComponents.driver = base.transform;
					if (isPlayer)
					{
						vehicleComponents.streetVehicleModesHelper.SetPlayerState();
						cameraManager.SetVehicleCamera(vehicleComponents);
						panelsManager.ShowPanel(PanelType.Bicycle);
					}
					else
					{
						vehicleComponents.streetVehicleModesHelper.SetAIState();
					}
					vehicleComponents.doorReservator.ReleaseReservation(base.transform);
				}
			}
			else if (animatorHandler.GetOffGyroboardState.Running)
			{
				if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.4f)
				{
					base.transform.position = Vector3.MoveTowards(base.transform.position, vehicleComponents.handleTrigger.position, Time.deltaTime * 3f);
					base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, vehicleComponents.handleTrigger.rotation, Time.deltaTime * 250f);
				}
				vehicleComponents.streetVehicleModesHelper.SetEmptyStanding();
				if (isPlayer)
				{
					cameraManager.SetPlayerCamera();
					panelsManager.ShowPanel(PanelType.Game);
				}
			}
			else if (!animatorHandler.UseGyroboard)
			{
				SetVehicled(vehicled: false);
			}
		}

		protected override void SetVehicled(bool vehicled, VehicleComponents vehicleComponents = null, bool fixRotation = true)
		{
			if (vehicled && vehicleComponents.streetVehicleModesHelper.Mode == StreetVehicleMode.Free)
			{
				vehicleComponents.streetVehicleModesHelper.SetEmptyStanding();
			}
			base.SetVehicled(vehicled, vehicleComponents, fixRotation);
		}

		private void OnAnimatorIK(int layerIndex)
		{
			if (running && animatorHandler.StandingOnGyroboardState.Running && !(vehicleComponents.leftHandle == null))
			{
				animatorHandler.Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
				animatorHandler.Animator.SetIKPosition(AvatarIKGoal.LeftHand, vehicleComponents.leftHandle.position);
				animatorHandler.Animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
				animatorHandler.Animator.SetIKRotation(AvatarIKGoal.LeftHand, vehicleComponents.leftHandle.rotation);
				animatorHandler.Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
				animatorHandler.Animator.SetIKPosition(AvatarIKGoal.RightHand, vehicleComponents.rightHandle.position);
				animatorHandler.Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
				animatorHandler.Animator.SetIKRotation(AvatarIKGoal.RightHand, vehicleComponents.rightHandle.rotation);
			}
		}
	}
}
