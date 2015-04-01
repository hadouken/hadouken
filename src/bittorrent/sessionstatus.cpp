#include <Hadouken/BitTorrent/SessionStatus.hpp>

#include <libtorrent/session_status.hpp>

using namespace Hadouken::BitTorrent;

SessionStatus::SessionStatus(const libtorrent::session_status& status)
    : status_(status)
{
}

int SessionStatus::getAllowedUploadSlots() const
{
    return status_.allowed_upload_slots;
}

int SessionStatus::getDhtDownloadRate() const
{
    return status_.dht_download_rate;
}

int SessionStatus::getDhtNodes() const
{
    return status_.dht_nodes;
}

int SessionStatus::getDhtNodeCache() const
{
    return status_.dht_node_cache;
}

int SessionStatus::getDhtTorrents() const
{
    return status_.dht_torrents;
}

int SessionStatus::getDhtTotalAllocations() const
{
    return status_.dht_total_allocations;
}

int SessionStatus::getDhtUploadRate() const
{
    return status_.dht_upload_rate;
}

int SessionStatus::getDiskReadQueue() const
{
    return status_.disk_read_queue;
}

int SessionStatus::getDiskWriteQueue() const
{
    return status_.disk_write_queue;
}

int SessionStatus::getDownloadRate() const
{
    return status_.download_rate;
}

uint64_t SessionStatus::getTotalDownload() const
{
    return status_.total_download;
}

uint64_t SessionStatus::getTotalUpload() const
{
    return status_.total_upload;
}

int SessionStatus::getUploadRate() const
{
    return status_.upload_rate;
}

bool SessionStatus::hasIncomingConnections() const
{
    return status_.has_incoming_connections;
}
