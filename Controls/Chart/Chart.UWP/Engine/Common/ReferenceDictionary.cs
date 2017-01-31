using System;
using System.Collections.Generic;

namespace Telerik.Charting
{
    /// <summary>
    /// A dictionary, based on a List is faster and with smaller footprint for several items - e.g. 2 to 9.
    /// In .NET there is the HybridDictionary class which actually uses LinkedList for up to 9 items.
    /// </summary>
    internal class ReferenceDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> where TKey : class
    {
        private List<KeyValuePair<TKey, TValue>> items = new List<KeyValuePair<TKey, TValue>>(4);

        public int Count
        {
            get
            {
                return this.items.Count;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                KeyValuePair<TKey, TValue>? match = this.FindEntry(key);
                if (match != null)
                {
                    return match.Value.Value;
                }

                return default(TValue);
            }
        }

        public bool ContainsKey(TKey key)
        {
            return this.FindEntry(key) != null;
        }

        public void Set(TKey key, TValue value)
        {
            for (int i = 0; i < this.items.Count; i++)
            {
                KeyValuePair<TKey, TValue> item = this.items[i];
                if (object.ReferenceEquals(key, item.Key))
                {
                    // an item with the same key already exists
                    this.items[i] = new KeyValuePair<TKey, TValue>(key, value);
                    return;
                }
            }

            this.items.Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            KeyValuePair<TKey, TValue>? match = this.FindEntry(key);
            if (match == null)
            {
                value = default(TValue);
                return false;
            }

            value = match.Value.Value;
            return true;
        }

        public void Clear()
        {
            this.items.Clear();
        }

        public void Remove(TKey key)
        {
            for (int i = 0; i < this.items.Count; i++)
            {
                if (object.ReferenceEquals(this.items[i].Key, key))
                {
                    this.items.RemoveAt(i);
                    return;
                }
            }
        }

        public IEnumerable<TKey> EnumerateKeys()
        {
            for (int i = 0; i < this.items.Count; i++)
            {
                yield return this.items[i].Key;
            }
        }

        public IEnumerable<TValue> EnumerateValues()
        {
            for (int i = 0; i < this.items.Count; i++)
            {
                yield return this.items[i].Value;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        private KeyValuePair<TKey, TValue>? FindEntry(TKey key)
        {
            for (int i = 0; i < this.items.Count; i++)
            {
                // ReferenceEquals is faster than Equals
                if (object.ReferenceEquals(key, this.items[i].Key))
                {
                    return this.items[i];
                }
            }

            return null;
        }
    }
}
