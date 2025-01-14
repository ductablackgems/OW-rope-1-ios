using System;
using TMPro;
using UnityEngine;

namespace BG_Library.IAP
{
    [RequireComponent(typeof(TMP_Text))]
    public class IAPProductPrice : MonoBehaviour
    {
        [SerializeField] private TMP_Text _priceText;
        [SerializeField] private IAPProductStats _iapProductStats;

        private void Start()
        {
            _priceText.text = "-";
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
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_priceText == null)
                _priceText = GetComponent<TMP_Text>();
        }
#endif
    }
}