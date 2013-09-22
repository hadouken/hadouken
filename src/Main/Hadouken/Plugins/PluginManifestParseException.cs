using System;

namespace Hadouken.Plugins
{
    internal class PluginManifestParseException : Exception
    {
        public PluginManifestParseException(Exception innerException)
            : base("Could not load plugin manifest file", innerException)
        {
        }
    }
}