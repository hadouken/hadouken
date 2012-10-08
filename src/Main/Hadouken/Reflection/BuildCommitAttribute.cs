using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Reflection
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class BuildCommitAttribute : Attribute
    {
        public BuildCommitAttribute(string hash)
        {
            Hash = hash;
        }

        public string Hash { get; private set; }
    }
}
