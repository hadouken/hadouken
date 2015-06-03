using System;
using System.Collections;
using System.Collections.Generic;

namespace Hadouken.Common.Text.BEncoding {
    public sealed class BEncodedDictionary : BEncodedValue, IDictionary<BEncodedString, BEncodedValue> {
        private readonly IDictionary<BEncodedString, BEncodedValue> _map;

        public BEncodedDictionary(IDictionary<BEncodedString, BEncodedValue> map) {
            if (map == null) {
                throw new ArgumentNullException("map");
            }
            this._map = map;
        }

        public IEnumerator<KeyValuePair<BEncodedString, BEncodedValue>> GetEnumerator() {
            return this._map.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        public void Add(KeyValuePair<BEncodedString, BEncodedValue> item) {
            this._map.Add(item);
        }

        public void Clear() {
            this._map.Clear();
        }

        public bool Contains(KeyValuePair<BEncodedString, BEncodedValue> item) {
            return this._map.Contains(item);
        }

        public void CopyTo(KeyValuePair<BEncodedString, BEncodedValue>[] array, int arrayIndex) {
            this._map.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<BEncodedString, BEncodedValue> item) {
            return this._map.Remove(item);
        }

        public int Count {
            get { return this._map.Count; }
        }

        public bool IsReadOnly {
            get { return this._map.IsReadOnly; }
        }

        public bool ContainsKey(BEncodedString key) {
            return this._map.ContainsKey(key);
        }

        public void Add(BEncodedString key, BEncodedValue value) {
            this._map.Add(key, value);
        }

        public bool Remove(BEncodedString key) {
            return this._map.Remove(key);
        }

        public bool TryGetValue(BEncodedString key, out BEncodedValue value) {
            return this._map.TryGetValue(key, out value);
        }

        public BEncodedValue this[BEncodedString key] {
            get { return this._map[key]; }
            set { this._map[key] = value; }
        }

        public ICollection<BEncodedString> Keys {
            get { return this._map.Keys; }
        }

        public ICollection<BEncodedValue> Values {
            get { return this._map.Values; }
        }
    }
}