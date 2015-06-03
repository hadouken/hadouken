using System;
using System.Collections.Generic;
using System.Linq;
using Hadouken.Common.Data;
using Hadouken.Common.Extensibility;
using Hadouken.Extensions.AutoMove.Data.Models;

namespace Hadouken.Extensions.AutoMove.Data {
    [Component]
    public class AutoMoveRepository : IAutoMoveRepository {
        private readonly IDbConnection _dbConnection;

        public AutoMoveRepository(IDbConnection dbConnection) {
            if (dbConnection == null) {
                throw new ArgumentNullException("dbConnection");
            }
            this._dbConnection = dbConnection;
        }

        public void CreateRule(Rule rule) {
            const string query = "insert into AutoMove_Rule (Name, TargetPath) values (@Name, @TargetPath); select last_insert_rowid();";
            rule.Id = this._dbConnection.Query<int>(query, rule).First();
        }

        public void CreateParameter(Parameter parameter) {
            const string query = "insert into AutoMove_Parameter (RuleId, Source, Pattern) values (@RuleId, @Source, @Pattern); select last_insert_rowid();";
            parameter.Id = this._dbConnection.Query<int>(query, parameter).First();
        }

        public void DeleteRule(int ruleId) {
            const string query = "delete from AutoMove_Rule where Id = @Id";
            this._dbConnection.Execute(query, new {Id = ruleId});
        }

        public void DeleteParameter(int parameterId) {
            const string query = "delete from AutoMove_Parameter where Id = @Id";
            this._dbConnection.Execute(query, new {Id = parameterId});
        }

        public IEnumerable<Rule> GetRules() {
            var query = "select r.Id, r.Name, r.TargetPath from AutoMove_Rule r";
            return this._dbConnection.Query<Rule>(query);
        }

        public IEnumerable<Parameter> GetParametersByRuleId(int ruleId) {
            const string query = "select p.Id, p.RuleId, p.Source, p.Pattern from AutoMove_Parameter p where p.RuleId = @RuleId";
            return this._dbConnection.Query<Parameter>(query, new {RuleId = ruleId});
        }

        public void UpdateRule(Rule rule) {
            const string query = "update AutoMove_Rule set Name = @Name, TargetPath = @TargetPath where Id = @Id";
            this._dbConnection.Execute(query, rule);
        }

        public void UpdateParameter(Parameter parameter) {
            const string query = "update AutoMove_Parameter set Source = @Source, Pattern = @Pattern where Id = @Id";
            this._dbConnection.Execute(query, parameter);
        }
    }
}