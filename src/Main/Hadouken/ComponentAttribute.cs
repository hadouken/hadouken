using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentAttribute : Attribute
    {
        public ComponentAttribute() : this(ComponentLifestyle.Singleton)
        {
        }

        public ComponentAttribute(ComponentLifestyle lifestyle)
        {
            Lifestyle = lifestyle;
        }

        public ComponentLifestyle Lifestyle { get; private set; }
    }
}
