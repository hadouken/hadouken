using System;

namespace Hadouken.Common.Extensibility {
    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentAttribute : Attribute {
        private readonly ComponentLifestyle _lifestyle;

        public ComponentAttribute(ComponentLifestyle lifestyle = ComponentLifestyle.Transient) {
            this._lifestyle = lifestyle;
        }

        public ComponentLifestyle Lifestyle {
            get { return this._lifestyle; }
        }
    }
}