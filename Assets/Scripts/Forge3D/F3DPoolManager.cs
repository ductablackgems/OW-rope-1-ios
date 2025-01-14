using System;
using System.Collections.Generic;
using UnityEngine;

namespace Forge3D
{
	[Serializable]
	public class F3DPoolManager : MonoBehaviour
	{
		public static F3DPoolManager instance;

		public static Dictionary<string, F3DPool> Pools = new Dictionary<string, F3DPool>();

		public string databaseName = "";

		public int selectedItem;

		public bool[] haveToShowArr;

		private List<F3DPool> curPools = new List<F3DPool>();

		private void Awake()
		{
			InstallManager();
			instance = this;
		}

		public F3DPool GetPool(string valueName)
		{
			if (valueName != "" && curPools != null && curPools.Count > 0)
			{
				for (int i = 0; i < curPools.Count; i++)
				{
					if (curPools[i].poolName == valueName)
					{
						return curPools[i];
					}
				}
			}
			return null;
		}

		private void InstallManager()
		{
			curPools.Clear();
			List<F3DPoolContainer> list = new List<F3DPoolContainer>();
			Pools.Clear();
			Pools = new Dictionary<string, F3DPool>();
			F3DPoolManagerDB f3DPoolManagerDB = Resources.Load("F3DPoolManagerCache/" + databaseName) as F3DPoolManagerDB;
			if (f3DPoolManagerDB != null && f3DPoolManagerDB.pools != null)
			{
				foreach (F3DPoolContainer pool in f3DPoolManagerDB.pools)
				{
					list.Add(pool);
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				GameObject gameObject = new GameObject();
				gameObject.transform.parent = base.gameObject.transform;
				gameObject.name = list[i].poolName;
				F3DPool f3DPool = gameObject.AddComponent<F3DPool>();
				f3DPool.poolName = gameObject.name;
				f3DPool.SetTemplates(list[i].templates);
				f3DPool.SetLength(list[i].poolLength);
				f3DPool.SetLengthMax(list[i].poolLengthMax);
				f3DPool.needBroadcasting = list[i].needBroadcasting;
				f3DPool.broadcastSpawnName = list[i].broadcastSpawnName;
				f3DPool.broadcastDespawnName = list[i].broadcastDespawnName;
				f3DPool.needSort = list[i].needSort;
				f3DPool.delayedSpawnInInstall = list[i].delayedSpawnInInstall;
				f3DPool.objectsPerUpdate = list[i].objectsPerUpdate;
				f3DPool.optimizeSpawn = list[i].optimizeSpawn;
				f3DPool.targetFPS = list[i].targetFPS;
				f3DPool.needSort = list[i].needSort;
				f3DPool.needParenting = list[i].needParenting;
				f3DPool.needDebugging = list[i].needDebug;
				f3DPool.pooling = true;
				f3DPool.Install();
				curPools.Add(f3DPool);
				Pools.Add(f3DPool.name, f3DPool);
			}
		}

		public int GetPoolsCount()
		{
			if (curPools != null)
			{
				return curPools.Count;
			}
			return -1;
		}
	}
}
