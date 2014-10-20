using System;
using System.Collections;
using System.Collections.Generic;

namespace Hadouken.Common.Text.BEncoding
{
    public sealed class BEncodedDictionary : BEncodedValue, IDictionary<BEncodedString, BEncodedValue>
    {
        private readonly IDictionary<BEncodedString, BEncodedValue> _map;

        public BEncodedDictionary(IDictionary<BEncodedString, BEncodedValue> map)
        {
            if (map == null) throw new ArgumentNullException("map");
            _map = map;
        }

        public IEnumerator<KeyValuePair<BEncodedString, BEncodedValue>> GetEnumerator()
        {
            return _map.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<BEncodedString, BEncodedValue> item)
        {
            _map.Add(item);
        }

        public void Clear()
        {
            _map.Clear();
        }

        public bool Contains(KeyValuePair<BEncodedString, BEncodedValue> item)
        {
            return _map.Contains(item);
        }

        public void CopyTo(KeyValuePair<BEncodedString, BEncodedValue>[] array, int arrayIndex)
        {
            _map.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<BEncodedString, BEncodedValue> item)
        {
            return _map.Remove(item);
        }

        public int Count
        {
            get { return _map.Count; }
        }

        public bool IsReadOnly
        {
            get { return _map.IsReadOnly; }
        }

        public bool ContainsKey(BEncodedString key)
        {
            return _map.ContainsKey(key);
        }

        public void Add(BEncodedString key, BEncodedValue value)
        {
            _map.Add(key, value);
        }

        public bool Remove(BEncodedString key)
        {
            return _map.Remove(key);
        }

        public bool TryGetValue(BEncodedString key, out BEncodedValue value)
        {
            return _map.TryGetValue(key, out value);
        }

        public BEncodedValue this[BEncodedString key]
        {
            get { return _map[key]; }
            set { _map[key] = value; }
        }

        public ICollection<BEncodedString> Keys
        {
            get { return _map.Keys; }
        }

        public ICollection<BEncodedValue> Values
        {
            get { return _map.Values; }
        }
    }
}
