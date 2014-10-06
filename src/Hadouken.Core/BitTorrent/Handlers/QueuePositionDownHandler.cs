using System;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Messaging;
using Ragnar;

namespace Hadouken.Core.BitTorrent.Handlers
{
    public sealed class QueuePositionDownHandler : IMessageHandler<QueuePositionDownMessage>
    {
        private readonly ISession _session;

        public QueuePositionDownHandler(ISession session)
        {
            if (session == null) throw new ArgumentNullException("session");
            _session = session;
        }

        public void Handle(QueuePositionDownMessage message)
        {
            using (var handle = _session.FindTorrent(message.InfoHash))
            {
                if (handle == null) return;
                handle.QueuePositionDown();
            }
        }
    }
}
