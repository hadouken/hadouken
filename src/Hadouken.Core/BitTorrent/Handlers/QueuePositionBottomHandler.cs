using System;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Messaging;
using Ragnar;

namespace Hadouken.Core.BitTorrent.Handlers
{
    public sealed class QueuePositionBottomHandler : IMessageHandler<QueuePositionBottomMessage>
    {
        private readonly ISession _session;

        public QueuePositionBottomHandler(ISession session)
        {
            if (session == null) throw new ArgumentNullException("session");
            _session = session;
        }

        public void Handle(QueuePositionBottomMessage message)
        {
            using (var handle = _session.FindTorrent(message.InfoHash))
            {
                if (handle == null) return;
                handle.QueuePositionBottom();
            }
        }
    }
}
