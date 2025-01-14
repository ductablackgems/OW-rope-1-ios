using System;

namespace App
{
	public class WhoIsMasks
	{
		public WhoIsItemDefinition[] AllVehicles
		{
			get;
			private set;
		}

		public WhoIsItemDefinition[] AllHumans
		{
			get;
			private set;
		}

		public WhoIsItemDefinition[] AllRagdollableHumans
		{
			get;
			private set;
		}

		public WhoIsItemDefinition[] AllAnimals
		{
			get;
			private set;
		}

		public WhoIsItemDefinition[] AllStreetVehicles
		{
			get;
			private set;
		}

		public WhoIsItemDefinition[] BulletImpactResolving
		{
			get;
			private set;
		}

		public WhoIsItemDefinition[] FiretruckWater
		{
			get;
			private set;
		}

		public WhoIsItemDefinition[] MolotovExplosion
		{
			get;
			private set;
		}

		public WhoIsItemDefinition[] Flamethrower
		{
			get;
			private set;
		}

		public WhoIsItemDefinition[] GarageInterior
		{
			get;
			private set;
		}

		public WhoIsItemDefinition[] AIScanner
		{
			get;
			private set;
		}

		public WhoIsItemDefinition[] PullableByRope
		{
			get;
			private set;
		}

		public WhoIsItemDefinition[] AICarBoxSensor
		{
			get;
			private set;
		}

		public WhoIsMasks(WhoIsEntities entities)
		{
			AllVehicles = new WhoIsItemDefinition[4]
			{
				entities.Vehicle,
				entities.Bicycle,
				entities.Gyroboard,
				entities.Skateboard
			};
			AllHumans = new WhoIsItemDefinition[2]
			{
				entities.Player,
				entities.Enemy
			};
			AllRagdollableHumans = new WhoIsItemDefinition[2]
			{
				entities.RagdollablePlayer,
				entities.RagdollableEnemy
			};
			AllAnimals = new WhoIsItemDefinition[1]
			{
				entities.Dog
			};
			AllStreetVehicles = new WhoIsItemDefinition[3]
			{
				entities.Bicycle,
				entities.Gyroboard,
				entities.Skateboard
			};
			BulletImpactResolving = MergeMasks(AllHumans, AllVehicles, AllAnimals, new WhoIsItemDefinition[3]
			{
				entities.RagdollPart,
				entities.Lamp,
				entities.FallenVehiclePart
			});
			FiretruckWater = MergeMasks(AllRagdollableHumans, AllVehicles);
			MolotovExplosion = MergeMasks(AllRagdollableHumans, AllVehicles);
			Flamethrower = MergeMasks(AllRagdollableHumans, AllVehicles);
			GarageInterior = new WhoIsItemDefinition[2]
			{
				entities.Vehicle,
				entities.Player
			};
			AIScanner = MergeMasks(AllVehicles, new WhoIsItemDefinition[2]
			{
				entities.Player,
				entities.Dog
			});
			PullableByRope = MergeMasks(AllVehicles, new WhoIsItemDefinition[1]
			{
				entities.RagdollableEnemy
			});
			AICarBoxSensor = MergeMasks(AllVehicles, new WhoIsItemDefinition[2]
			{
				entities.RagdollableEnemy,
				entities.Player
			});
		}

		private WhoIsItemDefinition[] MergeMasks(params WhoIsItemDefinition[][] masks)
		{
			int num = 0;
			foreach (WhoIsItemDefinition[] array in masks)
			{
				num += array.Length;
			}
			WhoIsItemDefinition[] array2 = new WhoIsItemDefinition[num];
			int num2 = 0;
			foreach (WhoIsItemDefinition[] array3 in masks)
			{
				Array.Copy(array3, 0, array2, num2, array3.Length);
				num2 += array3.Length;
			}
			return array2;
		}
	}
}
