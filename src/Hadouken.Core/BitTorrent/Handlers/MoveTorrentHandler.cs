using System;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Messaging;
using Ragnar;

namespace Hadouken.Core.BitTorrent.Handlers
{
    internal sealed class MoveTorrentHandler : IMessageHandler<MoveTorrentMessage>
    {
        private readonly ISession _session;

        public MoveTorrentHandler(ISession session)
        {
            if (session == null) throw new ArgumentNullException("session");
            _session = session;
        }

        public void Handle(MoveTorrentMessage message)
        {
            using (var handle = _session.FindTorrent(message.InfoHash))
            {
                if (handle == null) return;

                var flags = message.OverwriteExisting
                    ? TorrentHandle.MoveFlags.AlwaysReplaceFiles
                    : TorrentHandle.MoveFlags.DontReplace;

                handle.MoveStorage(message.Destination, flags);
            }
        }
    }
}
