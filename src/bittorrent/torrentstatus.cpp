#include <Hadouken/BitTorrent/TorrentStatus.hpp>

#include <Hadouken/BitTorrent/TorrentHandle.hpp>
#include <libtorrent/torrent_handle.hpp>

using namespace Hadouken::BitTorrent;

TorrentStatus::TorrentStatus(const libtorrent::torrent_status& status)
    : status_(status)
{
}

int TorrentStatus::getActiveTime() const
{
    return status_.active_time;
}

time_t TorrentStatus::getAddedTime() const
{
    return status_.added_time;
}

uint64_t TorrentStatus::getAllTimeDownload() const
{
    return status_.all_time_download;
}

uint64_t TorrentStatus::getAllTimeUpload() const
{
    return status_.all_time_upload;
}

Poco::Timespan TorrentStatus::getAnnounceInterval() const
{
    return Poco::Timespan(status_.announce_interval.total_microseconds());
}

int TorrentStatus::getBlockSize() const
{
    return status_.block_size;
}

Poco::Timestamp TorrentStatus::getCompletedTime() const
{
    return Poco::Timestamp::fromEpochTime(status_.completed_time);
}

int TorrentStatus::getConnectionsLimit() const
{
    return status_.connections_limit;
}

int TorrentStatus::getConnectCandidates() const
{
    return status_.connect_candidates;
}

std::string TorrentStatus::getCurrentTracker() const
{
    return status_.current_tracker;
}

float TorrentStatus::getDistributedCopies() const
{
    return status_.distributed_copies;
}

int TorrentStatus::getDownloadPayloadRate() const
{
    return status_.download_payload_rate;
}

int TorrentStatus::getDownloadRate() const
{
    return status_.download_rate;
}

int TorrentStatus::getDownBandwidthQueue() const
{
    return status_.down_bandwidth_queue;
}

std::string TorrentStatus::getError() const
{
    return status_.error;
}

Poco::Timestamp TorrentStatus::getFinishedTime() const
{
    return Poco::Timestamp::fromEpochTime(status_.finished_time);
}

TorrentHandle TorrentStatus::getHandle() const
{
    return TorrentHandle(status_.handle);
}

bool TorrentStatus::hasIncoming() const
{
    return status_.has_incoming;
}

bool TorrentStatus::hasMetadata() const
{
    return status_.has_metadata;
}

std::string TorrentStatus::getInfoHash() const
{
    return libtorrent::to_hex(status_.info_hash.to_string());
}

bool TorrentStatus::getIpFilterApplies() const
{
    return status_.ip_filter_applies;
}

Poco::Timespan TorrentStatus::getLastScrape() const
{
    return Poco::Timespan(status_.last_scrape, 0);
}

Poco::Timestamp TorrentStatus::getLastSeenComplete() const
{
    return Poco::Timestamp::fromEpochTime(status_.last_seen_complete);
}

int TorrentStatus::getListPeers() const
{
    return status_.list_peers;
}

int TorrentStatus::getListSeeds() const
{
    return status_.list_seeds;
}

std::string TorrentStatus::getName() const
{
    return status_.name;
}

bool TorrentStatus::getNeedSaveResume() const
{
    return status_.need_save_resume;
}

Poco::Timespan TorrentStatus::getNextAnnounce() const
{
    return Poco::Timespan(status_.next_announce.total_microseconds());
}

int TorrentStatus::getNumComplete() const
{
    return status_.num_complete;
}

int TorrentStatus::getNumConnections() const
{
    return status_.num_connections;
}

int TorrentStatus::getNumIncomplete() const
{
    return status_.num_incomplete;
}

int TorrentStatus::getNumPeers() const
{
    return status_.num_peers;
}

int TorrentStatus::getNumPieces() const
{
    return status_.num_pieces;
}

int TorrentStatus::getNumSeeds() const
{
    return status_.num_seeds;
}

int TorrentStatus::getNumUploads() const
{
    return status_.num_uploads;
}

int TorrentStatus::getPriority() const
{
    return status_.priority;
}

float TorrentStatus::getProgress() const
{
    return status_.progress;
}

int TorrentStatus::getQueuePosition() const
{
    return status_.queue_position;
}

std::string TorrentStatus::getSavePath() const
{
    return status_.save_path;
}

Poco::Timespan TorrentStatus::getSeedingTime() const
{
    return Poco::Timespan(status_.seeding_time, 0);
}

int TorrentStatus::getSeedRank() const
{
    return status_.seed_rank;
}

int TorrentStatus::getSparseRegions() const
{
    return status_.sparse_regions;
}

TorrentStatus::State TorrentStatus::getState() const
{
    return (TorrentStatus::State)(int)status_.state;
}

uint64_t TorrentStatus::getTotalDownload() const
{
    return status_.total_download;
}

uint64_t TorrentStatus::getTotalUpload() const
{
    return status_.total_upload;
}

int TorrentStatus::getUploadRate() const
{
    return status_.upload_rate;
}

bool TorrentStatus::isAutoManaged() const
{
    return status_.auto_managed;
}

bool TorrentStatus::isFinished() const
{
    return status_.is_finished;
}

bool TorrentStatus::isMovingStorage() const
{
    return status_.moving_storage;
}

bool TorrentStatus::isPaused() const
{
    return status_.paused;
}

bool TorrentStatus::isSeeding() const
{
    return status_.is_seeding;
}

bool TorrentStatus::isSequentialDownload() const
{
    return status_.sequential_download;
}
