namespace App.Vehicles.Car.Navigation
{
	public interface IVehicleController
	{
		void Move(float steering, float accel, float footbrake, float handbrake);

		void SetOptimizedForAI(bool value);

		void UpdateOldRotation();

		void SteerHelper();

		bool MoveEachFrame();
	}
}
