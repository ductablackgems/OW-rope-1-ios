using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace BG_Library.DEBUG
{
    /// <summary>
    /// Banner
    /// </summary>
    public class DebugUnit0 : MonoBehaviour
    {
        public Text statusText;
        public Text subText;
        public Text infoText;
        public Text totalRevText;
        public Toggle checkToggle;

        public string info;
        public double totalValue;
        public int impression;
        
        public virtual void SetDefault()
        {
            statusText.text = "----";
            subText.text = "----";
            infoText.text = "----";
            totalRevText.text = "----";
            if (checkToggle != null)
            {
                checkToggle.isOn = false;
            }

            totalValue = 0;
            impression = 0;
        }
    }
}