using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BG_Library.DEBUG
{
    public class DebugFullScreenAdsManager : MonoBehaviour
    {
        public enum FORMAT
        {
            MAXMEDIATION,
            ADMOB
        }

        [SerializeField] private Text sourceText;
        [SerializeField] private Text revText;
        [SerializeField] private Text infoText;
        [SerializeField] private Text impressionText;
        [SerializeField] private Text debugText;
        [SerializeField] private DebugUnit1[] unit;

		private int impression;
        private double totalRev;

        public void Init()
        {
            for (var i = 0; i < 2; i++)
            {
                unit[i].SetDefault();
            }

            sourceText.text = "----";
            revText.text = "Total revenue: 0$";
            infoText.text = "----";
            impressionText.text = "----";
        }

        public void TimeOut(FORMAT format, string sub)
        {
            var order = (int)format;
            unit[order].subText.text = sub;
        }

        public void Load(FORMAT format, string sub = null)
        {
            var order = (int)format;
            unit[order].statusText.text = ADS_STATUS.LOADING.ToString();
            unit[order].subText.text = sub;
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

        public void DisplayFail(FORMAT format, string sub, string info)
        {
            var order = (int)format;
            unit[order].statusText.text = ADS_STATUS.DISPLAYFAIL.ToString();
            unit[order].subText.text = sub;
            unit[order].infoText.text = info;
        }

        public void Dismissed(FORMAT format, string sub, string info)
        {
            var order = (int)format;
            unit[order].statusText.text = ADS_STATUS.DISPLAYFAIL.ToString();
            unit[order].subText.text = sub;
            unit[order].infoText.text = info;
        }

        public void UpdateRev(FORMAT format, double rev, string currencyCode)
        {
            var order = (int)format;
            
            sourceText.text = format.ToString();
            infoText.text = unit[order].info;
            
            impression++;
            impressionText.text = $"Paid rev count: {impression}";
            
            unit[order].statusText.text = ADS_STATUS.DISPLAYED.ToString();
            unit[order].subText.text = unit[order].info;
            unit[order].RecordImpression();
            
            totalRev += rev;
            revText.text = $"Total revenue: {totalRev}{currencyCode}";

            unit[order].UpdateRev(rev);
        }

        public void UpdateDebugText(string st)
        {
            debugText.text = st;
        }
    }
}