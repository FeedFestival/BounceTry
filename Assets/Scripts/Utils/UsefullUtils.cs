using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public static class UsefullUtils
    {
        public static float GetPercent(float value, float percent)
        {
            return (value / 100f) * percent;
        }

        public static float GetValuePercent(float value, float maxValue)
        {
            return (value * 100f) / maxValue;
        }

        public static string GetDataValue(string data, string index)
        {
            string value = data.Substring(data.IndexOf(index, StringComparison.Ordinal) + index.Length);
            if (value.Contains("|"))
                value = value.Remove(value.IndexOf('|'));
            return value;
        }
        public static int GetIntDataValue(string data, string index)
        {
            int numb;
            var success = int.TryParse(GetDataValue(data, index), out numb);

            return success ? numb : 0;
        }
        public static bool GetBoolDataValue(string data, string index)
        {
            var value = GetDataValue(data, index);
            if (string.IsNullOrEmpty(value) || value.Equals("0"))
                return false;
            return true;
        }
        public static long GetLongDataValue(string data, string index)
        {
            long numb;
            var success = long.TryParse(GetDataValue(data, index), out numb);

            return success ? numb : 0;
        }

        public static long GetLongDataValue(string data)
        {
            long numb;
            var success = long.TryParse(data, out numb);

            return success ? numb : 0;
        }

        public static Color white;
        public static Color placeholderTextColor;  // fadeGrey
        public static Color textColor;  // grey
        public static Color black;
        public static Color importantText;

        public static void InitColors()
        {
            ColorUtility.TryParseHtmlString("#E6E6E6FF", out white);
            ColorUtility.TryParseHtmlString("#18191AFF", out black);
            ColorUtility.TryParseHtmlString("#FF3232", out importantText);
            ColorUtility.TryParseHtmlString("#909090FF", out textColor); //AAAAAAFF
            ColorUtility.TryParseHtmlString("#323232FF", out placeholderTextColor);
        }
    }

    public enum NavbarButton
    {
        HomeButton,
        FriendsButton,
        HistoryButton
    }
}