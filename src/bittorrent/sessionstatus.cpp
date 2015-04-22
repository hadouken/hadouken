#include <Hadouken/BitTorrent/SessionStatus.hpp>

#include <libtorrent/session_status.hpp>

using namespace Hadouken::BitTorrent;

SessionStatus::SessionStatus(const libtorrent::session_status& status):
    status_(new libtorrent::session_status(status))
{
}

SessionStatus::~SessionStatus()
{
}

int SessionStatus::getAllowedUploadSlots() const
{
    return status_->allowed_upload_slots;
}

int SessionStatus::getDhtDownloadRate() const
{
    return status_->dht_download_rate;
}

uint64_t SessionStatus::getDhtGlobalNodes() const
{
    return status_->dht_global_nodes;
}

int SessionStatus::getDhtNodes() const
{
    return status_->dht_nodes;
}

int SessionStatus::getDhtNodeCache() const
{
    return status_->dht_node_cache;
}

int SessionStatus::getDhtTorrents() const
{
    return status_->dht_torrents;
}

int SessionStatus::getDhtTotalAllocations() const
{
    return status_->dht_total_allocations;
}

int SessionStatus::getDhtUploadRate() const
{
    return status_->dht_upload_rate;
}

int SessionStatus::getDiskReadQueue() const
{
    return status_->disk_read_queue;
}

int SessionStatus::getDiskWriteQueue() const
{
    return status_->disk_write_queue;
}

int SessionStatus::getDownloadRate() const
{
    return status_->download_rate;
}

int SessionStatus::getIpOverheadDownloadRate() const
{
    return status_->ip_overhead_download_rate;
}

int SessionStatus::getIpOverheadUploadRate() const
{
    return status_->ip_overhead_upload_rate;
}

int SessionStatus::getNumPeers() const
{
    return status_->num_peers;
}

int SessionStatus::getNumUnchoked() const
{
    return status_->num_unchoked;
}

uint64_t SessionStatus::getTotalDownload() const
{
    return status_->total_download;
}

uint64_t SessionStatus::getTotalUpload() const
{
    return status_->total_upload;
}

int SessionStatus::getUploadRate() const
{
    return status_->upload_rate;
}

int SessionStatus::getPayloadDownloadRate() const
{
    return status_->payload_download_rate;
}

int SessionStatus::getPayloadUploadRate() const
{
    return status_->payload_upload_rate;
}

int SessionStatus::getTrackerDownloadRate() const
{
    return status_->tracker_download_rate;
}

int SessionStatus::getTrackerUploadRate() const
{
    return status_->tracker_upload_rate;
}

uint64_t SessionStatus::getTotalDhtDownload() const
{
    return status_->total_dht_download;
}

uint64_t SessionStatus::getTotalDhtUpload() const
{
    return status_->total_dht_upload;
}

uint64_t SessionStatus::getTotalFailedBytes() const
{
    return status_->total_failed_bytes;
}

uint64_t SessionStatus::getTotalIpOverheadDownload() const
{
    return status_->total_ip_overhead_download;
}

uint64_t SessionStatus::getTotalIpOverheadUpload() const
{
    return status_->total_ip_overhead_upload;
}

uint64_t SessionStatus::getTotalPayloadDownload() const
{
    return status_->total_payload_download;
}

uint64_t SessionStatus::getTotalPayloadUpload() const
{
    return status_->total_payload_upload;
}

uint64_t SessionStatus::getTotalRedundantBytes() const
{
    return status_->total_redundant_bytes;
}

uint64_t SessionStatus::getTotalTrackerDownload() const
{
    return status_->total_tracker_download;
}

uint64_t SessionStatus::getTotalTrackerUpload() const
{
    return status_->total_tracker_upload;
}

int SessionStatus::getDownBandwidthBytesQueue() const
{
    return status_->down_bandwidth_bytes_queue;
}

int SessionStatus::getDownBandwidthQueue() const
{
    return status_->down_bandwidth_queue;
}

int SessionStatus::getUpBandwidthBytesQueue() const
{
    return status_->up_bandwidth_bytes_queue;
}

int SessionStatus::getUpBandwidthQueue() const
{
    return status_->up_bandwidth_queue;
}

int SessionStatus::getOptimisticUnchokeCounter() const
{
    return status_->optimistic_unchoke_counter;
}

int SessionStatus::getUnchokeCounter() const
{
    return status_->unchoke_counter;
}

bool SessionStatus::hasIncomingConnections() const
{
    return status_->has_incoming_connections;
}
