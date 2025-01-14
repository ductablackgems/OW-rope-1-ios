using App.Vehicles.Motorbike;
using UnityEngine;

namespace App.Vehicles.Car
{
	public class CarStopper : MonoBehaviour
	{
		private VehicleComponents components;

		private Rigidbody _rigidbody;

		private CarController carController;

		private MotorbikeControl motorbikeControl;

		private DurationTimer paralyzedTimer = new DurationTimer();

		private bool stoped;

		public bool Stopping
		{
			get;
			private set;
		}

		public void Paralyze()
		{
			paralyzedTimer.Run(4f);
		}

		private void Awake()
		{
			components = this.GetComponentSafe<VehicleComponents>();
			_rigidbody = this.GetComponentSafe<Rigidbody>();
			carController = GetComponent<CarController>();
			motorbikeControl = GetComponent<MotorbikeControl>();
		}

		private void Update()
		{
			if (paralyzedTimer.Running())
			{
				if (paralyzedTimer.InProgress())
				{
					return;
				}
				paralyzedTimer.Stop();
			}
			if (components.driver == null)
			{
				Stopping = true;
				if ((double)Mathf.Abs(base.transform.InverseTransformDirection(_rigidbody.velocity).z) < 0.1)
				{
					if (!stoped)
					{
						if (carController == null)
						{
							motorbikeControl.Control(0f, 0f);
						}
						else
						{
							carController.Move(0f, 0f, 0f, 0f);
						}
						stoped = true;
						_rigidbody.velocity = Vector3.zero;
						_rigidbody.Sleep();
					}
				}
				else if (carController == null)
				{
					motorbikeControl.Control(0f, 0f, 1f);
				}
				else
				{
					carController.Move(0f, 0f, 0f, 1f);
				}
			}
			else
			{
				Stopping = false;
				stoped = false;
			}
		}
	}
}
