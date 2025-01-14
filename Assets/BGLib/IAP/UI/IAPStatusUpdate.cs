using System;
using UnityEngine;

namespace BG_Library.IAP
{
    public class IAPStatusUpdate : MonoBehaviour
    {
        [SerializeField] private GameObject _iapReadyContent;
        [SerializeField] private GameObject _iapNotReadyContent;

        private void Awake()
        {
            ToggleContentLoading(true);
            
            IAPManager.PurchaseProductListener += OnPurchaseProduct;
            IAPManager.PurchaseResultListener += OnPurchaseComplete;
        }
        
        public void OnDestroy()
        {
            IAPManager.PurchaseProductListener -= OnPurchaseProduct;
            IAPManager.PurchaseResultListener -= OnPurchaseComplete;
        }

        private void OnEnable()
        {
            IAPManager.CheckInitializedAndHandle(OnInitializedComplete);
        }

        private void OnPurchaseProduct()
        {
            ToggleContentLoading(true);
        }

        private void OnPurchaseComplete(IAPPurchaseResult iappurchaseresult)
        {
            ToggleContentLoading(false);
        }

        private void OnInitializedComplete()
        {
            ToggleContentLoading(false);
        }

        private void ToggleContentLoading(bool isEnable)
        {
            _iapReadyContent.SetActive(!isEnable);
            _iapNotReadyContent.SetActive(isEnable);
        }
    }
}