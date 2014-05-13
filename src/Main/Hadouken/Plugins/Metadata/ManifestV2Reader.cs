using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using Hadouken.Plugins.Metadata.Permissions;
using Hadouken.SemVer;
using Newtonsoft.Json.Linq;

namespace Hadouken.Plugins.Metadata
{
    public class ManifestV2Reader : IManifestReader
    {
        private static readonly IDictionary<string, Func<IPermissionParser>> PermissionParsers =
            new Dictionary<string, Func<IPermissionParser>>();

        static ManifestV2Reader()
        {
            PermissionParsers.Add("dns", () => new DnsPermissionParser());
            PermissionParsers.Add("fileio", () => new FileIOPermissionParser());
            PermissionParsers.Add("sockets", () => new SocketsPermissionParser());
        }

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

            // Read "eventHandlers" element
            var eventHandlersObject = manifestObject["eventHandlers"] as JObject;
            var eventHandlers = new List<EventHandler>();

            if (eventHandlersObject != null)
            {
                foreach (var pair in eventHandlersObject)
                {
                    eventHandlers.Add(new EventHandler(pair.Key, pair.Value.Value<string>()));
                }
            }

            var name = manifestObject["id"].Value<string>();
            var version = new SemanticVersion(manifestObject["version"].Value<string>());

            SemanticVersion minimumHostVersion = "0.0";

            if (manifestObject["minimumHostVersion"] != null)
            {
                minimumHostVersion = new SemanticVersion(manifestObject["minimumHostVersion"].Value<string>());
            }

            var set = new PermissionSet(PermissionState.None);
            var perms = manifestObject["permissions"] as JObject;

            if (perms != null)
            {
                foreach (var pair in perms)
                {
                    if (!PermissionParsers.ContainsKey(pair.Key)) continue;

                    var parser = PermissionParsers[pair.Key]();

                    if (pair.Value.Type == JTokenType.String
                        && pair.Value.Value<string>() == "<unrestricted>")
                    {
                        set.AddPermission(parser.GetUnrestricted());
                    }
                    else
                    {
                        set.AddPermission(parser.Parse(pair.Value));
                    }
                }
            }

            return new Manifest(name, version, minimumHostVersion, dependencyList, eventHandlers, userInterface, set);
        }
    }
}
