#ifndef HADOUKEN_BITTORRENT_SESSIONSTATUS_HPP
#define HADOUKEN_BITTORRENT_SESSIONSTATUS_HPP

#include <memory>
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
            ~SessionStatus();

            // TODO: active_requests

            int getAllowedUploadSlots() const;

            int getDhtDownloadRate() const;

            uint64_t getDhtGlobalNodes() const;

            int getDhtNodes() const;

            int getDhtNodeCache() const;

            // TODO: dht routing table

            int getDhtTorrents() const;

            int getDhtTotalAllocations() const;

            int getDhtUploadRate() const;

            int getDiskReadQueue() const;

            int getDiskWriteQueue() const;

            int getDownloadRate() const;

            int getIpOverheadDownloadRate() const;

            int getIpOverheadUploadRate() const;

            int getNumPeers() const;

            int getNumUnchoked() const;

            int getPayloadDownloadRate() const;

            int getPayloadUploadRate() const;

            int getTrackerDownloadRate() const;

            int getTrackerUploadRate() const;

            uint64_t getTotalDhtDownload() const;

            uint64_t getTotalDhtUpload() const;

            uint64_t getTotalDownload() const;

            uint64_t getTotalFailedBytes() const;

            uint64_t getTotalIpOverheadDownload() const;

            uint64_t getTotalIpOverheadUpload() const;

            uint64_t getTotalPayloadDownload() const;

            uint64_t getTotalPayloadUpload() const;

            uint64_t getTotalRedundantBytes() const;

            uint64_t getTotalTrackerDownload() const;

            uint64_t getTotalTrackerUpload() const;

            uint64_t getTotalUpload() const;

            int getDownBandwidthBytesQueue() const;

            int getDownBandwidthQueue() const;

            int getUpBandwidthBytesQueue() const;

            int getUpBandwidthQueue() const;

            int getUploadRate() const;

            int getOptimisticUnchokeCounter() const;

            int getUnchokeCounter() const;

            bool hasIncomingConnections() const;

        private:
            std::unique_ptr<libtorrent::session_status> status_;
        };
    }
}

#endif
