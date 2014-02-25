using Hadouken.Framework.SemVer;

namespace Hadouken.Plugins.Metadata
{
    public interface IManifest
    {
        string AssemblyFile { get; }

        string Name { get; }

        SemanticVersion Version { get; }

        Dependency[] Dependencies { get; }
    }
}
