using System.Collections.Generic;
using Hadouken.Extensions.AutoMove.Data.Models;

namespace Hadouken.Extensions.AutoMove.Data {
    public interface IAutoMoveRepository {
        void CreateRule(Rule rule);
        void CreateParameter(Parameter parameter);
        void DeleteRule(int ruleId);
        void DeleteParameter(int parameterId);
        IEnumerable<Rule> GetRules();
        IEnumerable<Parameter> GetParametersByRuleId(int ruleId);
        void UpdateRule(Rule rule);
        void UpdateParameter(Parameter parameter);
    }
}