namespace App.Quests
{
	public class ObjectiveDestroyVehicle : VehicleGameplayObjective
	{
		protected override void OnActivated()
		{
			base.OnActivated();
			SpawnVehicle();
			EnableDestroyer(enable: false);
		}

		protected override void OnVehicleDestroyed()
		{
			EnableDestroyer(enable: true);
			base.OnVehicleDestroyed();
			Finish();
		}
	}
}
