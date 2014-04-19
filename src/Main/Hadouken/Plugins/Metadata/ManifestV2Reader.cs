using System.Collections.Generic;
using System.Linq;
using Hadouken.SemVer;
using Newtonsoft.Json.Linq;

namespace Hadouken.Plugins.Metadata
{
    public class ManifestV2Reader : IManifestReader
    {
        public Manifest Read(JObject manifestObject)
        {
            // Ensure manifest_version is 2
            var manifestVersionToken = manifestObject["manifest_version"];

            if (manifestVersionToken == null)
                return null;

            var manifestVersion = manifestVersionToken.Value<int>();

            if (manifestVersion != 2)
                return null;

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

            IUserInterface userInterface = null;

            // Read "ui" element
            var ui = manifestObject["ui"] as JObject;

            if (ui != null)
            {
                var backgroundScriptsElement = ui["background"] as JArray;
                var backgroundScripts = Enumerable.Empty<string>();

                if (backgroundScriptsElement != null)
                {
                    backgroundScripts = backgroundScriptsElement.Select(t => t.Value<string>()).ToList();
                }

                var pagesObject = ui["pages"] as JObject;
                var pages = new Dictionary<string, IPage>();

                // Read "pages" elements
                if (pagesObject != null)
                {
                    foreach (var pageObject in pagesObject.Children<JProperty>())
                    {
                        var obj = (JObject) pageObject.Value;

                        var scriptsElement = obj["scripts"] as JArray;
                        var scripts = Enumerable.Empty<string>();

                        if (scriptsElement != null)
                        {
                            scripts = scriptsElement.Select(t => t.Value<string>());
                        }

                        pages.Add(pageObject.Name, new Page(obj["html"].Value<string>(), scripts));
                    }
                }

                userInterface = new UserInterface(backgroundScripts, pages);
            }

            var name = manifestObject["id"].Value<string>();
            var version = new SemanticVersion(manifestObject["version"].Value<string>());

            return new Manifest(name, version, dependencyList, userInterface);
        }
    }
}
