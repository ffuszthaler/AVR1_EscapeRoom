using System.Collections.Generic;

namespace DefaultNamespace
{
    public static class DictionaryExt
    {
        public static bool ContainsKeyValuePair<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            return dict.TryGetValue(key, out var validInteractable) &&
               validInteractable.Equals(value);
        }
    }
}