using App.Util;
using System.Collections.Generic;
using UnityEngine;

namespace App.Quests
{
	public class ObjectiveCollect : GameplayObjective
	{
		private CollectableItem[] prefabs = new CollectableItem[0];

		private List<CollectableItem> itemsToCollect = new List<CollectableItem>(16);

		protected override void OnInitialized()
		{
			base.OnInitialized();
			prefabs = GetComponentsInChildren<CollectableItem>();
			CollectableItem[] array = prefabs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(value: true);
			}
		}

		protected override void OnActivated()
		{
			base.OnActivated();
			SpawnItems();
		}

		protected override void OnDeactivated()
		{
			base.OnDeactivated();
			DespawnItems();
		}

		private void OnItemCollected(CollectableItem item)
		{
			itemsToCollect.Remove(item);
			item.Collected -= OnItemCollected;
			if (itemsToCollect.Count == 0)
			{
				Finish();
			}
		}

		private void SpawnItems()
		{
			itemsToCollect.Clear();
			CollectableItem[] array = prefabs;
			foreach (CollectableItem collectableItem in array)
			{
				CollectableItem collectableItem2 = Object.Instantiate(collectableItem, base.transform);
				collectableItem2.transform.position = collectableItem.transform.position;
				collectableItem2.Collected += OnItemCollected;
				collectableItem2.gameObject.SetActive(value: true);
				itemsToCollect.Add(collectableItem2);
				collectableItem.gameObject.SetActive(value: false);
			}
		}

		private void DespawnItems()
		{
            int num = itemsToCollect.Count;
            while (num-- > 0)
            {
                CollectableItem collectableItem = itemsToCollect[num];
                collectableItem.Collected -= OnItemCollected;
                itemsToCollect.RemoveAt(num);
                UnityEngine.Object.Destroy(collectableItem.gameObject);
            }
		}
	}
}
