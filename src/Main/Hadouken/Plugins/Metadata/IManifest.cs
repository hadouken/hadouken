using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework.SemVer;

namespace Hadouken.Plugins.Metadata
{
    public interface IManifest
    {
        string Name { get; }

        SemanticVersion Version { get; }

        Dependency[] Dependencies { get; }
    }
}
