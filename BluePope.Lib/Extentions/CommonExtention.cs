using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace BluePope
{
    public static class CommonExtention
    {
        #region string to number
        public static int ToInt(this string str)
        {
            int.TryParse(str.Replace(",", ""), out int r);
            return r;
        }
        public static int? ToIntOrNull(this string str)
        {
            if (int.TryParse(str.Replace(",", ""), out int r))
                return r;
            else
                return null;
        }

        public static long ToLong(this string str)
        {
            long.TryParse(str.Replace(",", ""), out long r);
            return r;
        }
        public static long? ToLongOrNull(this string str)
        {
            if (long.TryParse(str.Replace(",", ""), out long r))
                return r;
            else
                return null;
        }

        public static decimal ToDecimal(this string str)
        {
            decimal.TryParse(str.Replace(",", ""), out decimal r);
            return r;
        }

        public static decimal? ToDecimalOrNull(this string str)
        {
            if (decimal.TryParse(str.Replace(",", ""), out decimal r))
                return r;
            else
                return null;
        }

        #endregion

        #region convert

        /// <summary>
        /// 문자열을 yyyyMMdd 날짜문자형태로
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToDbDateString(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return null;

            var sb = new StringBuilder(str);
            sb.Replace("-", "");
            sb.Replace("/", "");
            sb.Replace(".", "");

            if (DateTime.TryParseExact(sb.ToString(), "yyyyMMdd", CultureInfo.CurrentCulture, DateTimeStyles.None, out _))
                return sb.ToString();

            return null;
        }

        /// <summary>
        /// 달력 문자열을 정리후 yyyy-MM-dd 형태로
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToDateString(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return null;

            var sb = new StringBuilder(str);
            sb.Replace("-", "");
            sb.Replace("/", "");
            sb.Replace(".", "");

            DateTime d;

            if (DateTime.TryParseExact(sb.ToString(), "yyyyMMdd", CultureInfo.CurrentCulture, DateTimeStyles.None, out d))
                return d.ToString("yyyy-MM-dd");

            return null;
        }

        /// <summary>
        /// DateTime?안의 Value 확인 후 ToString
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToString(this DateTime? datetime, string format) => datetime.HasValue ? datetime.Value.ToString(format) : string.Empty;
        #endregion

        #region DateTime 관련 함수
        /// <summary>
        /// 문자열을 DateTime? 형태로
        /// </summary>
        /// <param name="str"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static DateTime? ToDateTime(this string str, string format = null)
        {
            if (str.IsNull())
                return null;

            if (format.IsNull())
            {
                if (str.Length == 8)
                    format = "yyyyMMdd";
                else
                    format = "yyyy-MM-dd";
            }

            return DateTime.ParseExact(str, format, System.Globalization.CultureInfo.CurrentCulture);
        }
        public static DateTime FirstDay(this DateTime date) => date.AddDays(-(date.Day - 1));
        public static DateTime LastDay(this DateTime date) => date.AddDays(-(date.Day - 1)).AddMonths(1).AddDays(-1);
        #endregion

        #region DB style
        public static bool IsNull(this string str) => string.IsNullOrWhiteSpace(str);
        public static string IsNull(this string str, string alter)
        {
            if (string.IsNullOrWhiteSpace(str))
                return alter;
            else
                return str;
        }

        public static bool In(this string str, params string[] param) => param != null ? param.Contains(str) : false;
        public static bool In(this int num, params int[] param) => param != null ? param.Contains(num) : false;
        public static bool In(this decimal num, params decimal[] param) => param != null ? param.Contains(num) : false;

        public static bool Between(this int num, int min, int max) => (min <= num && num <= max);
        public static bool Between(this long num, long min, long max) => (min <= num && num <= max);
        public static bool Between(this decimal num, decimal min, decimal max) => (min <= num && num <= max);
        public static bool Between(this float num, float min, float max) => (min <= num && num <= max);
        public static bool Between(this double num, double min, double max) => (min <= num && num <= max);
        #endregion
    }
}