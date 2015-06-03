using System;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Extensibility;
using Hadouken.Common.Messaging;
using Hadouken.Extensions.AutoMove.Data;

namespace Hadouken.Extensions.AutoMove {
    [Extension("plugin.automove",
        Name = "AutoMove",
        Description = "Automatically moves torrents according to user-specified rules.",
        ResourceNamespace = "Hadouken.Extensions.AutoMove.Resources",
        Scripts = new[] {
            "js/app.js",
            "js/controllers/settingsController.js",
            "js/controllers/upsertRuleController.js"
        }
        )]
    public class AutoMovePlugin : IPlugin {
        private readonly IAutoMoveRepository _autoMoveRepository;
        private readonly IMessageBus _messageBus;
        private readonly IParameterValueReplacer _parameterValueReplacer;
        private readonly IRuleFinder _ruleFinder;

        public AutoMovePlugin(IMessageBus messageBus,
            IAutoMoveRepository autoMoveRepository,
            IRuleFinder ruleFinder,
            IParameterValueReplacer parameterValueReplacer) {
            if (messageBus == null) {
                throw new ArgumentNullException("messageBus");
            }
            if (autoMoveRepository == null) {
                throw new ArgumentNullException("autoMoveRepository");
            }
            if (ruleFinder == null) {
                throw new ArgumentNullException("ruleFinder");
            }
            if (parameterValueReplacer == null) {
                throw new ArgumentNullException("parameterValueReplacer");
            }

            this._messageBus = messageBus;
            this._autoMoveRepository = autoMoveRepository;
            this._ruleFinder = ruleFinder;
            this._parameterValueReplacer = parameterValueReplacer;
        }

        public void Load() {
            this._messageBus.Subscribe<TorrentCompletedMessage>(this.OnTorrentCompleted);
        }

        public void Unload() {
            this._messageBus.Unsubscribe<TorrentCompletedMessage>(this.OnTorrentCompleted);
        }

        private void OnTorrentCompleted(TorrentCompletedMessage message) {
            var rule = this._ruleFinder.FindRule(message.Torrent);
            if (rule == null) {
                return;
            }

            var parameters = this._autoMoveRepository.GetParametersByRuleId(rule.Id);
            var targetPath = this._parameterValueReplacer.Replace(message.Torrent, parameters, rule.TargetPath);

            this._messageBus.Publish(new MoveTorrentMessage(message.Torrent.InfoHash, targetPath));
        }
    }
}