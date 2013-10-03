using System;
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
                Name = manifestObject["name"].Value<string>(),
                Version = new SemanticVersion(manifestObject["version"].Value<string>())
            };

            return manifest;
        }
    }
}
