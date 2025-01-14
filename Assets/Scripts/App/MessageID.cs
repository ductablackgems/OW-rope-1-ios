namespace App
{
	public static class MessageID
	{
		public static class Player
		{
			public static readonly string Root = "player";

			public static readonly string Died = Root + ".died";

			public static readonly string Respawned = Root + ".respawned";

			public static readonly string WeaponChanged = Root + "weapon.changed";

			public static readonly string VehicleEnter = Root + "vehicle.enter";

			public static readonly string VehicleLeave = Root + "vehicle.leave";

			public static readonly string RespawnDialogClosed = Root + "respawn.dialog.closed";
		}

		public static class Dog
		{
			public static readonly string Root = "dog";

			public static readonly string Died = Root + ".died";

			public static readonly string Revived = Root + ".revived";
		}

		public static class Shop
		{
			public static readonly string Root = "shop";

			public static readonly string ItemBought = Root + ".item.bought";
		}

		public static class Vehicle
		{
			public static readonly string Root = "vehicle";

			public static readonly string Enter = Root + ".enter";

			public static readonly string Leave = Root + ".leave";
		}

		public static class Airplane
		{
			public static readonly string Root = "airplane";

			public static readonly string Enter = Root + ".enter";

			public static readonly string Leave = Root + ".leave";

			public static readonly string BorderLeave = Root + ".border.leave";

			public static readonly string BorderEnter = Root + ".border.enter";
		}

		public static class Health
		{
			public static readonly string Root = "health";

			public static readonly string Changed = Root + ".changed";

			public static readonly string Dead = Root + ".dead";
		}
	}
}
