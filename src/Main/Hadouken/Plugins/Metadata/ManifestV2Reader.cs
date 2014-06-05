using System;
using System.Collections.Generic;
using Hadouken.Plugins.Metadata.Parsers;
using Hadouken.SemVer;
using Newtonsoft.Json.Linq;

namespace Hadouken.Plugins.Metadata
{
    public class ManifestV2Reader : IManifestReader
    {
        private static readonly IDictionary<string, Action<Manifest, JToken>> Parsers =
            new Dictionary<string, Action<Manifest, JToken>>();

        static ManifestV2Reader()
        {
            Parsers.Add("dependencies", (m, t) => m.Dependencies = new DependenciesParser().Parse(t));
            Parsers.Add("eventHandlers", (m, t) => m.EventHandlers = new EventHandlersParser().Parse(t));
            Parsers.Add("permissions", (m, t) => m.Permissions = new PermissionsParser().Parse(t));
            Parsers.Add("ui", (m, t) => m.UserInterface = new UserInterfaceParser().Parse(t));
        }

        public Manifest Read(JObject manifestObject)
        {
            var manifest = new Manifest();

            foreach (var item in manifestObject)
            {
                Action<Manifest, JToken> parser;

                if (!Parsers.TryGetValue(item.Key, out parser))
                {
                    continue;
                }

                parser(manifest, item.Value);
            }

            return manifest;
        }
    }
}
