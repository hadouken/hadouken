using System;
using System.Collections.Generic;

namespace Hadouken.Configuration
{
    public interface IPluginConfigurationCollection : IEnumerable<IPluginConfiguration>
    {
        string BaseDirectory { get; set; }

        Uri RepositoryUri { get; set; }
    }

    public interface IPluginConfiguration
    {
        string Id { get; }

        string Path { get; }
    }
}
