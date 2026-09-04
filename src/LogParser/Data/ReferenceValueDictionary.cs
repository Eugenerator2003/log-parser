using System;
using System.Collections.Generic;
using System.Text;

namespace LogParser.Data
{
    public class ReferenceValueDictionary<T, TKey> where TKey : notnull
    {
        private Func<T, TKey> selector;
        private Dictionary<TKey, T> dictionary;

        public ReferenceValueDictionary(Func<T, TKey> selector)
        {
            this.selector = selector;
            dictionary = new Dictionary<TKey, T>();
        }

        public void Add(T value)
        {
            dictionary.Add(selector.Invoke(value), value);
        }

        public T? Get(TKey key)
        {
            dictionary.TryGetValue(key, out var value);
            return value;
        }
    }
}
