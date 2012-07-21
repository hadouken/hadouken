using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken
{
    public static class DateTimeExtensions
    {
        private static DateTime unix = new DateTime(1970, 1, 1).ToLocalTime();

        public static int ToUnixTime(this DateTime dt)
        {
            return (int)(dt - unix).TotalSeconds;
        }
    }
}
