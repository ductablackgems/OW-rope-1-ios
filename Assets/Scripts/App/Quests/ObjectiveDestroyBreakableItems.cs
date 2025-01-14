using App.Vehicles;
using System.Collections.Generic;
using UnityEngine;

namespace App.Quests
{
    public class ObjectiveDestroyBreakableItems : GameplayObjective
    {
        [Header("Objective Destroy Breakable Items")]
        [SerializeField] GameObject targetsContainer;

        [SerializeField]
        private int amount;

        public List<BreakableItemInstantiate> items = new List<BreakableItemInstantiate>(128);

        private int counter;

        protected override void OnActivated()
        {
            base.OnActivated();
            targetsContainer.GetComponentsInChildren(true, items);
            for (int i = 0; i < items.Count; i++)
            {
                items[i].Breaked += OnItemBreaked;

            }
            base.ShortDecription = string.Format(shortDescriptionOrg, counter, amount);
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            RemoveListeners();
        }

        protected override void OnReset()
        {
            base.OnReset();
            counter = 0;
            RemoveListeners();
            items.Clear();
        }

        private void OnItemBreaked(BreakableItemInstantiate item)
        {
            counter++;
            item.Breaked -= OnItemBreaked;
            items.Remove(item);
            base.ShortDecription = string.Format(shortDescriptionOrg, counter, amount);
            if (counter >= amount)
            {
                Finish();
            }
        }

        private void RemoveListeners()
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].Breaked -= OnItemBreaked;
            }
        }
    }
}
