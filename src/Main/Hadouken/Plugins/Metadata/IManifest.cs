using System.Collections.Generic;
using System.Security;
using Hadouken.SemVer;

namespace Hadouken.Plugins.Metadata
{
    public interface IManifest
    {
        SemanticVersion MinimumHostVersion { get; }

        IEnumerable<EventHandler> EventHandlers { get; }
            
        UserInterface UserInterface { get; }

        PermissionSet Permissions { get; }
    }
}
