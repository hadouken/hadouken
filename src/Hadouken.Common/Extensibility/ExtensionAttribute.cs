using System;

namespace Hadouken.Common.Extensibility
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ExtensionAttribute : Attribute
    {
        private readonly string _extensionId;

        public ExtensionAttribute(string extensionId)
        {
            if (extensionId == null) throw new ArgumentNullException("extensionId");
            _extensionId = extensionId;
        }

        public string ExtensionId
        {
            get { return _extensionId; }
        }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
