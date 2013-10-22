using Hadouken.Framework.SemVer;

namespace Hadouken.Plugins.Repository.Models
{
    public sealed class Release
    {
        public SemanticVersion Version { get; set; }

        public string Changelog { get; set; }
    }
}
