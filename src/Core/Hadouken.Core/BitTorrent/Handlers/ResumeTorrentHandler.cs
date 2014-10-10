using System;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Messaging;
using Ragnar;

namespace Hadouken.Core.BitTorrent.Handlers
{
    internal sealed class ResumeTorrentHandler : IMessageHandler<ResumeTorrentMessage>
    {
        private readonly ISession _session;

        public ResumeTorrentHandler(ISession session)
        {
            if (session == null) throw new ArgumentNullException("session");
            _session = session;
        }

        public void Handle(ResumeTorrentMessage message)
        {
            using (var handle = _session.FindTorrent(message.InfoHash))
            {
                if (handle == null) return;

                handle.AutoManaged = true;

                using (var status = handle.QueryStatus())
                {
                    if (status.Paused)
                    {
                        handle.Resume();
                    }
                }
            }
        }
    }
}
