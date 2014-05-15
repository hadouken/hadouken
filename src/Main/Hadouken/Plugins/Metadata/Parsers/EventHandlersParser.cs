using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Hadouken.Plugins.Metadata.Parsers
{
    public sealed class EventHandlersParser
    {
        public IEnumerable<EventHandler> Parse(JToken value)
        {
            var handlersList = value as JObject;

            if (handlersList == null)
            {
                return Enumerable.Empty<EventHandler>();
            }

            var result = new List<EventHandler>();

            foreach (var pair in handlersList)
            {
                var name = pair.Key;
                var target = pair.Value.Value<string>();

                result.Add(new EventHandler(name, target));
            }

            return result;
        }
    }
}
