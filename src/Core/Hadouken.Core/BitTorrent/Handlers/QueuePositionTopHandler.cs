using System;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Messaging;
using Ragnar;

namespace Hadouken.Core.BitTorrent.Handlers
{
    public sealed class QueuePositionTopHandler : IMessageHandler<QueuePositionTopMessage>
    {
        private readonly ISession _session;

        public QueuePositionTopHandler(ISession session)
        {
            if (session == null) throw new ArgumentNullException("session");
            _session = session;
        }

        public void Handle(QueuePositionTopMessage message)
        {
            using (var handle = _session.FindTorrent(message.InfoHash))
            {
                if (handle == null) return;
                handle.QueuePositionTop();
            }
        }
    }
}
