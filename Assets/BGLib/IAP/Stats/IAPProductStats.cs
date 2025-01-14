using System;
using UnityEngine;
using UnityEngine.Purchasing;

namespace BG_Library.IAP
{
    [CreateAssetMenu(fileName = "IAP product stats", menuName = "BG/IAP/IAP product stats")]

    public class IAPProductStats : ScriptableObject
    {
        public enum EReward { REMOVE_AD = 0, CURRENCY = 1, CUSTOM = 2 }

        [Serializable]
        public class PurchaseReward
        {
            public EReward Reward;
            public string atlas;
            public int PackRewardValue;
        }

        public string Id => name;
        public string TrackingId => name.Split('.')[^1];
        public ProductType ProductType;
        public PurchaseReward[] Rewards;
    }
}