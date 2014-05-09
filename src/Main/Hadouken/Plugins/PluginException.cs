using System;

namespace Hadouken.Plugins
{
    [Serializable]
    public sealed class PluginException : Exception
    {
        public PluginException()
            : base()
        {

        }

        public PluginException(string message)
            : base(message)
        {

        }
    }
}