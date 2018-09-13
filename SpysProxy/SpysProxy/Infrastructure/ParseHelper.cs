using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Noesis.Javascript;

namespace SpysProxy.Infrastructure
{
    internal static class ParseHelper
    {
        public static string DoJs(string algorithm, string key)
        {
            using (var context = new JavascriptContext())
            {
                context.SetParameter("answer", "");
                var script = $@"{algorithm};
                                answer += '' + {key}";
                context.Run(script);
                return context.GetParameter("answer").ToString();
            }
        }

        public static string DoRegexIp(string ipAddress)
        {
            var regexPhone = new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}");
            var matches = regexPhone.Matches(ipAddress);
            if (matches.Count <= 0) return null;
            ipAddress = matches[0].Value;
            return ipAddress;
        }

        public static string DoWithOutJs(string algorithm)
        {
            var i = 88;

            var keysString = algorithm.Substring(algorithm.IndexOf(@"}('", StringComparison.Ordinal) + 3,
                algorithm.IndexOf(@"',6", StringComparison.Ordinal) - 3 -
                algorithm.IndexOf(@"}('", StringComparison.Ordinal));
            var valuesString = algorithm.Substring(algorithm.IndexOf(@"0,'", StringComparison.Ordinal) + 3,
                algorithm.IndexOf(@"'.s", StringComparison.Ordinal) - 3 -
                algorithm.IndexOf(@"0,'", StringComparison.Ordinal)).Split('^').Reverse();
            var dictionary = valuesString.ToDictionary(val => (i > 64 ? (char) i-- : i < 39 ? (char) (19 + i--) : (char) (58 + i--)));

            return "test";
        }
    }
}
