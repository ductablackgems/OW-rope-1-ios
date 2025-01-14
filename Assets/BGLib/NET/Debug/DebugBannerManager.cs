using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace  BG_Library.DEBUG
{
    public class DebugBannerManager : MonoBehaviour
    {
        public enum FORMAT
        {
            BN_MAX,
            NA_ADMOB,
            CO_ADMOB
        }

        [SerializeField] private Text sourceText;
        [SerializeField] private Text impressionText;
        [SerializeField] private Text revText;
        [SerializeField] private Text infoText;
        [SerializeField] private DebugUnit0[] unit;

        private double totalRev;
        private int impression;
        
        public void Init()
        {
            for (var i = 0; i < 3; i++)
            {
                unit[i].SetDefault();
            }

            sourceText.text = "----";
            revText.text = "Total revenue: 0$";
            infoText.text = "----";
        }

        public void Load(FORMAT format, string sub = null, string info = null)
        {
            var order = (int)format;
            unit[order].statusText.text = ADS_STATUS.LOADING.ToString();
            unit[order].subText.text = sub;
            unit[order].infoText.text = info;
        }

        public void Loaded(FORMAT format, string unitInfo, string info)
        {
            var order = (int)format;
            unit[order].statusText.text = ADS_STATUS.LOADED.ToString();
            unit[order].info = unitInfo;
            
            unit[order].subText.text = unitInfo;
            unit[order].infoText.text = info;
        }

        public void LoadFail(FORMAT format, string sub, string info)
        {
            var order = (int)format;
            unit[order].statusText.text = ADS_STATUS.LOADFAIL.ToString();
            unit[order].subText.text = sub;
            unit[order].infoText.text = info;
        }
        
        public void Show(FORMAT format, string sub = null)
        {
            var order = (int)format;
            sourceText.text = format.ToString();
            infoText.text = unit[order].info;
            
            unit[order].statusText.text = ADS_STATUS.SHOWING.ToString();
            unit[order].subText.text = unit[order].info;
            unit[order].infoText.text = sub;
        }

        public void Hide(FORMAT format)
        {
            var order = (int)format;
            unit[order].statusText.text = ADS_STATUS.HIDE.ToString();
            unit[order].subText.text = "";
            unit[order].infoText.text = "";
        }

        public void UpdateRev(FORMAT format, double rev, string currencyCode)
        {
            var order = (int)format;
            totalRev += rev;
            revText.text = $"Total revenue: {totalRev}$";
            
            impression++;
            impressionText.text = $"Paid rev count: {impression}";

            unit[order].totalValue += rev;
            unit[order].impression++;
            unit[order].totalRevText.text = 
                $"Total revenue: {unit[order].totalValue}{currencyCode} | Count: {unit[order].impression}";
        }

        public void SetCheckToggle(FORMAT format, bool isOn)
        {
            var order = (int)format;
            unit[order].checkToggle.isOn = isOn;
        }
    }
}