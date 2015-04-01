#ifndef HADOUKEN_BITTORRENT_SESSIONSTATUS_HPP
#define HADOUKEN_BITTORRENT_SESSIONSTATUS_HPP

#include <stdint.h>

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

            int getDhtNodeCache() const;

            // TODO: dht routing table

            int getDhtTorrents() const;

            int getDhtTotalAllocations() const;

            int getDhtUploadRate() const;

            int getDiskReadQueue() const;

            int getDiskWriteQueue() const;

            int getDownloadRate() const;

            uint64_t getTotalDownload() const;

            uint64_t getTotalUpload() const;

            int getUploadRate() const;

            bool hasIncomingConnections() const;

        private:
            const libtorrent::session_status& status_;
        };
    }
}

#endif
