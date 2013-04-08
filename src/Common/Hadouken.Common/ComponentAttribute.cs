using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Common
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentAttribute : Attribute
    {
        public ComponentAttribute()
        {
        }

        public ComponentAttribute(ComponentType componentType)
        {
            ComponentType = componentType;
        }

        public ComponentType ComponentType { get; private set; }
    }
}
