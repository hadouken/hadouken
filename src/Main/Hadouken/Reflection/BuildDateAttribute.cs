using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Reflection
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class BuildDateAttribute : Attribute
    {
        public BuildDateAttribute(int unixTime)
        {
            BuildDate = new DateTime(1970, 1, 1).AddSeconds(unixTime);
        }

        public DateTime BuildDate { get; private set; }
    }
}
