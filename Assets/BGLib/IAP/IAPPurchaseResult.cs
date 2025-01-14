using UnityEngine.Purchasing;

namespace BG_Library.IAP
{
    public struct IAPPurchaseResult
    {
        public enum EResult
        {
            Complete = 0, Restore = 1, WrongProduct = 2,
            WrongInstance = 3, WrongStoreController = 4,
            PurchaseFailed = 5
        }

        public IAPProductStats Product;
        public Product PurchasedProduct;
        public string Position;
        public string ProductId;
        public PurchaseFailureReason PurchaseFailureReason;
        public EResult Result;
    }
}