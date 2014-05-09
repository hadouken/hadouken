using System.Collections.Generic;
using Hadouken.SemVer;

namespace Hadouken.Plugins.Metadata
{
    public interface IManifest
    {
        string Name { get; }

        SemanticVersion Version { get; }

        SemanticVersion MinimumHostVersion { get; }

        IEnumerable<Dependency> Dependencies { get; }

        IEnumerable<EventHandler> EventHandlers { get; }
            
        IUserInterface UserInterface { get; }
    }
}
