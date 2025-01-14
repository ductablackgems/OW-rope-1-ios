using App.AI;
using App.Vehicles.Skid;

namespace App.Vehicles.Car.Navigation
{
	public class SkidBrake : AbstractAIScript
	{
		private SkidEffectControl skidEffectControl;

		private DurationTimer forceBrakeTimer = new DurationTimer();

		public void ForceBrake(float duration)
		{
			forceBrakeTimer.Run(duration);
			if (skidEffectControl != null && !skidEffectControl.EffectEnabled)
			{
				skidEffectControl.EffectEnabled = true;
			}
		}

		public bool IsBreaking()
		{
			return forceBrakeTimer.InProgress();
		}

		private void Awake()
		{
			skidEffectControl = base.ComponentsRoot.GetComponent<SkidEffectControl>();
		}

		private void OnDisable()
		{
			forceBrakeTimer.Stop();
			if (skidEffectControl != null && skidEffectControl.EffectEnabled)
			{
				skidEffectControl.EffectEnabled = false;
			}
		}

		private void Update()
		{
			if (skidEffectControl != null && !forceBrakeTimer.InProgress() && skidEffectControl.EffectEnabled)
			{
				skidEffectControl.EffectEnabled = false;
			}
		}
	}
}
