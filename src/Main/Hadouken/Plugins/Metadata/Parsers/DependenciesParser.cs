using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hadouken.SemVer;
using Newtonsoft.Json.Linq;

namespace Hadouken.Plugins.Metadata.Parsers
{
    public sealed class DependenciesParser
    {
        private static readonly string[] ValidDependencyProperties = new[] {"id", "version"};

        public IEnumerable<Dependency> Parse(JToken value)
        {
            var dependencyList = value as JArray;

            if (dependencyList == null)
            {
                throw new InvalidDataException("Value of 'dependencies' must be an array.");
            }

            if (dependencyList.Children().Any(c => c.Type != JTokenType.Object))
            {
                throw new InvalidDataException("Elements of the 'dependencies' array must be objects.");
            }

            var result = new List<Dependency>();

            foreach (var item in dependencyList.Children<JObject>())
            {
                var props = item.Properties().OrderBy(p => p.Name).Select(p => p.Name).ToList();

                if (!ValidDependencyProperties.SequenceEqual(props))
                {
                    var unexpectedProperties = string.Join(",", props.Except(ValidDependencyProperties));
                    throw new UnexpectedPropertyException(unexpectedProperties);
                }

                var id = item["id"].Value<string>();
                var version = item["version"].Value<string>();

                SemanticVersionRange range;

                if (!SemanticVersionRange.TryParse(version, out range))
                {
                    throw new InvalidDataException("Invalid semantic version range: " + version);
                }

                result.Add(new Dependency
                {
                    Name = id,
                    VersionRange = range 
                });
            }

            return result;
        } 
    }
}
