#ifndef HADOUKEN_BITTORRENT_SESSIONSTATUS_HPP
#define HADOUKEN_BITTORRENT_SESSIONSTATUS_HPP

namespace libtorrent
{
    struct session_status;
}

namespace Hadouken
{
    namespace BitTorrent
    {
        struct SessionStatus
        {
            explicit SessionStatus(const libtorrent::session_status& status);

            // TODO: active_requests

            int getAllowedUploadSlots() const;

            int getDhtDownloadRate() const;

            // TODO: dht_global_nodes

            int getDhtNodes() const;

            bool hasIncomingConnections() const;

        private:
            const libtorrent::session_status& status_;
        };
    }
}

#endif
