# Setup IAP
 1. Create Product Data: Create > IAPManager > Create > IAP product stats
 2. Add Prefab Manager To Splash Scene: <folder lib>/Prefab/IAPManager
 3. Add Prodcut data to List IAP in IAPManager
# Use C#
## IAPManager.cs
1. PurchaseProduct(productId) - Call buy product
```
  IAPManager.PurchaseProduct(<ProdcutStats>.Id)
```
2. PurchaseResultListener {Call Back} - return buy result
```
    public class ShopManager : Monobehaviour
    {
        // Listener Event
        void Awake()
        {
            IAPManager.PurchaseResultListener += OnPurchaseComplete
        }
	
	    // UnListener Event
        void OnDestroy()
        {
            IAPManager.PurchaseResultListener -= OnPurchaseComplete
        }
        
        
        private void OnPurchaseComplete(IAPPurchaseResult iappurchaseresult)
        {
            switch (iappurchaseresult.Result)
            {
                case IAPPurchaseResult.EResult.Complete:
                    // iappurchaseresult.Product.Reward - Reward setup in stats
                    // iappurchaseresult.Product.Reward.PackRewardValue - give reward amount
                    // iappurchaseresult.Product.Reward.Reward - Type Reward > REMOVE_AD, CURRENCY (CASH OR GOLD), CUSTOM (Item Or Tool)
                    // iappurchaseresult.Product.Reward.atlas - Reward give Currency Id or Item, Tool Id (example: CASH, GOLD, TOOL_1...)
                    // todo give product reward
                    break;
                case IAPPurchaseResult.EResult.WrongInstance:
                    // Purchase faield: IAP Manager instance null (Read Setup IAP)  
                    break;
                case IAPPurchaseResult.EResult.WrongProduct:
                    // Purchase faield: can't find product with id 
                    break;
                case IAPPurchaseResult.EResult.WrongStoreController:
                    // Purchase faield: IAP initialized faield
                    break;
            }
        }
    }
```
3. IAPProductPrice.cs - Auto set Text to cost amount iap product
4. IAPStatusUpdate.cs - Auto update status IAP (<b>NEED ADD TO BUTTON BUY</b>)
