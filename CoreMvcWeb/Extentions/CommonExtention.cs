using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace CoreMvcWeb.Extentions
{
    public static class CommonExtention
    {
        public static List<T> AddRangeNotNull<T>(this List<T> list, params T[] strList)
        {
            foreach (var str in strList)
            {
                if (str != null)
                {
                    if (str.GetType() == typeof(string))
                    {
                        if (string.IsNullOrWhiteSpace(str.ToString()) == false)
                        {
                            list.Add(str);
                        }
                    }
                    else
                    {
                        list.Add(str);
                    }
                }
            }

            return list;
        }

        public static int GetByteLength(this string str)
        {
            return Encoding.Default.GetByteCount(str);
        }

        public static DateTime? ToDateTime(this string str, string format = null)
        {
            if (string.IsNullOrWhiteSpace(str))
                return null;

            if (string.IsNullOrWhiteSpace(format))
            {
                if (str.Length == 8)
                    format = "yyyyMMdd";
                else
                    format = "yyyy-MM-dd";
            }

            return DateTime.ParseExact(str, format, System.Globalization.CultureInfo.CurrentCulture);
        }

        public static DateTime LastDay(this DateTime date)
        {
            return date.ToString("yyyy-MM-01").ToDateTime().Value.AddMonths(1).AddDays(-1);
        }

        public static string ToString(this DateTime? datetime, string format)
        {
            if (datetime.HasValue)
            {
                return datetime.Value.ToString(format);
            }
            return string.Empty;
        }

        public static string GetDateTimeValue(this HtmlHelper htmlHelper, DateTime? datetime, string dateFormat)
        {
            if (datetime.HasValue)
            {
                return datetime.Value.ToString(dateFormat);
            }
            else
            {
                return string.Empty;
            }
        }

        public static bool In(this string str, params string[] param)
        {
            if (param == null || param.Length == 0)
            {
                return false;
            }
            else
            {
                return param.Contains(str);
            }
        }

        public static bool In(this int num, params int[] param)
        {
            if (param == null || param.Length == 0)
            {
                return false;
            }
            else
            {
                return param.Contains(num);
            }
        }

        public static string ToDateFormat8(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            return str.Replace("-", "").Replace("/", "").Replace(".", "");
        }

        public static bool IsNull(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return true;
            else
                return false;
        }
        public static string IsNull(this string str, string b)
        {
            if (string.IsNullOrWhiteSpace(str))
                return b;
            else
                return str;
        }

        public static void CopyPropertiesTo<T, TU>(this T source, TU dest)
        {
            var sourceProps = typeof(T).GetProperties().Where(x => x.CanRead).ToList();
            var destProps = typeof(TU).GetProperties()
                    .Where(x => x.CanWrite)
                    .ToList();

            foreach (var sourceProp in sourceProps)
            {
                if (destProps.Any(x => x.Name == sourceProp.Name))
                {
                    var p = destProps.First(x => x.Name == sourceProp.Name);
                    if (p.CanWrite)
                    { // check if the property can be set or no.
                        p.SetValue(dest, sourceProp.GetValue(source, null), null);
                    }
                }

            }

        }

        public static int ToInt(this string str)
        {
            try
            {
                return Convert.ToInt32(str.Replace(",", ""));
            }
            catch
            {
                return 0;
            }
        }

        public static decimal ToDecimal(this string str)
        {
            try
            {
                return Convert.ToDecimal(str.Replace(",", ""));
            }
            catch
            {
                return 0;
            }
        }

        public static string GetValue(this List<KeyValuePair<string, string>> src, string key)
        {
            if (src.Count(p => p.Key == key) > 0)
                return src.First(p => p.Key == key).Value;

            return null;
        }
    }
}