using System.Net;

namespace IdorAnalizerCSharp
{
    internal static class HttpUtility
    {
        public static NameValueCollection ParseQueryString(string query)
        {
            if (string.IsNullOrEmpty(query))
                return new NameValueCollection();

            if (query.StartsWith("?"))
                query = query.Substring(1);

            var result = new NameValueCollection();
            var pairs = query.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var pair in pairs)
            {
                if (string.IsNullOrEmpty(pair)) continue;

                var parts = pair.Split(new[] { '=' }, 2);
                if (parts.Length == 0) continue;

                string key = parts[0];
                string value = parts.Length > 1 ? WebUtility.UrlDecode(parts[1]) : string.Empty;

                result[WebUtility.UrlDecode(key)] = value;
            }

            return result;
        }
    }
}
