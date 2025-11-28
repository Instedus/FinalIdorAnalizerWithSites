using System.Net;
using System.Text;

namespace IdorAnalizerCSharp
{
    public class NameValueCollection
    {
        private readonly Dictionary<string, string> _collection = new Dictionary<string, string>();

        public NameValueCollection() { }

        public NameValueCollection(NameValueCollection collection)
        {
            foreach (var key in collection.AllKeys)
            {
                if (collection[key] != null)
                {
                    _collection[key] = collection[key];
                }
            }
        }

        public string[] AllKeys => _collection.Keys.ToArray();

        // Добавляем отсутствующее свойство Count
        public int Count => _collection.Count;

        public string this[string key]
        {
            get => _collection.TryGetValue(key, out var value) ? value : null;
            set => _collection[key] = value;
        }

        public void Add(string key, string value)
        {
            _collection[key] = value;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var key in _collection.Keys)
            {
                if (sb.Length > 0) sb.Append('&');
                sb.Append(WebUtility.UrlEncode(key));
                sb.Append('=');
                sb.Append(WebUtility.UrlEncode(_collection[key] ?? ""));
            }
            return sb.ToString();
        }
    }
}
