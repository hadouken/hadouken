using System;
using Newtonsoft.Json;

namespace Hadouken.Plugins
{
    public sealed class Manifest
    {
        [JsonProperty("manifest_version", Required = Required.Always)]
        public int ManifestVersion { get; set; }

        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty("description", Required = Required.Default)]
        public string Description { get; set; }

        [JsonProperty("version", Required =  Required.Always)]
        public Version  Version { get; set; }
    }
}
