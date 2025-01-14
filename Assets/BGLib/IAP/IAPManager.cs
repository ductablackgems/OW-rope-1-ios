using System;
using System.Globalization;
using System.Linq;
using BG_Library.NET;
using Firebase.Analytics;
using UnityEngine;
using UnityEngine.Purchasing;

namespace BG_Library.IAP
{
    public partial class IAPManager : MonoBehaviour, IStoreListener
    {
        [SerializeField] private IAPProductStats[] _iapProductStats;
        
        private IStoreController _implStoreController;
        private IExtensionProvider _implExtensionProvider;

        private string _cachePosition;
        private static Action _onInitialized;
        private static IAPManager _iapManager;
        
        public static bool IsInitialized => _iapManager?._implStoreController != null && _iapManager?._implExtensionProvider != null;
        public delegate void CallBackPurchaseResult(IAPPurchaseResult iapPurchaseResult);
        public static CallBackPurchaseResult PurchaseResultListener;
        public static Action PurchaseProductListener;

        public static bool IsEnableDebugIap = false;
        
        #region Func
        private void Awake()
        {
            _iapManager = this;
            ConfigurationBuilder configurationBuilder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            for (var i = 0; i < _iapProductStats.Length; i++)
            {
                IAPProductStats iapProductStats = _iapProductStats[i];
                configurationBuilder.AddProduct(iapProductStats.Id, iapProductStats.ProductType);
            }
            UnityPurchasing.Initialize(this, configurationBuilder);
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Call buy position
        /// </summary>
        /// <param name="position">position buy</param>
        /// <param name="productId">product id</param>
        public static void PurchaseProduct(string position, string productId)
        {
            _iapManager._cachePosition = position;
            if (_iapManager == null)
            {
                RaisedPurchaseResult(new IAPPurchaseResult
                {
                    Position = _iapManager._cachePosition,
                    ProductId = productId,
                    Result = IAPPurchaseResult.EResult.WrongInstance
                });
            }
            else if (!IsInitialized)
            {
                RaisedPurchaseResult(new IAPPurchaseResult
                {
                    Position = _iapManager._cachePosition,
                    ProductId = productId,
                    Result = IAPPurchaseResult.EResult.WrongStoreController
                });
            }
            else if (IsEnableDebugIap)
            {
                IAPProductStats iapProductStats = _iapManager._iapProductStats.ToList().Find(iapProduct => iapProduct.Id.Equals(productId));
                Product product = _iapManager._implStoreController.products.WithID(productId);
                RaisedPurchaseResult(new IAPPurchaseResult
                {
                    Position = _iapManager._cachePosition,
                    Product = iapProductStats,
                    ProductId = productId,
                    PurchasedProduct = product,
                    Result = IAPPurchaseResult.EResult.Complete
                }, false);
            }
            else
            {
                IAPProductStats iapProductStats = _iapManager._iapProductStats.ToList().Find(iapProduct => iapProduct.Id.Equals(productId));
                Product product = _iapManager._implStoreController.products.WithID(productId);
                _iapManager._implStoreController.InitiatePurchase(product);
                PurchaseProductListener?.Invoke();
                
                AdsManager.OpenAdAction = EOAAction.IAP;
                FirebaseEvent.LogEvent($"IAP_click", 
                    new Parameter("location", position),
                    new Parameter("package_id", iapProductStats.TrackingId),
                    new Parameter("value", (float)product.metadata.localizedPrice),
                    new Parameter("currency", product.metadata.isoCurrencyCode));
            }
        }

        private static void RaisedPurchaseResult(IAPPurchaseResult iapPurchaseResult, bool isTracking = true)
        {
            if (isTracking)
            {
                if (iapPurchaseResult.Result == IAPPurchaseResult.EResult.Complete)
                {
                    FirebaseEvent.LogEvent($"IAP_success", 
                        new Parameter("location", iapPurchaseResult.Position),
                        new Parameter("package_id", iapPurchaseResult.Product.TrackingId),
                        new Parameter("value", (float)iapPurchaseResult.PurchasedProduct.metadata.localizedPrice),
                        new Parameter("currency", iapPurchaseResult.PurchasedProduct.metadata.isoCurrencyCode));
                    
                    /*AdjustWrapper.TrackIAPRevenue(
                        (float)iapPurchaseResult.PurchasedProduct.metadata.localizedPrice,
                        iapPurchaseResult.PurchasedProduct.metadata.isoCurrencyCode);*/
                    AdjustSdk.AdjustEvent iAPAdjustEvent = new AdjustSdk.AdjustEvent(NetConfigsSO.Ins.AdjustEventIAP);
                    iAPAdjustEvent.SetRevenue((float)iapPurchaseResult.PurchasedProduct.metadata.localizedPrice
                        , iapPurchaseResult.PurchasedProduct.metadata.isoCurrencyCode);
                    AdjustSdk.Adjust.TrackEvent(iAPAdjustEvent);
                }
                else if (iapPurchaseResult.Result == IAPPurchaseResult.EResult.PurchaseFailed)
                {
                    FirebaseEvent.LogEvent($"IAP_failed", 
                        new Parameter("location", iapPurchaseResult.Position),
                        new Parameter("package_id", iapPurchaseResult.Product.TrackingId),
                        new Parameter("value", (float)iapPurchaseResult.PurchasedProduct.metadata.localizedPrice),
                        new Parameter("error_name", iapPurchaseResult.PurchaseFailureReason.ToString()),
                        new Parameter("currency", iapPurchaseResult.PurchasedProduct.metadata.isoCurrencyCode));
                }
                else
                {
                    FirebaseEvent.LogEvent($"IAP_failed", 
                        new Parameter("location", iapPurchaseResult.Position),
                        new Parameter("error_name", iapPurchaseResult.Result.ToString()));
                }
            }
            if(iapPurchaseResult.Result == IAPPurchaseResult.EResult.Complete) 
                SavePurchaseProduct(iapPurchaseResult.Product.Id);
            PurchaseResultListener?.Invoke(iapPurchaseResult);
            Debug.Log($"IAP call buy product {iapPurchaseResult.ProductId} result: {iapPurchaseResult.Result}");
        }
        
        public static void CallRestore()
        {
            if(Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.OSXPlayer) return;
            _iapManager._implExtensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions(OnRestore);
        }

        private static void OnRestore(bool restoreResult)
        {
            Debug.Log($"IAP call restore result: {restoreResult}");
        }
        
        public static string GetPriceString(string productId)
        {
            if (_iapManager == null)
                return null;
            
            if (!IsInitialized)
                return null;
            
            Product product = _iapManager._implStoreController.products.WithID(productId);
            if (product != null)
                    return product.metadata.localizedPriceString;
            return null;
        }
        
        public static decimal GetPrice(string productId)
        {
            if (_iapManager == null)
                return 0;
            
            if (!IsInitialized)
                return 0;
            
            Product product = _iapManager._implStoreController.products.WithID(productId);
            if (product != null)
                return product.metadata.localizedPrice;
            return 0;
        }
        
        public static string GetCultureInfoFromISOCurrencyCode(string productId)
        {
            Product product = _iapManager._implStoreController.products.WithID(productId);
            foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                RegionInfo ri = new RegionInfo(ci.LCID);
                if (ri.ISOCurrencySymbol == product.metadata.isoCurrencyCode)
                    return ri.CurrencySymbol;
            }
            return null;
        }

