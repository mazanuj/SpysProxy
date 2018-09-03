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
    }
}
