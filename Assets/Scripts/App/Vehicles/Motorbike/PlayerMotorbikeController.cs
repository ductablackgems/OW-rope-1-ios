using App.Vehicles.Car.Navigation;
using UnityEngine;

namespace App.Vehicles.Motorbike
{
    public class PlayerMotorbikeController : MonoBehaviour, IPlayerVehicleController
    {
        private MotorbikeControl motorbikeControl;

        private Rigidbody _rigidbody;

        private float horizontalInput;

        private void Awake()
        {
            motorbikeControl = this.GetComponentSafe<MotorbikeControl>();
            _rigidbody = this.GetComponentSafe<Rigidbody>();
        }

        private void OnEnable()
        {
            horizontalInput = 0f;
        }

        private void FixedUpdate()
        {
            if (motorbikeControl.AiOptimized)
            {
                motorbikeControl.AiOptimized = false;
            }
            horizontalInput = VehicleInputsHelper.GetHorizonatalInput(horizontalInput);
            float num = VehicleInputsHelper.GetVerticalInput();
            float brake = 0f;
            float z = base.transform.InverseTransformDirection(_rigidbody.velocity).z;
            if (z > 2f && num < 0f)
            {
                brake = 0f - num;
                num = 0f;
            }
            else if (z < -2f && num > 0f)
            {
                brake = num;
                num = 0f;
            }
            motorbikeControl.Control(horizontalInput, num, brake);
        }

        bool IPlayerVehicleController.enabled
        {
            get
            {
                return base.enabled;
            }
            set
            {
                base.enabled = value;
            }
        }
    }
}
