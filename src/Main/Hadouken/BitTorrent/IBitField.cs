using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.BitTorrent
{
    public interface IBitField
    {
        int Length { get; }
        int LengthInBytes { get; }
        double PercentComplete { get; }
        int TrueCount { get; }

        bool this[int index] { get; }

        IBitField And(IBitField value);
        IBitField Clone();
        int FirstFalse();
        int FirstFalse(int startIndex, int endIndex);
        int FirstTrue();
        int FirstTrue(int startIndex, int endIndex);
        IBitField From(IBitField value);
        IEnumerator<bool> GetEnumerator();
        
        IBitField Not();
        IBitField Or(IBitField value);
        IBitField Set(int index, bool value);
        IBitField Xor(IBitField value);
    }
}
