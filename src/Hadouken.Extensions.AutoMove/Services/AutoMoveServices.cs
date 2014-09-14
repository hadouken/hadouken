using System;
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
    }
}
