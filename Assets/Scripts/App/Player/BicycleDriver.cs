using App.GUI;
using App.Util;
using App.Vehicles;
using App.Vehicles.Bicycle;
using UnityEngine;

namespace App.Player
{
	public class BicycleDriver : AbstractVehicleDriver
	{
		public void OnGetOffBike()
		{
		}

		protected override AnimatorState GetSittingState()
		{
			return animatorHandler.SittingOnBicycleState;
		}

		protected override AnimatorState GetThrowOutDriverState()
		{
			return animatorHandler.ThrowOutBicycleDriverState;
		}

		protected override bool GetUseVehicleParameter()
		{
			return animatorHandler.UseBicycle;
		}

		protected override VehicleType GetVehicleType()
		{
			return VehicleType.Bicycle;
		}

		protected override void SetUseVehicleParameter(bool useVehicle)
		{
			animatorHandler.UseBicycle = useVehicle;
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
				vehicleComponents.streetVehicleModesHelper.SetEmptyStanding();
				Transform transform = animatorHandler.MirrorGetInVehicle ? vehicleComponents.rightHandleTrigger : vehicleComponents.handleTrigger;
				base.transform.position = Vector3.MoveTowards(base.transform.position, transform.position, Time.deltaTime * 3f);
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, transform.rotation, Time.deltaTime * 250f);
				if (base.transform.position == transform.position && base.transform.rotation == transform.rotation)
				{
					moveTowardVehicleHandle = false;
					animatorHandler.UseBicycle = true;
				}
			}
			else if (animatorHandler.ThrowOutBicycleDriverState.Running)
			{
				vehicleComponents.streetVehicleModesHelper.SetEmptyStanding();
				if (onlyThrowOutDriver)
				{
					Stop();
				}
			}
			else if (animatorHandler.GetOnBicycleState.Running)
			{
				vehicleComponents.streetVehicleModesHelper.SetEmptyStanding();
				if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.4f)
				{
					base.transform.position = Vector3.MoveTowards(base.transform.position, vehicleComponents.sitPoint.position, Time.deltaTime * 3f);
					base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, vehicleComponents.sitPoint.rotation, Time.deltaTime * 250f);
				}
			}
			else if (animatorHandler.SittingOnBicycleState.Running)
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
			else if (animatorHandler.GetOffBicycleState.Running)
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
			else if (!animatorHandler.UseBicycle)
			{
				SetVehicled(vehicled: false);
			}
		}

		protected override void SetVehicled(bool vehicled, VehicleComponents vehicleComponents = null, bool fixRotation = true)
		{
			if (vehicled && vehicleComponents.streetVehicleModesHelper.Mode == StreetVehicleMode.Free)
			{
				vehicleComponents.streetVehicleModesHelper.SetEmptyStanding();
				bool flag = (vehicleComponents.handleTrigger.position - base.transform.position).magnitude > (vehicleComponents.rightHandleTrigger.position - base.transform.position).magnitude;
				if (animatorHandler.MirrorGetInVehicle != flag)
				{
					animatorHandler.MirrorGetInVehicle = flag;
				}
			}
			if (vehicled)
			{
				animatorHandler.SItOnBicycleBlend = vehicleComponents.sittingBlend;
			}
			base.SetVehicled(vehicled, vehicleComponents, fixRotation);
		}

		private void OnAnimatorIK(int layerIndex)
		{
			if (running && animatorHandler.SittingOnBicycleState.Running)
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
				animatorHandler.Animator.SetIKPosition(AvatarIKGoal.LeftFoot, vehicleComponents.leftPedal.position + Vector3.up * 0.05f);
				animatorHandler.Animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
				animatorHandler.Animator.SetIKPosition(AvatarIKGoal.RightFoot, vehicleComponents.rightPedal.position + Vector3.up * 0.05f);
			}
		}
	}
}
