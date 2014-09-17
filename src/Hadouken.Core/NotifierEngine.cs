using System;
using System.Collections.Generic;
using Hadouken.Common.Extensibility;
using Hadouken.Common.Extensibility.Notifications;
using Hadouken.Common.Logging;
using Hadouken.Core.Data;

namespace Hadouken.Core
{
    public class NotifierEngine : INotifierEngine
    {
        private readonly ILogger<NotifierEngine> _logger;
        private readonly INotifierRepository _notifierRepository;
        private readonly IList<INotifier> _notifiers;

        public NotifierEngine(ILogger<NotifierEngine> logger,
            INotifierRepository notifierRepository,
            IEnumerable<INotifier> notifiers)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (notifierRepository == null) throw new ArgumentNullException("notifierRepository");
            if (notifiers == null) throw new ArgumentNullException("notifiers");
            _logger = logger;
            _notifierRepository = notifierRepository;
            _notifiers = new List<INotifier>(notifiers);
        }

        public IEnumerable<INotifier> GetAll()
        {
            return new List<INotifier>(_notifiers);
        } 

        public void NotifyAll(Notification notification)
        {
            var notifierIds = new List<string>(_notifierRepository.GetNotifiersForType(notification.Type) ?? new string[] {});

            foreach (var notifier in _notifiers)
            {
                if (!notifierIds.Contains(notifier.GetId())) continue;
                if (!notifier.CanNotify()) continue;

                try
                {
                    notifier.Notify(notification);
                }
                catch (Exception exception)
                {
                    _logger.Error(
                        exception,
                        "Error when sending notification. Notifier: {NotifierId}.",
                        notifier.GetId());
                }
            }
        }
    }
}