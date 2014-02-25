using Hadouken.SemVer;

namespace Hadouken.Plugins.Metadata
{
    public sealed class Dependency
    {
        public string Name { get; set; }

        public SemanticVersionRange VersionRange { get; set; }
    }
}
