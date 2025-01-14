namespace App
{
	public class WhoIsEntities
	{
		public readonly WhoIsItemDefinition Player = new WhoIsItemDefinition("Player");

		public readonly WhoIsItemDefinition Enemy = new WhoIsItemDefinition("Enemy");

		public readonly WhoIsItemDefinition RagdollablePlayer = new WhoIsItemDefinition("Player", "RagdollPart", hasColliderOnRoot: true);

		public readonly WhoIsItemDefinition RagdollableEnemy = new WhoIsItemDefinition("Enemy", "RagdollPart", hasColliderOnRoot: true);

		public readonly WhoIsItemDefinition RagdollPart = new WhoIsItemDefinition("RagdollPart");

		public readonly WhoIsItemDefinition Vehicle = new WhoIsItemDefinition("Vehicle", "VehicleCollider", hasColliderOnRoot: true);

		public readonly WhoIsItemDefinition FallenVehiclePart = new WhoIsItemDefinition("FallenVehiclePart");

		public readonly WhoIsItemDefinition Gyroboard = new WhoIsItemDefinition("Gyroboard", "GyroboardCollider");

		public readonly WhoIsItemDefinition Bicycle = new WhoIsItemDefinition("Bicycle", "BicycleCollider");

		public readonly WhoIsItemDefinition Skateboard = new WhoIsItemDefinition("Skateboard", "SkateboardCollider");

		public readonly WhoIsItemDefinition Lamp = new WhoIsItemDefinition("Lamp");

		public readonly WhoIsItemDefinition Dog = new WhoIsItemDefinition("Dog");
	}
}
