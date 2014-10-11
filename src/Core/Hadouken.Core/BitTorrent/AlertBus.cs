using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Ragnar;

namespace Hadouken.Core.BitTorrent
{
    internal sealed class AlertBus : IAlertBus
    {
        private readonly IAlertFactory _alertFactory;
        private readonly IDictionary<Type, IList<object>> _callbacks;
        private readonly Thread _readThread;
        private bool _isReading;

        public AlertBus(IAlertFactory alertFactory)
        {
            if (alertFactory == null) throw new ArgumentNullException("alertFactory");
            _alertFactory = alertFactory;
            _callbacks = new Dictionary<Type, IList<object>>();
            _readThread = new Thread(ReadAlerts);
        }

        public void StartRead()
        {
            if (_isReading) return;

            _isReading = true;
            _readThread.Start();
        }

        public void StopRead()
        {
            if (!_isReading) return;

            _isReading = false;
            _readThread.Join();
        }

        public void Subscribe<TAlert>(Action<TAlert> callback) where TAlert : Alert
        {
            var t = typeof (TAlert);

            // Add empty list
            if (!_callbacks.ContainsKey(t)) _callbacks.Add(t, new List<object>());

            _callbacks[t].Add(callback);
        }

        private void ReadAlerts()
        {
            var timeout = TimeSpan.FromSeconds(1);
            var flags = (BindingFlags.Instance | BindingFlags.NonPublic);

            while (_isReading)
            {
                var foundAlerts = _alertFactory.PeekWait(timeout);
                if (!foundAlerts) continue;

                var alerts = _alertFactory.PopAll();

                foreach (var alert in alerts)
                {
                    var t = alert.GetType();
                    this.GetType().GetMethod("Publish", flags).MakeGenericMethod(t).Invoke(this, new object[] {alert});
                }
            }
        }

// ReSharper disable once UnusedMember.Local
        private void Publish<TAlert>(TAlert alert) where TAlert : Alert
        {
            var t = typeof (TAlert);
            if (!_callbacks.ContainsKey(t)) return;

            foreach (var callback in _callbacks[t].OfType<Action<TAlert>>())
            {
                callback(alert);
            }
        }
    }
}