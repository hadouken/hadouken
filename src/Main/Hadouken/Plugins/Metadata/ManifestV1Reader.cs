using System.Collections.Generic;
using Hadouken.SemVer;
using Newtonsoft.Json.Linq;

namespace Hadouken.Plugins.Metadata
{
    public class ManifestV1Reader : IManifestReader
    {
        public Manifest Read(JObject manifestObject)
        {
            // Required fields
            if (manifestObject["id"] == null)
            {
                throw new MissingPropertyException("id");
            }

            if (manifestObject["version"] == null)
            {
                throw new MissingPropertyException("version");
            }

            // Read all dependencies
            var dependencies = manifestObject["dependencies"] as JArray;
            var dependencyList = new List<Dependency>();

            if (dependencies != null)
            {
                foreach (var dependency in dependencies.Children())
                {
                    SemanticVersionRange range;

                    if (!SemanticVersionRange.TryParse(dependency["version"].Value<string>(), out range))
                        continue;

                    var d = new Dependency
                    {
                        Name = dependency["id"].Value<string>(),
                        VersionRange = range
                    };

                    dependencyList.Add(d);
                }
            }

            var name = manifestObject["id"].Value<string>();
            var version = new SemanticVersion(manifestObject["version"].Value<string>());

            return new Manifest(name, version)
            {
                Dependencies = dependencyList
            };
        }
    }
}
