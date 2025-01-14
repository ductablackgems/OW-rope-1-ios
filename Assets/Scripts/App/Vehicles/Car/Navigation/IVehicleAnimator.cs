namespace App.Vehicles.Car.Navigation
{
	public interface IVehicleAnimator
	{
		void Animate(float deltaTime);

		bool Animated();

		void SetAnimated(bool animated);
	}
}
