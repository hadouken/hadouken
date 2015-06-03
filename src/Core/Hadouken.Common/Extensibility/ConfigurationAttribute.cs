using System;

namespace Hadouken.Common.Extensibility {
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ConfigurationAttribute : Attribute {
        private readonly Type _type;

        public ConfigurationAttribute(Type type) {
            if (type == null) {
                throw new ArgumentNullException("type");
            }
            this._type = type;
        }

        public Type Type {
            get { return this._type; }
        }

        public string Key { get; set; }
    }
}