using System;
using System.Collections.Generic;
using UnityEngine;

namespace BG_Library.IAP
{
    public partial class IAPManager
    {
        [System.Serializable]
        public class PurchaseData
        {
            public string ProductId;
            public int PurchaseCount;
        }

        [System.Serializable]
        public class UserData
        {
            public List<PurchaseData> PurchaseDatas = new List<PurchaseData>();
        }

        private static UserData _data;
        const string DATA_ID = "UserPurchaseData";

        private static void SavePurchaseProduct(string productId)
        {
            UserData data = GetData();
            PurchaseData purchaseData = GetPurchaseData(productId);
            if (purchaseData == null)
            {
                purchaseData = new PurchaseData();
                purchaseData.ProductId = productId;
                data.PurchaseDatas.Add(purchaseData);
            }
            purchaseData.PurchaseCount++;
            Save();
        }

        public static int GetPurchaseProductCount(string productId)
        {
            PurchaseData purchaseData = GetPurchaseData(productId);
            if (purchaseData == null)
                return 0;
            return purchaseData.PurchaseCount;
        }

        public static bool IsPurchaseProduct(string productId)
        {
            UserData data = GetData();
            for (var i = 0; i < data.PurchaseDatas.Count; i++)
                if (data.PurchaseDatas[i].ProductId.Equals(productId))
                    return true;
            return false;
        }

        private static PurchaseData GetPurchaseData(string productId)
        {
            UserData data = GetData();
            return data.PurchaseDatas.Find(purchaseData => purchaseData.ProductId.Equals(productId));
        }

        private static void Save()
        {
            PlayerPrefs.SetString(DATA_ID, JsonUtility.ToJson(GetData()));
            PlayerPrefs.Save();
        }

        private static UserData GetData()
        {
            if (_data == null)
            {
                _data = JsonUtility.FromJson<UserData>(PlayerPrefs.GetString(DATA_ID));
                if (_data == null) _data = new UserData();
            }

            return _data;
        }
    }
}