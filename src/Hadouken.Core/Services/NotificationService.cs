using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hadouken.Common.Extensibility;
using Hadouken.Common.Extensibility.Notifications;
using Hadouken.Common.JsonRpc;
using Hadouken.Common.Logging;
using Hadouken.Core.Data;
using Hadouken.Core.Services.Models;
using Hadouken.Localization;

namespace Hadouken.Core.Services
{
    public sealed class NotificationService : IJsonRpcService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly INotifierEngine _notifierEngine;
        private readonly INotifierRepository _notifierRepository;

        public NotificationService(ILogger<NotificationService> logger,
            INotifierEngine notifierEngine,
            INotifierRepository notifierRepository)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (notifierEngine == null) throw new ArgumentNullException("notifierEngine");
            if (notifierRepository == null) throw new ArgumentNullException("notifierRepository");
            _logger = logger;
            _notifierEngine = notifierEngine;
            _notifierRepository = notifierRepository;
        }

        [JsonRpcMethod("notifiers.getAll")]
        public IEnumerable<Notifier> GetNotifiers()
        {
            var notifiers = _notifierEngine.GetAll();

            return (from n in notifiers
                let attr = n.GetType().GetCustomAttribute<ExtensionAttribute>()
                select new Notifier
                {
                    Id = attr.ExtensionId,
                    Name = attr.Name,
                    Description = attr.Description,
                    CanNotify = n.CanNotify(),
                    RegisteredTypes = _notifierRepository.GetTypesForNotifier(attr.ExtensionId)
                });
        }

        [JsonRpcMethod("notifiers.getAllTypes")]
        public IEnumerable<string> GetAllTypes()
        {
            return Enum.GetNames(typeof (NotificationType)).Where(n => n != NotificationType.Test.ToString());
        }
        
        [JsonRpcMethod("notifiers.test")]
        public void TestNotifier(string notifierId)
        {
            var notifier = (from n in _notifierEngine.GetAll()
                where n.GetId() == notifierId
                select n).FirstOrDefault();

            if (notifier == null)
            {
                _logger.Warn("Unknown notifier: {NotifierId}.", notifierId);
                return;
            }

            if (!notifier.CanNotify())
            {
                _logger.Warn("Notifier is not configured: {NotifierId}.", notifierId);
                return;
            }

            try
            {
                notifier.Notify(new Notification(NotificationType.Test,
                    Notifications.TestTitle,
                    Notifications.TestMessage));
            }
            catch (Exception exception)
            {
                _logger.Error(exception, "Error in notifier: {NotifierId}.", notifierId);
            }
        }

        [JsonRpcMethod("notifiers.register")]
        public void RegisterNotifier(string notifierId, NotificationType type)
        {
            _notifierRepository.RegisterNotifier(notifierId, type);
        }

        [JsonRpcMethod("notifiers.unregister")]
        public void UnregisterNotifier(string notifierId, NotificationType type)
        {
            _notifierRepository.UnregisterNotifier(notifierId, type);
        }
    }
}
