using System;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public static class CoreNumberToWordsService
    {
        static string ResolveNumber(string number)
        {

            string[] a = { "", "One ", "Two ", "Three ", "Four ", "Five ", "Six ", "Seven ", "Eight ", "Nine ", "Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ", "Fifteen ", "Sixteen ", "Seventeen ", "Eighteen ", "Nineteen " };
            string[] b = { "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

            if (number.Length > 3)
                throw new Exception("Invalid numeric value");

            string _number = string.Format("{0}{1}", "000".Substring(0, 3 - number.Length), number);

            int[] values = { int.Parse(_number.Substring(0, 1)), int.Parse(_number.Substring(1, 1)), int.Parse(_number.Substring(2, 1)) };
            int value2 = int.Parse(_number.Substring(1, 2));

            string result = "";

            result += values[0] > 0 ? (a[values[0]] + " Hundred" + (value2 > 0 ? " and " : " ")) : " ";
            result += values[1] > 0 ? (value2 < 20 ? a[value2] : b[values[1]] + " ") : "";
            result += values[2] > 0 ? (value2 > 20 ? a[values[2]] : (value2 == values[2] ? a[value2] : "")) : "";

            return result;

        }

        public static string AmountInWords(decimal number)
        {
            string[] c = { " ", "Thousand ", "Million ", "Billion ", "Trillion " };

            string mainPart = "";
            string subPart = "";

            if (number > 0)
            {
                string value = number.ToString("0,0.00");

                var parts = value.Split('.');

                string[] mainParts = parts[0].Split(',');

                int _pos = 0;
                for (int pos = mainParts.Length - 1; pos >= 0; pos--)
                {
                    if (Convert.ToInt32(mainParts[pos]) > 0)
                    {
                        mainPart = ResolveNumber(mainParts[pos]) + c[_pos] + mainPart;
                    }
                    _pos++;
                }

                if (int.Parse(parts[1]) > 0)
                    subPart = ResolveNumber(parts[1]);
            }

            string result = (mainPart.Length > 0 ? string.Format("{0}Naira", mainPart) : "") + " " + (subPart != "" ? string.Format("{0}Kobo", subPart) : "") + " Only";

            return result;

        }
    }
}