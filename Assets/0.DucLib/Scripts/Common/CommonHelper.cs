using System;
using System.Linq;
using System.Text.RegularExpressions;
using _0.OW.Scripts.Data;
using DG.Tweening;
using UnityEngine;
using Object = System.Object;

namespace _0.DucLib.Scripts.Common
{
    public static class CommonHelper
    {
        public static string ColorToHex(Color color)
        {
            return $"#{ColorUtility.ToHtmlStringRGBA(color)}";
        }
        public static Color HexToColor(string hex)
        {
            Color color;
            if (ColorUtility.TryParseHtmlString(hex, out color))
            {
                return color;
            }
            else
            {
                return Color.black; 
            }
        }
        #region Number

        private static readonly string[] suffixes = new string[]
        {
            "",  // Giá trị nhỏ hơn 1000
            "k", // Hàng nghìn
            "M", // Hàng triệu
            "B", // Hàng tỷ
            "T", // Hàng ngàn tỷ
            "P", // Hàng triệu tỷ
            "E"  // Hàng tỷ tỷ
        };

        /// <summary>
        /// Hàm chuyển đổi số sang dạng rút gọn.
        /// </summary>
        /// <param name="number">Số cần chuyển đổi.</param>
        /// <returns>Chuỗi số dạng rút gọn.</returns>
        public static string AbbreviateNumber(double number)
        {
            if (number < 1000)
            {
                // Nếu nhỏ hơn 1000, giữ nguyên
                return number.ToString("N0");
            }

            int suffixIndex = 0;

            // Rút gọn số thành dạng k, M, B...
            while (number >= 1000 && suffixIndex < suffixes.Length - 1)
            {
                number /= 1000f;
                suffixIndex++;
            }

            // Nếu là bội số của 1000, không cần hiển thị phần thập phân
            if (number % 1 == 0)
            {
                return $"{(int)number}{suffixes[suffixIndex]}";
            }

            // Hiển thị phần thập phân cho số không phải bội số của 1000
            return string.Format("{0:F1}{1}", number, suffixes[suffixIndex]);
        }
        /// <summary>
        /// chuyển đổi dạng string suffixes rút gọn sang số : 123aa -> 
        /// </summary>
        /// <param name="abbreviatedNumber"></param>
        /// <returns></returns>
        public static double ExpandAbbreviatedNumber(string abbreviatedNumber)
        {
            // Kiểm tra giá trị đầu vào
            if (string.IsNullOrEmpty(abbreviatedNumber))
            {
                throw new ArgumentException("Input cannot be null or empty.");
            }

            // Lấy phần hậu tố
            string suffix = string.Empty;
            for (int i = abbreviatedNumber.Length - 1; i >= 0; i--)
            {
                if (char.IsLetter(abbreviatedNumber[i]))
                {
                    suffix = abbreviatedNumber[i] + suffix;
                }
                else
                {
                    break;
                }
            }

            // Lấy phần số
            string numberPart = abbreviatedNumber.Substring(0, abbreviatedNumber.Length - suffix.Length);

            // Chuyển đổi phần số
            if (!double.TryParse(numberPart, out double number))
            {
                throw new FormatException($"Invalid number format: {numberPart}");
            }

            // Xác định chỉ số của hậu tố trong danh sách suffixes
            int suffixIndex = Array.IndexOf(suffixes, suffix);

            if (suffixIndex < 0)
            {
                throw new ArgumentException($"Invalid suffix: {suffix}");
            }

            // Nhân số với 1000 lũy thừa theo chỉ số hậu tố
            return number * Math.Pow(1000, suffixIndex);
        }

        #endregion

        #region Object

        public static void ActiveObject(this Object obj, bool active)
        {
            if (obj is GameObject gameObject)
            {
                gameObject.SetActive(active);
            }
            else if (obj is Component component)
            {
                component.gameObject.SetActive(active);
            }
        }
        public static void ShowObject(this Object obj)
        {
            if (obj is GameObject gameObject)
            {
                gameObject.SetActive(true);
            }
            else if (obj is Component component)
            {
                component.gameObject.SetActive(true);
            }
        }

        public static void HideObject(this Object obj)
        {
            if (obj is GameObject gameObject)
            {
                gameObject.SetActive(false);
            }
            else if (obj is Component component)
            {
                component.gameObject.SetActive(false);
            }
        }
        public static void ShowPopup(this Transform obj)
        {
            obj.transform.localScale = Vector3.one * 0.5f;
            obj.DOScale(1, 0.3f).SetEase(Ease.OutBack).SetUpdate(true);
        }

        #endregion
    }
}