using System;
using System.Collections;
using System.Collections.Generic;

namespace Hadouken.Common.Text.BEncoding {
    public sealed class BEncodedList : BEncodedValue, IList<BEncodedValue> {
        private readonly IList<BEncodedValue> _items;

        public BEncodedList(IList<BEncodedValue> items) {
            if (items == null) {
                throw new ArgumentNullException("items");
            }
            this._items = items;
        }

        public IEnumerator<BEncodedValue> GetEnumerator() {
            return this._items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        public void Add(BEncodedValue item) {
            this._items.Add(item);
        }

        public void Clear() {
            this._items.Clear();
        }

        public bool Contains(BEncodedValue item) {
            return this._items.Contains(item);
        }

        public void CopyTo(BEncodedValue[] array, int arrayIndex) {
            this._items.CopyTo(array, arrayIndex);
        }

        public bool Remove(BEncodedValue item) {
            return this._items.Remove(item);
        }

        public int Count {
            get { return this._items.Count; }
        }

        public bool IsReadOnly {
            get { return this._items.IsReadOnly; }
        }

        public int IndexOf(BEncodedValue item) {
            return this._items.IndexOf(item);
        }

        public void Insert(int index, BEncodedValue item) {
            this._items.Insert(index, item);
        }

        public void RemoveAt(int index) {
            this._items.RemoveAt(index);
        }

        public BEncodedValue this[int index] {
            get { return this._items[index]; }
            set { this._items[index] = value; }
        }
    }
}