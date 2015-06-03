using System;
using System.Collections.Generic;
using Hadouken.Common.Data;
using Hadouken.Common.Extensibility.Notifications;

namespace Hadouken.Core.Data {
    public sealed class NotifierRepository : INotifierRepository {
        private readonly IDbConnection _dbConnection;

        public NotifierRepository(IDbConnection dbConnection) {
            if (dbConnection == null) {
                throw new ArgumentNullException("dbConnection");
            }
            this._dbConnection = dbConnection;
        }

        public IEnumerable<string> GetNotifiersForType(NotificationType type) {
            const string query = "select n.NotifierId from Notifier n where n.NotificationType = @Type";
            return this._dbConnection.Query<string>(query, new {Type = type.ToString()});
        }

        public IEnumerable<string> GetTypesForNotifier(string notifierId) {
            const string query = "select n.NotificationType from Notifier n where n.NotifierId = @NotifierId";
            return this._dbConnection.Query<string>(query, new {NotifierId = notifierId});
        }

        public void RegisterNotifier(string notifierId, NotificationType type) {
            const string query = "insert into Notifier (NotifierId, NotificationType) values (@NotifierId, @Type);";
            this._dbConnection.Execute(query, new {NotifierId = notifierId, Type = type.ToString()});
        }

        public void UnregisterNotifier(string notifierId, NotificationType type) {
            const string query = "delete from Notifier where NotifierId = @NotifierId and NotificationType = @Type";
            this._dbConnection.Execute(query, new {NotifierId = notifierId, Type = type.ToString()});
        }
    }
}