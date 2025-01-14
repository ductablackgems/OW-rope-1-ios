using UnityEngine;

namespace App.Vehicles.Airplane.AI
{
	public class LoopAirTraffic : AirTraffic
	{
		[SerializeField]
		private float duration;

		private DurationTimer timer = new DurationTimer();

		public override bool IsLoop => true;

		public override bool IsFallback => true;

		protected override void OnRun()
		{
			base.OnRun();
			timer.Run(duration);
			airplane.SetOrientation = true;
		}

		protected override void OnStop()
		{
			base.OnStop();
			timer.Stop();
		}

		protected override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			if (timer.Done())
			{
				timer.Stop();
				OnAirplanePathFinished();
			}
		}
	}
}
