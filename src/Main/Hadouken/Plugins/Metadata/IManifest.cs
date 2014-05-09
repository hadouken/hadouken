using System.Collections.Generic;
using Hadouken.SemVer;

namespace Hadouken.Plugins.Metadata
{
    public interface IManifest
    {
        string Name { get; }

        SemanticVersion Version { get; }

        IEnumerable<Dependency> Dependencies { get; }

        IEnumerable<EventHandler> EventHandlers { get; }
            
        IUserInterface UserInterface { get; }
    }
}
