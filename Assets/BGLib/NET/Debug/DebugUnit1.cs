using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BG_Library.DEBUG
{
    /// <summary>
    /// Max FA
    /// </summary>
    public class DebugUnit1 : DebugUnit0
    {
        public Text impressionText;

        private double totalRev;

        public override void SetDefault()
        {
            base.SetDefault();
            impressionText.text = "----";
        }

        public void RecordImpression()
        {
            impression++;
            impressionText.text = $"Paid rev count: {impression}";
        }

        public void UpdateRev(double rev)
        {
            totalRev += rev;
            totalRevText.text = $"Total revenue: {totalRev}$";
        }
    }
}