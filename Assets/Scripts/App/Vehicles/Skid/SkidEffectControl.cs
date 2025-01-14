using App.Vehicles.Car;
using App.Vehicles.Car.Navigation;
using App.Vehicles.Motorbike;
using UnityEngine;

namespace App.Vehicles.Skid
{
	public class SkidEffectControl : MonoBehaviour
	{
		public AudioSource audioSource;

		public bool isBike;

		private VehicleModesHandler vehicleModesHandler;

		private Rigidbody _rigidbody;

		private IHandbrakeVehicle handbrakeVehicle;

		private WheelCollider[] wheelColliders;

		private WheelEffects[] wheelEffects;

		private bool effectForced;

		private bool skidActive;

		private DurationTimer checkTimer = new DurationTimer();

		public bool EffectEnabled
		{
			get
			{
				return checkTimer.Running();
			}
			set
			{
				if (value)
				{
					checkTimer.FakeDone(1f);
					base.enabled = true;
					return;
				}
				checkTimer.Stop();
				if (skidActive)
				{
					StopEffect();
				}
			}
		}

		public bool EffectForced
		{
			get
			{
				return effectForced;
			}
			set
			{
				if (value)
				{
					effectForced = value;
					base.enabled = true;
					return;
				}
				if (EffectEnabled)
				{
					effectForced = value;
					return;
				}
				effectForced = value;
				if (skidActive)
				{
					StopEffect();
				}
			}
		}

		private void Awake()
		{
			if (isBike)
			{
				MotorbikeControl motorbikeControl = (MotorbikeControl)(handbrakeVehicle = this.GetComponentSafe<MotorbikeControl>());
				wheelColliders = new WheelCollider[1]
				{
					motorbikeControl.backWheel
				};
				wheelEffects = GetComponentsInChildren<WheelEffects>();
			}
			else
			{
				CarController carController = (CarController)(handbrakeVehicle = this.GetComponentSafe<CarController>());
				wheelColliders = carController.WheelColliders;
				wheelEffects = carController.WheelEffects;
			}
			vehicleModesHandler = this.GetComponentSafe<VehicleModesHandler>();
			_rigidbody = this.GetComponentInChildrenSafe<Rigidbody>();
		}

		private void Update()
		{
			if (!EffectEnabled && !EffectForced)
			{
				base.enabled = false;
			}
			else if (vehicleModesHandler.mode != VehicleMode.AI)
			{
				EffectEnabled = false;
				EffectForced = false;
			}
			else if (EffectForced)
			{
				if (!skidActive)
				{
					StartEffect();
				}
			}
			else
			{
				if (checkTimer.InProgress())
				{
					return;
				}
				checkTimer.Run(0.5f);
				bool num = handbrakeVehicle.HandbreakInput > 0.5f && _rigidbody.velocity.magnitude > 0.3f;
				bool flag = false;
				if (num)
				{
					for (int i = 0; i < wheelColliders.Length; i++)
					{
						if (wheelColliders[i].isGrounded)
						{
							flag = true;
							break;
						}
					}
				}
				if (skidActive != flag)
				{
					if (flag)
					{
						StartEffect();
					}
					else
					{
						StopEffect();
					}
				}
			}
		}

		public void StartEffect()
		{
			skidActive = true;
			for (int i = 0; i < wheelEffects.Length; i++)
			{
				wheelEffects[i].EmitTyreSmoke();
			}
			audioSource.Play();
		}

		public void StopEffect()
		{
			skidActive = false;
			for (int i = 0; i < wheelEffects.Length; i++)
			{
				wheelEffects[i].EndSkidTrail();
			}
			audioSource.Stop();
		}
	}
}
