using System;
using System.Collections;
using System.Collections.Generic;

namespace Hadouken.Common.Text.BEncoding
{
    public sealed class BEncodedList : BEncodedValue, IList<BEncodedValue>
    {
        private readonly IList<BEncodedValue> _items;

        public BEncodedList(IList<BEncodedValue> items)
        {
            if (items == null) throw new ArgumentNullException("items");
            _items = items;
        }

        public IEnumerator<BEncodedValue> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(BEncodedValue item)
        {
            _items.Add(item);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public bool Contains(BEncodedValue item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(BEncodedValue[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public bool Remove(BEncodedValue item)
        {
            return _items.Remove(item);
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public bool IsReadOnly
        {
            get { return _items.IsReadOnly; }
        }

        public int IndexOf(BEncodedValue item)
        {
            return _items.IndexOf(item);
        }

        public void Insert(int index, BEncodedValue item)
        {
            _items.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }

        public BEncodedValue this[int index]
        {
            get { return _items[index]; }
            set { _items[index] = value; }
        }
    }
}
