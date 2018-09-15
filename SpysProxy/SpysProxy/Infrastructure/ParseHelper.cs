using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Noesis.Javascript;
using NPOI.Util;

namespace SpysProxy.Infrastructure
{
    internal static class ParseHelper
    {
        private static Dictionary<char, string> _dictionary;
        
        public static string DoJs(string algorithm, string portCrypt)
        {
            using (var context = new JavascriptContext())
            {
                context.SetParameter("answer", "");
                var script = $@"{algorithm};
                                answer += '' + {portCrypt}";
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

        public static string DoWithOutJs(string algorithm, string portCrypt)
        {
            var i = 88;
            var keysString = algorithm.Substring(algorithm.IndexOf(@"}('", StringComparison.Ordinal) + 3,
                algorithm.IndexOf(@"',6", StringComparison.Ordinal) - 3 -
                algorithm.IndexOf(@"}('", StringComparison.Ordinal));
            var valuesString = algorithm.Substring(algorithm.IndexOf(@"0,'", StringComparison.Ordinal) + 3,
                algorithm.IndexOf(@"'.s", StringComparison.Ordinal) - 3 -
                algorithm.IndexOf(@"0,'", StringComparison.Ordinal)).Split('^').Reverse();
            _dictionary = valuesString.ToDictionary(val => (i > 64 ? (char) i-- : i > 38 ? (char) (58 + i--) : (char) (19 + i--)));
            var regex = new Regex(@"\w");
            var ans = regex.Replace(keysString, F);
            using (var context = new JavascriptContext())
            {
                context.SetParameter("answer", "");
                var script = $@"{ans};
                                answer += '' + {portCrypt}";
                context.Run(script);
                return context.GetParameter("answer").ToString();
            }
        }

        private static string F(Match match)
        {
            var tmp = _dictionary.Where(e => e.Key.ToString() == match.Value).Select(e => e.Value).FirstOrDefault();
            return tmp == string.Empty ? match.Value : tmp;
        }
    }
}
