using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using Hadouken.SemVer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hadouken.Plugins.Metadata
{
    public sealed class Manifest : IManifest
    {
        public const string FileName = "manifest.json";

        private static readonly IDictionary<int, Func<IManifestReader>> ManifestReaders =
            new Dictionary<int, Func<IManifestReader>>();

        static Manifest()
        {
            ManifestReaders.Add(1, () => new ManifestV1Reader());
            ManifestReaders.Add(2, () => new ManifestV2Reader());
        }

        public Manifest(string name, SemanticVersion version)
        {
            Name = name;
            Version = version;

            // Default values
            MinimumHostVersion = new SemanticVersion("0.0");
            Dependencies = Enumerable.Empty<Dependency>();
            EventHandlers = Enumerable.Empty<EventHandler>();
            Permissions = new PermissionSet(PermissionState.None);
            UserInterface = new UserInterface();
        }
        
        public string Name { get; private set; }

        public SemanticVersion Version { get; private set; }

        public SemanticVersion MinimumHostVersion { get; set; }

        public IEnumerable<Dependency> Dependencies { get; set; }

        public IEnumerable<EventHandler> EventHandlers { get; set; } 

        public UserInterface UserInterface { get; set; }

        public PermissionSet Permissions { get; set; }

        public static bool TryParse(Stream json, out IManifest manifest)
        {
            Exception exception;
            return TryParse(json, out manifest, out exception);
        }

        public static bool TryParse(Stream json, out IManifest manifest, out Exception exception)
        {
            manifest = null;
            exception = null;

            try
            {
                using (var streamReader = new StreamReader(json))
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    var obj = JToken.ReadFrom(jsonReader) as JObject;

                    if (obj == null)
                        return false;

                    JToken manifestVersionToken;

                    if (!obj.TryGetValue("manifest_version", out manifestVersionToken))
                        return false;

                    if (manifestVersionToken.Type != JTokenType.Integer)
                        return false;

                    var manifestVersion = manifestVersionToken.Value<int>();

                    if (!ManifestReaders.ContainsKey(manifestVersion))
                        return false;

                    var manifestReader = ManifestReaders[manifestVersion]();
                    manifest = manifestReader.Read(obj);

                    return true;
                }
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
        }
    }
}
