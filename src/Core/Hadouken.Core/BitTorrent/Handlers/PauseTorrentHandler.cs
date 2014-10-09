using System;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Messaging;
using Ragnar;

namespace Hadouken.Core.BitTorrent.Handlers
{
    internal sealed class PauseTorrentHandler : IMessageHandler<PauseTorrentMessage>
    {
        private readonly ISession _session;

        public PauseTorrentHandler(ISession session)
        {
            if (session == null) throw new ArgumentNullException("session");
            _session = session;
        }

        public void Handle(PauseTorrentMessage message)
        {
            using (var handle = _session.FindTorrent(message.InfoHash))
            {
                if (handle == null) return;

                handle.AutoManaged = false;
                handle.Pause();
            }
        }
    }
}
