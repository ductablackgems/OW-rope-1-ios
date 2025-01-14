using System;
using System.Linq;
using UnityEngine;

namespace App.Prefabs
{
	public class VehiclePrefabsScriptableObject : ScriptableObject
	{
		public VehiclePrefabId[] prefabs;

		public VehiclePrefabId[] bikePrefabs;

		public VehiclePrefabId[] superSportPrefabs;

		public VehiclePrefabId[] policePrefabs;

		public VehiclePrefabId[] taxiPrefabs;

		public VehiclePrefabId[] ambulancePrefabs;

		public VehiclePrefabId[] firetruckPrefabs;

		public VehiclePrefabId[] garbageTruckPrefabs;

		public VehiclePrefabId[] skateboardPrefabs;

		public VehiclePrefabId[] airplanePrefabs;

		public GameObject[] bicycles;

		public GameObject[] gyroboards;

		public VehiclePrefabId Find(string tid)
		{
			try
			{
				return prefabs.First((VehiclePrefabId prefab) => prefab.tid == tid);
			}
			catch (InvalidOperationException)
			{
			}
			try
			{
				return superSportPrefabs.First((VehiclePrefabId prefab) => prefab.tid == tid);
			}
			catch (InvalidOperationException)
			{
			}
			try
			{
				return policePrefabs.First((VehiclePrefabId prefab) => prefab.tid == tid);
			}
			catch (InvalidOperationException)
			{
			}
			try
			{
				return bikePrefabs.First((VehiclePrefabId prefab) => prefab.tid == tid);
			}
			catch (InvalidOperationException)
			{
			}
			try
			{
				return taxiPrefabs.First((VehiclePrefabId prefab) => prefab.tid == tid);
			}
			catch (InvalidOperationException)
			{
			}
			try
			{
				return ambulancePrefabs.First((VehiclePrefabId prefab) => prefab.tid == tid);
			}
			catch (InvalidOperationException)
			{
			}
			try
			{
				return firetruckPrefabs.First((VehiclePrefabId prefab) => prefab.tid == tid);
			}
			catch (InvalidOperationException)
			{
			}
			try
			{
				return garbageTruckPrefabs.First((VehiclePrefabId prefab) => prefab.tid == tid);
			}
			catch (InvalidOperationException)
			{
			}
			return null;
		}
	}
}
