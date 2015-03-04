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

bool SessionStatus::hasIncomingConnections() const
{
    return status_.has_incoming_connections;
}
