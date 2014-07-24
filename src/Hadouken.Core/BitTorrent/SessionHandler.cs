using System;
using Ragnar;

namespace Hadouken.Core.BitTorrent
{
    public class SessionHandler : ISessionHandler
    {
        private readonly ISession _session;

        public SessionHandler(ISession session)
        {
            if (session == null) throw new ArgumentNullException("session");
            _session = session;
        }

        public void Load()
        {
            _session.ListenOn(6881, 6881);
        }

        public void Unload()
        {
        }
    }
}