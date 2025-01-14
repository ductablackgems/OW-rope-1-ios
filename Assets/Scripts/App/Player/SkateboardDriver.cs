using App.GUI;
using App.Util;
using App.Vehicles;
using App.Vehicles.Bicycle;
using UnityEngine;

namespace App.Player
{
	public class SkateboardDriver : AbstractVehicleDriver
	{
		private DurationTimer startSkateboardTimer = new DurationTimer();

		protected override AnimatorState GetSittingState()
		{
			return animatorHandler.SkateboardGroundedState;
		}

		protected override AnimatorState GetThrowOutDriverState()
		{
			return animatorHandler.ThrowOutSkateboardDriverState;
		}

		protected override bool GetUseVehicleParameter()
		{
			return animatorHandler.UseSkateboard;
		}

		protected override VehicleType GetVehicleType()
		{
			return VehicleType.Skateboard;
		}

		protected override void SetUseVehicleParameter(bool useVehicle)
		{
			animatorHandler.UseSkateboard = useVehicle;
		}

		protected override bool SittingIn()
		{
			return animator.GetCurrentAnimatorStateInfo(0).IsTag("ActiveSkateboard");
		}

		private void Update()
		{
			if (!running)
			{
				return;
			}
			if (moveTowardVehicleHandle)
			{
				base.transform.position = Vector3.MoveTowards(base.transform.position, vehicleComponents.handleTrigger.position, Time.deltaTime * 3f);
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, vehicleComponents.handleTrigger.rotation, Time.deltaTime * 250f);
				if (base.transform.position == vehicleComponents.handleTrigger.position && base.transform.rotation == vehicleComponents.handleTrigger.rotation)
				{
					moveTowardVehicleHandle = false;
					animatorHandler.UseSkateboard = true;
					vehicleComponents.skateboardAnimatorHandler.enabled = true;
					startSkateboardTimer.Run(0.5f);
				}
			}
			else if (animatorHandler.ThrowOutSkateboardDriverState.Running)
			{
				vehicleComponents.streetVehicleModesHelper.SetEmptyStanding();
				if (onlyThrowOutDriver)
				{
					Stop();
				}
			}
			else if (animatorHandler.GetOnSkateboardState.Running)
			{
				vehicleComponents.streetVehicleModesHelper.SetEmptyStanding();
				base.transform.position = Vector3.MoveTowards(base.transform.position, vehicleComponents.sitPoint.position, Time.deltaTime * 3f);
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, vehicleComponents.sitPoint.rotation, Time.deltaTime * 250f);
				if (startSkateboardTimer.Done())
				{
					animatorHandler.TriggerStartSkateboard();
					vehicleComponents.skateboardAnimatorHandler.TriggerStart();
					startSkateboardTimer.Stop();
				}
			}
			else if (animator.GetCurrentAnimatorStateInfo(0).IsTag("ActiveSkateboard"))
			{
				if (vehicleComponents.streetVehicleModesHelper.Mode != StreetVehicleMode.Player && vehicleComponents.streetVehicleModesHelper.Mode != StreetVehicleMode.AI)
				{
					vehicleComponents.driver = base.transform;
					if (isPlayer)
					{
						vehicleComponents.streetVehicleModesHelper.SetPlayerState();
						cameraManager.SetSkateboardCamera(vehicleComponents.gameObject);
						panelsManager.ShowPanel(PanelType.Bicycle);
					}
					else
					{
						vehicleComponents.streetVehicleModesHelper.SetAIState();
					}
					vehicleComponents.doorReservator.ReleaseReservation(base.transform);
				}
			}
			else if (animatorHandler.GetOffSkateboardState.Running)
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
			else if (!animatorHandler.UseSkateboard)
			{
				SetVehicled(vehicled: false);
			}
		}

		public void OnAnimatorMove()
		{
			if (running && Time.deltaTime != 0f && animator.GetCurrentAnimatorStateInfo(0).IsTag("ActiveSkateboard"))
			{
				vehicleComponents.sitPoint.localPosition = Vector3.up * animatorHandler.SkaterYOffset;
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

		private void Start()
		{
			if (!isPlayer)
			{
				animatorHandler.SkaterDirection.ForceValue(1f);
			}
		}
	}
}
