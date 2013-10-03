using System;
using System.Collections.Generic;
using Hadouken.Framework.SemVer;
using Newtonsoft.Json.Linq;

namespace Hadouken.Plugins.Metadata
{
    public class ManifestV1Reader : IManifestReader
    {
        public Manifest Read(JObject manifestObject)
        {
            // Ensure manifest_version is 1
            var manifestVersionToken = manifestObject["manifest_version"];

            if (manifestVersionToken == null)
                return null;

            var manifestVersion = manifestVersionToken.Value<int>();

            if (manifestVersion != 1)
                return null;

            var manifest = new Manifest
            {
                Name = manifestObject["id"].Value<string>(),
                Version = new SemanticVersion(manifestObject["version"].Value<string>())
            };

            // Read all dependencies
            var dependencies = manifestObject["dependencies"] as JArray;
            var dependencyList = new List<Dependency>();

            if (dependencies == null) return manifest;

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

            manifest.Dependencies = dependencyList.ToArray();

            return manifest;
        }
    }
}
