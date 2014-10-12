using System;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Messaging;
using Ragnar;

namespace Hadouken.Core.BitTorrent.Handlers
{
    internal class RemoveTorrentHandler : IMessageHandler<RemoveTorrentMessage>
    {
        private readonly ISession _session;
        private readonly ITorrentInfoRepository _torrentInfoRepository;

        public RemoveTorrentHandler(ISession session,
            ITorrentInfoRepository torrentInfoRepository)
        {
            if (session == null) throw new ArgumentNullException("session");
            if (torrentInfoRepository == null) throw new ArgumentNullException("torrentInfoRepository");
            _session = session;
            _torrentInfoRepository = torrentInfoRepository;
        }

        public void Handle(RemoveTorrentMessage message)
        {
            using (var handle = _session.FindTorrent(message.InfoHash))
            {
                if (handle == null) return;
                _session.RemoveTorrent(handle, message.RemoveData);
                _torrentInfoRepository.Remove(message.InfoHash);
            }
        }
    }
}
