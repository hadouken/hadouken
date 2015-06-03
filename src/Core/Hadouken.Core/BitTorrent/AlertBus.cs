using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Ragnar;

namespace Hadouken.Core.BitTorrent {
    internal sealed class AlertBus : IAlertBus {
        private readonly IAlertFactory _alertFactory;
        private readonly IDictionary<Type, IList<object>> _callbacks;
        private readonly Thread _readThread;
        private bool _isReading;

        public AlertBus(IAlertFactory alertFactory) {
            if (alertFactory == null) {
                throw new ArgumentNullException("alertFactory");
            }
            this._alertFactory = alertFactory;
            this._callbacks = new Dictionary<Type, IList<object>>();
            this._readThread = new Thread(this.ReadAlerts);
        }

        public void StartRead() {
            if (this._isReading) {
                return;
            }

            this._isReading = true;
            this._readThread.Start();
        }

        public void StopRead() {
            if (!this._isReading) {
                return;
            }

            this._isReading = false;
            this._readThread.Join();
        }

        public void Subscribe<TAlert>(Action<TAlert> callback) where TAlert : Alert {
            var t = typeof (TAlert);

            // Add empty list
            if (!this._callbacks.ContainsKey(t)) {
                this._callbacks.Add(t, new List<object>());
            }

            this._callbacks[t].Add(callback);
        }

        private void ReadAlerts() {
            var timeout = TimeSpan.FromSeconds(1);
            const BindingFlags flags = (BindingFlags.Instance | BindingFlags.NonPublic);

            while (this._isReading) {
                var foundAlerts = this._alertFactory.PeekWait(timeout);
                if (!foundAlerts) {
                    continue;
                }

                var alerts = this._alertFactory.PopAll();

                foreach (var alert in alerts) {
                    var t = alert.GetType();
                    this.GetType().GetMethod("Publish", flags).MakeGenericMethod(t).Invoke(this, new object[] {alert});
                }
            }
        }

// ReSharper disable once UnusedMember.Local
        private void Publish<TAlert>(TAlert alert) where TAlert : Alert {
            var t = typeof (TAlert);
            if (!this._callbacks.ContainsKey(t)) {
                return;
            }

            foreach (var callback in this._callbacks[t].OfType<Action<TAlert>>()) {
                callback(alert);
            }
        }
    }
}