        public static void CheckInitializedAndHandle(Action onInitializeComplete)
        {
            if (_iapManager == null || !IsInitialized)
                _onInitialized += onInitializeComplete;
            else
                onInitializeComplete?.Invoke();
        }
        #endregion

        #region Listener Store
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            string purchaseId = purchaseEvent.purchasedProduct.definition.id;
            IAPProductStats iapProductStats = _iapProductStats.ToList().Find(iapProduct => iapProduct.Id.Equals(purchaseId));

            if (iapProductStats != null)
            {
                RaisedPurchaseResult(new IAPPurchaseResult
                {
                    Position = _iapManager._cachePosition,
                    Product = iapProductStats,
                    ProductId = purchaseId,
                    PurchasedProduct = purchaseEvent.purchasedProduct,
                    Result = IAPPurchaseResult.EResult.Complete
                });
                return PurchaseProcessingResult.Complete;
            }
            
            RaisedPurchaseResult(new IAPPurchaseResult
            {
                Position = _iapManager._cachePosition,
                Product = iapProductStats,
                ProductId = purchaseId,
                PurchasedProduct = purchaseEvent.purchasedProduct,
                Result = IAPPurchaseResult.EResult.WrongProduct
            });
            return PurchaseProcessingResult.Pending;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            string purchaseId = product.definition.id;
            IAPProductStats iapProductStats = _iapProductStats.ToList().Find(iapProduct => iapProduct.Id.Equals(purchaseId));
            RaisedPurchaseResult(new IAPPurchaseResult
            {
                Position = _iapManager._cachePosition,
                Product = iapProductStats,
                ProductId = purchaseId,
                PurchasedProduct = product,
                PurchaseFailureReason = failureReason,
                Result = IAPPurchaseResult.EResult.PurchaseFailed
            });
            
            Debug.LogWarning($"IAP purchase product {product.definition.storeSpecificId} failed: {failureReason}");
        }
        
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _implStoreController = controller;
            _implExtensionProvider = extensions;
            
#if UNITY_ANDROID
            extensions.GetExtension<IGooglePlayStoreExtensions>().RestoreTransactions(OnRestore);
#endif
            
            _onInitialized?.Invoke();
           Debug.Log($"IAP initialized complete {_iapProductStats.Length} products!");
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogError($"IAP initialized failed error: {error}");
        }
        
        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.LogError($"IAP initialized failed error: {error}, Message: {message}");
        }
        #endregion
    }
}