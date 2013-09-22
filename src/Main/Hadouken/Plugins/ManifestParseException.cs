using System;

namespace Hadouken.Plugins
{
    internal class ManifestParseException : Exception
    {
        public ManifestParseException(Exception innerException)
            : base("Could not load plugin manifest file", innerException)
        {
        }
    }
}