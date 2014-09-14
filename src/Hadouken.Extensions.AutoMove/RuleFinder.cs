using System;
using System.Linq;
using System.Text.RegularExpressions;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Extensibility;
using Hadouken.Extensions.AutoMove.Data;
using Hadouken.Extensions.AutoMove.Data.Models;

namespace Hadouken.Extensions.AutoMove
{
    [Component]
    public sealed class RuleFinder : IRuleFinder
    {
        private readonly IAutoMoveRepository _autoMoveRepository;
        private readonly ISourceValueProvider _sourceValueProvider;

        public RuleFinder(IAutoMoveRepository autoMoveRepository,
            ISourceValueProvider sourceValueProvider)
        {
            if (autoMoveRepository == null) throw new ArgumentNullException("autoMoveRepository");
            if (sourceValueProvider == null) throw new ArgumentNullException("sourceValueProvider");
            _autoMoveRepository = autoMoveRepository;
            _sourceValueProvider = sourceValueProvider;
        }

        public Rule FindRule(ITorrent torrent)
        {
            var rules = _autoMoveRepository.GetRules() ?? new Rule[] {};
            return (from rule in rules
                let parameters = _autoMoveRepository.GetParametersByRuleId(rule.Id) ?? new Parameter[] {}
                where parameters.All(p => MatchesTorrent(torrent, p))
                select rule).FirstOrDefault();
        }

        private bool MatchesTorrent(ITorrent torrent, Parameter parameter)
        {
            var sourceValue = _sourceValueProvider.GetValue(torrent, parameter.Source);
            return Regex.IsMatch(sourceValue, parameter.Pattern);
        }
    }
}