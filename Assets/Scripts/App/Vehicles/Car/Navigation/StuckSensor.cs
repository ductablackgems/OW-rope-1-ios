namespace App.Vehicles.Car.Navigation
{
	public class StuckSensor
	{
		private const float StuckDuration = 6f;

		public const float StuckSpeed = 0.2f;

		private DurationTimer stuckTimer = new DurationTimer(useFixedTime: true);

		public void Clear()
		{
			stuckTimer.Stop();
		}

		public void Update(float vertical, float absoluteSpeed)
		{
			if (absoluteSpeed < 0.2f && vertical != 0f)
			{
				if (!stuckTimer.Running())
				{
					stuckTimer.Run(6f);
				}
			}
			else
			{
				stuckTimer.Stop();
			}
		}

		public bool Stuck()
		{
			return stuckTimer.Done();
		}
	}
}
