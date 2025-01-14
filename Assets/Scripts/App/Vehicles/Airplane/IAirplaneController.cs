namespace App.Vehicles.Airplane
{
	public interface IAirplaneController
	{
		bool IsActive
		{
			get;
		}

		float EnginePower
		{
			get;
		}

		float MaxEnginePower
		{
			get;
		}

		float ForwardSpeed
		{
			get;
		}

		float Throttle
		{
			get;
		}
	}
}
