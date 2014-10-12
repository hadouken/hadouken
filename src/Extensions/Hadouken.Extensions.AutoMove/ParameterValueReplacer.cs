using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Extensibility;
using Hadouken.Extensions.AutoMove.Data.Models;

namespace Hadouken.Extensions.AutoMove
{
    [Component]
    public class ParameterValueReplacer : IParameterValueReplacer
    {
        private readonly ISourceValueProvider _sourceValueProvider;

        public ParameterValueReplacer(ISourceValueProvider sourceValueProvider)
        {
            if (sourceValueProvider == null) throw new ArgumentNullException("sourceValueProvider");
            _sourceValueProvider = sourceValueProvider;
        }

        public string Replace(ITorrent torrent, IEnumerable<Parameter> parameters, string targetPath)
        {
            var tokens = new Dictionary<string, string>();

            foreach (var parameter in parameters)
            {
                var sourceValue = _sourceValueProvider.GetValue(torrent, parameter.Source);

                var regex = new Regex(parameter.Pattern, RegexOptions.ExplicitCapture | RegexOptions.Singleline);
                var match = regex.Match(sourceValue);

                foreach (var groupName in regex.GetGroupNames().Skip(1))
                {
                    if (tokens.ContainsKey(groupName)) continue;

                    var group = match.Groups[groupName];
                    tokens.Add(groupName, group.Value);
                }
            }

            return tokens.Aggregate(targetPath, (current, token) => current.Replace(string.Format("${{{0}}}", token.Key), token.Value));
        }
    }
}