
using System.Collections.Generic;

namespace MuonhoryoLibrary
{
    public static class DictionaryExtensions
    {
        public static void Add<TKey, TValue>(this Dictionary<TKey, TValue> dictionary,
            KeyValuePair<TKey, TValue> pair)
        {
            dictionary.Add(pair.Key, pair.Value);
        }
    }
}
