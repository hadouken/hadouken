using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hadouken.BitTorrent;
using MonoTorrent.Common;

namespace Hadouken.Impl.BitTorrent
{
    public class HdknBitField : IBitField
    {
        private BitField _bitfield;

        internal HdknBitField(BitField bitfield)
        {
            _bitfield = bitfield;
        }

        public int Length
        {
            get { return _bitfield.Length; }
        }

        public int LengthInBytes
        {
            get { return _bitfield.LengthInBytes; }
        }

        public double PercentComplete
        {
            get { return _bitfield.PercentComplete; }
        }

        public int TrueCount
        {
            get { return _bitfield.TrueCount; }
        }

        public bool this[int index]
        {
            get { return _bitfield[index]; }
        }

        public IBitField And(IBitField value)
        {
            _bitfield = _bitfield.And(((HdknBitField)value)._bitfield);
            return this;
        }

        public IBitField Clone()
        {
            return new HdknBitField(_bitfield.Clone());
        }

        public int FirstFalse()
        {
            return _bitfield.FirstFalse();
        }

        public int FirstFalse(int startIndex, int endIndex)
        {
            return _bitfield.FirstFalse(startIndex, endIndex);
        }

        public int FirstTrue()
        {
            return _bitfield.FirstTrue();
        }

        public int FirstTrue(int startIndex, int endIndex)
        {
            return _bitfield.FirstTrue(startIndex, endIndex);
        }

        public IBitField From(IBitField value)
        {
            _bitfield = _bitfield.From(((HdknBitField)value)._bitfield);
            return this;
        }

        public IEnumerator<bool> GetEnumerator()
        {
            return _bitfield.GetEnumerator();
        }

        public IBitField Not()
        {
            _bitfield = _bitfield.Not();
            return this;
        }

        public IBitField Or(IBitField value)
        {
            _bitfield = _bitfield.Or(((HdknBitField)value)._bitfield);
            return this;
        }

        public IBitField Set(int index, bool value)
        {
            _bitfield = _bitfield.Set(index, value);
            return this;
        }

        public IBitField Xor(IBitField value)
        {
            _bitfield = _bitfield.Xor(((HdknBitField)value)._bitfield);
            return this;
        }
    }
}
