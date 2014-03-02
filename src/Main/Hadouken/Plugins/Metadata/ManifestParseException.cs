using System;

namespace Hadouken.Plugins.Metadata
{
    internal class ManifestParseException : Exception
    {
        public ManifestParseException(Exception innerException)
            : base("Could not load plugin manifest file", innerException)
        {
        }
    }
}