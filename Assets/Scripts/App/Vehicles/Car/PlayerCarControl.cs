using App.Vehicles.Car.Navigation;
using UnityEngine;

namespace App.Vehicles.Car
{
    public class PlayerCarControl : MonoBehaviour, IPlayerVehicleController
    {
        private CarController carController;

        private Rigidbody _rigidbody;

        private float horizontalInput;

        private void Awake()
        {
            carController = this.GetComponentSafe<CarController>();
            _rigidbody = this.GetComponentSafe<Rigidbody>();
        }

        private void OnEnable()
        {
            horizontalInput = 0f;
        }

        private void FixedUpdate()
        {
            horizontalInput = VehicleInputsHelper.GetHorizonatalInput(horizontalInput);
            float num = VehicleInputsHelper.GetVerticalInput();
            float footbrake = 0f;
            float z = base.transform.InverseTransformDirection(_rigidbody.velocity).z;
            if (z > 2f && num < 0f)
            {
                footbrake = num;
                num = 0f;
            }
            else if (z < -2f && num > 0f)
            {
                footbrake = 0f - num;
                num = 0f;
            }
            carController.Move(horizontalInput, num, footbrake, 0f);
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
