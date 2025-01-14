namespace App.Quests
{
	public class ObjectiveUseVehicle : VehicleGameplayObjective
	{
		protected override void OnActivated()
		{
			base.OnActivated();
			data.Vehicle = SpawnVehicle();
		}

		protected override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			if (!(vehicle == null))
			{
				if (vehicle.driver == base.Player.Transform)
				{
					Finish();
				}
				else if (data.IsVehicleKilled)
				{
					Fail();
				}
			}
		}

		protected override void OnVehicleDestroyed()
		{
			base.OnVehicleDestroyed();
			Fail();
		}
	}
}
