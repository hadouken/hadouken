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

        /// <summary>
        /// Uses the specified instance of IComponentFactory to create this component.
        /// </summary>
        /// <param name="factory">The factory type, must inherit IComponentFactory.</param>
        public ComponentAttribute(Type factory)
        {
            Factory = factory;
        }

        public Type Factory { get; private set; }
    }
}
