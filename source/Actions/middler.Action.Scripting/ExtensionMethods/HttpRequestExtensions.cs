using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using Reflectensions.ExtensionMethods;

namespace middler.Action.Scripting.ExtensionMethods
{
    public static class HttpRequestExtensions
    {

        public static List<IPAddress> FindSourceIp(this HttpRequest httpRequest)
        {


            var sourceIps = new List<IPAddress>();


            if (httpRequest.Headers.ContainsKey("X-Forwarded-For"))
            {
                var ips = httpRequest.Headers["X-Forwarded-For"];
                sourceIps = ips.Select(IPAddress.Parse).ToList();
            }

            sourceIps.Add(httpRequest.HttpContext.Connection.RemoteIpAddress);


            return sourceIps;
        }

        public static Dictionary<string, string> GetHeaders(this HttpRequest request) {

            var requestHeaders = new Dictionary<string, string>();
            foreach (var kvp in request.Headers) {

                if (kvp.Key.Equals("$type", StringComparison.OrdinalIgnoreCase)) {

                } else {
                    requestHeaders[kvp.Key] = String.Join(", ", kvp.Value);
                }
            }
            return requestHeaders;

        }

        public static Dictionary<string, string> GetQueryParameters(this HttpRequest request) {

            var dictionary = new Dictionary<string, string>();

            foreach (var pair in request.Query) {
                //if (pair.Key.EndsWith("_header", StringComparison.CurrentCultureIgnoreCase))
                //    continue;

                //if (pair.Key.Equals("rawresponse", StringComparison.CurrentCultureIgnoreCase))
                //    continue;

                //if (pair.Key.Equals("formatresponse", StringComparison.CurrentCultureIgnoreCase))
                //    continue;

                if (String.IsNullOrWhiteSpace(pair.Value)) {
                    dictionary.Add(pair.Key, "true");
                    continue;
                }
                var value = GetJoinedValue(pair.Value);
                if (value == "")
                    value = null;

                if (value == "\"\"")
                    value = "";

                dictionary.Add(pair.Key, value);
            }

            return dictionary;

        }

        private static string GetJoinedValue(string[] value) {
            if (value != null)
                return string.Join(",", value.Select(v => v.ToNull() ?? "true"));

            return null;
        }




    }
}


