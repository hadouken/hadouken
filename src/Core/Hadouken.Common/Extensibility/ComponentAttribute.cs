using System;

namespace Hadouken.Common.Extensibility
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentAttribute : Attribute
    {
        private readonly ComponentLifestyle _lifestyle;

        public ComponentAttribute(ComponentLifestyle lifestyle = ComponentLifestyle.Transient)
        {
            _lifestyle = lifestyle;
        }

        public ComponentLifestyle Lifestyle
        {
            get { return _lifestyle; }
        }
    }
}
