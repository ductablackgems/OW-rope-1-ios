using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BG_Library.IAP
{
    [RequireComponent(typeof(Text))]
    public class IAPProductPriceLegacy : MonoBehaviour
    {
        [SerializeField] private Text _priceText;
        [SerializeField] private IAPProductStats _iapProductStats;

        private void Start()
        {
            _priceText.text = "-";
            RecheckInit();
        }

        private void RecheckInit()
        {
            IAPManager.CheckInitializedAndHandle(OnInitializedComplete);
        }

        private void OnInitializedComplete()
        {
            decimal priceString = IAPManager.GetPrice(_iapProductStats.Id);
            string isoCode = IAPManager.GetCultureInfoFromISOCurrencyCode(_iapProductStats.Id);
            _priceText.text = $"{priceString} {isoCode}";
        }

        public void SetIAPProductStats(IAPProductStats iapProductStats)
        {
            _iapProductStats = iapProductStats;
            RecheckInit();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_priceText == null)
                _priceText = GetComponent<Text>();
        }
#endif
    }
}