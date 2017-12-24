using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Passaword.Utils
{
    public static class HttpRequestExtensions
    {
        public static string GetIpAddress(this HttpRequest request, bool tryUseXForwardHeader = true)
        {
            string ip = null;

            if (tryUseXForwardHeader)
                ip = request.GetHeaderValueAs<string>("X-Forwarded-For").SplitCsv().FirstOrDefault();

            if (ip.IsNullOrWhitespace() && request.HttpContext?.Connection?.RemoteIpAddress != null)
                ip = request.HttpContext.Connection.RemoteIpAddress.ToString();

            if (ip.IsNullOrWhitespace())
                ip = request.GetHeaderValueAs<string>("REMOTE_ADDR");

            if (ip.IsNullOrWhitespace())
                return null;

            return ip;
        }

        private static T GetHeaderValueAs<T>(this HttpRequest request, string headerName)
        {
            StringValues values;

            if (request?.Headers?.TryGetValue(headerName, out values) ?? false)
            {
                string rawValues = values.ToString();   // writes out as Csv when there are multiple.

                if (!rawValues.IsNullOrWhitespace())
                    return (T)Convert.ChangeType(values.ToString(), typeof(T));
            }
            return default(T);
        }

        public static List<string> SplitCsv(this string csvList, bool nullOrWhitespaceInputReturnsNull = false)
        {
            if (string.IsNullOrWhiteSpace(csvList))
                return nullOrWhitespaceInputReturnsNull ? null : new List<string>();

            return csvList
                .TrimEnd(',')
                .Split(',')
                .AsEnumerable<string>()
                .Select(s => s.Trim())
                .ToList();
        }

        public static bool IsNullOrWhitespace(this string s)
        {
            return String.IsNullOrWhiteSpace(s);
        }
    }
}
