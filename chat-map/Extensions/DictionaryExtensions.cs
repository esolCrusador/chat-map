using System;
using System.Collections.Generic;

namespace ChatMap.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> create)
        {
            TValue value;
            if (!dictionary.TryGetValue(key, out value))
                lock (dictionary)
                    if (!dictionary.TryGetValue(key, out value)) {
                        value = create(key);

                        dictionary.Add(key, value);
                    }

            return value;
        }
    }
}
