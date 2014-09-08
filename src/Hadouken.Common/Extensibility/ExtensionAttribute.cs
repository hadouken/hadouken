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

        /// <summary>
        /// Gets the ID of this extension. Should be
        /// unique among all local extensions.
        /// </summary>
        public string ExtensionId
        {
            get { return _extensionId; }
        }

        /// <summary>
        /// Gets or sets the displayed name of this extension.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the extension description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the namespace root for
        /// embedded resouces this extension might have.
        /// </summary>
        public string ResourceNamespace { get; set; }

        /// <summary>
        /// Gets or sets the JS files to load in the web UI.
        /// </summary>
        public string[] Scripts { get; set; }
    }
}
