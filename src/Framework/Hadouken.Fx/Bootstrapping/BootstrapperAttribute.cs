using System;

namespace Hadouken.Fx.Bootstrapping
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BootstrapperAttribute : Attribute
    {
        private readonly Type _bootstrapperType;

        public BootstrapperAttribute(Type bootstrapperType)
        {
            _bootstrapperType = bootstrapperType;
        }

        public Type Type
        {
            get { return _bootstrapperType; }
        }
    }
}
