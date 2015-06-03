using System;
using System.Collections.Generic;
using Hadouken.Common.JsonRpc;
using Hadouken.Extensions.AutoMove.Data;
using Hadouken.Extensions.AutoMove.Data.Models;

namespace Hadouken.Extensions.AutoMove.Services {
    public sealed class AutoMoveService : IJsonRpcService {
        private readonly IAutoMoveRepository _autoMoveRepository;

        public AutoMoveService(IAutoMoveRepository autoMoveRepository) {
            if (autoMoveRepository == null) {
                throw new ArgumentNullException("autoMoveRepository");
            }
            this._autoMoveRepository = autoMoveRepository;
        }

        [JsonRpcMethod("automove.rules.create")]
        public Rule CreateRule(Rule rule) {
            if (rule == null) {
                throw new ArgumentNullException("rule");
            }

            this._autoMoveRepository.CreateRule(rule);
            return rule;
        }

        [JsonRpcMethod("automove.parameters.create")]
        public Parameter CreateParameter(Parameter parameter) {
            if (parameter == null) {
                throw new ArgumentNullException("parameter");
            }

            this._autoMoveRepository.CreateParameter(parameter);
            return parameter;
        }

        [JsonRpcMethod("automove.rules.delete")]
        public void DeleteRule(int ruleId) {
            this._autoMoveRepository.DeleteRule(ruleId);
        }

        [JsonRpcMethod("automove.parameters.delete")]
        public void DeleteParameter(int parameterId) {
            this._autoMoveRepository.DeleteParameter(parameterId);
        }

        [JsonRpcMethod("automove.rules.update")]
        public void UpdateRule(Rule rule) {
            this._autoMoveRepository.UpdateRule(rule);
        }

        [JsonRpcMethod("automove.parameters.update")]
        public void UpdateParameter(Parameter parameter) {
            this._autoMoveRepository.UpdateParameter(parameter);
        }

        [JsonRpcMethod("automove.parameters.getByRuleId")]
        public IEnumerable<Parameter> GetByRuleId(int ruleId) {
            return this._autoMoveRepository.GetParametersByRuleId(ruleId);
        }

        [JsonRpcMethod("automove.rules.getAll")]
        public IEnumerable<Rule> GetRules() {
            return this._autoMoveRepository.GetRules();
        }
    }
}