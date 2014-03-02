using System;

namespace Hadouken.Fx
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PluginBootstrapperAttribute : Attribute
    {
        private readonly Type _bootstrapperType;

        public PluginBootstrapperAttribute(Type bootstrapperType)
        {
            _bootstrapperType = bootstrapperType;
        }

        public Type Type
        {
            get { return _bootstrapperType; }
        }
    }
}
