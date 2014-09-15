using System;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Extensibility;
using Hadouken.Common.Messaging;
using Hadouken.Extensions.AutoMove.Data;

namespace Hadouken.Extensions.AutoMove
{
    [Extension("plugin.automove",
        Name = "AutoMove",
        Description = "Automatically moves torrents according to user-specified rules.",
        ResourceNamespace = "Hadouken.Extensions.AutoMove.Resources",
        Scripts = new[]
        {
            "js/app.js",
            "js/controllers/settingsController.js",
            "js/controllers/upsertRuleController.js"
        }
    )]
    public class AutoMovePlugin : IPlugin
    {
        private readonly IMessageBus _messageBus;
        private readonly IAutoMoveRepository _autoMoveRepository;
        private readonly IRuleFinder _ruleFinder;
        private readonly IParameterValueReplacer _parameterValueReplacer;

        public AutoMovePlugin(IMessageBus messageBus,
            IAutoMoveRepository autoMoveRepository,
            IRuleFinder ruleFinder,
            IParameterValueReplacer parameterValueReplacer)
        {
            if (messageBus == null) throw new ArgumentNullException("messageBus");
            if (autoMoveRepository == null) throw new ArgumentNullException("autoMoveRepository");
            if (ruleFinder == null) throw new ArgumentNullException("ruleFinder");
            if (parameterValueReplacer == null) throw new ArgumentNullException("parameterValueReplacer");

            _messageBus = messageBus;
            _autoMoveRepository = autoMoveRepository;
            _ruleFinder = ruleFinder;
            _parameterValueReplacer = parameterValueReplacer;
        }

        public void Load()
        {
            _messageBus.Subscribe<TorrentCompletedMessage>(OnTorrentCompleted);
        }

        public void Unload()
        {
            _messageBus.Unsubscribe<TorrentCompletedMessage>(OnTorrentCompleted);
        }

        private void OnTorrentCompleted(TorrentCompletedMessage message)
        {
            var rule = _ruleFinder.FindRule(message.Torrent);
            if (rule == null) return;

            var parameters = _autoMoveRepository.GetParametersByRuleId(rule.Id);
            var targetPath = _parameterValueReplacer.Replace(message.Torrent, parameters, rule.TargetPath);

            _messageBus.Publish(new MoveTorrentMessage(message.Torrent.InfoHash, targetPath));
        }
    }
}
