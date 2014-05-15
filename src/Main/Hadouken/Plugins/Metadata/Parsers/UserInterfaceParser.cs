using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Hadouken.Plugins.Metadata.Parsers
{
    public sealed class UserInterfaceParser
    {
        public UserInterface Parse(JToken value)
        {
            var ui = value as JObject;

            if (ui == null)
            {
                return new UserInterface();
            }

            var backgroundScripts = new List<string>();
            var pages = new Dictionary<string, Page>();

            var background = ui["background"] as JArray;
            if (background != null)
            {
                backgroundScripts = background.Values<string>().ToList();
            }

            var pagesObject = ui["pages"] as JObject;
            if (pagesObject != null)
            {
                foreach (var item in pagesObject)
                {
                    var scripts = Enumerable.Empty<string>();

                    if (item.Value["scripts"] != null)
                    {
                        scripts = item.Value["scripts"].Values<string>();
                    }

                    var html = item.Value["html"].Value<string>();

                    pages.Add(item.Key, new Page(html, scripts));
                }
            }

            return new UserInterface(backgroundScripts, pages);
        }
    }
}
