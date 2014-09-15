using System;
using System.Collections.Generic;
using Hadouken.Common.JsonRpc;
using Hadouken.Extensions.AutoMove.Data;
using Hadouken.Extensions.AutoMove.Data.Models;

namespace Hadouken.Extensions.AutoMove.Services
{
    public sealed class AutoMoveServices : IJsonRpcService
    {
        private readonly IAutoMoveRepository _autoMoveRepository;

        public AutoMoveServices(IAutoMoveRepository autoMoveRepository)
        {
            if (autoMoveRepository == null) throw new ArgumentNullException("autoMoveRepository");
            _autoMoveRepository = autoMoveRepository;
        }

        [JsonRpcMethod("automove.rules.create")]
        public Rule CreateRule(Rule rule)
        {
            _autoMoveRepository.CreateRule(rule);
            return rule;
        }

        [JsonRpcMethod("automove.parameters.create")]
        public Parameter CreateParameter(Parameter parameter)
        {
            _autoMoveRepository.CreateParameter(parameter);
            return parameter;
        }

        [JsonRpcMethod("automove.rules.delete")]
        public void DeleteRule(int ruleId)
        {
            _autoMoveRepository.DeleteRule(ruleId);
        }

        [JsonRpcMethod("automove.parameters.delete")]
        public void DeleteParameter(int parameterId)
        {
            _autoMoveRepository.DeleteParameter(parameterId);
        }

        [JsonRpcMethod("automove.rules.update")]
        public void UpdateRule(Rule rule)
        {
            _autoMoveRepository.UpdateRule(rule);
        }

        [JsonRpcMethod("automove.parameters.update")]
        public void UpdateParameter(Parameter parameter)
        {
            _autoMoveRepository.UpdateParameter(parameter);
        }

        [JsonRpcMethod("automove.parameters.getByRuleId")]
        public IEnumerable<Parameter> GetByRuleId(int ruleId)
        {
            return _autoMoveRepository.GetParametersByRuleId(ruleId);
        }
    }
}
