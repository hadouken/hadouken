#ifndef HADOUKEN_BITTORRENT_TORRENTSTATUS_HPP
#define HADOUKEN_BITTORRENT_TORRENTSTATUS_HPP

#include <Hadouken/Config.hpp>
#include <libtorrent/torrent_handle.hpp>
#include <Poco/Timespan.h>
#include <Poco/Timestamp.h>

namespace libtorrent
{
    struct torrent_status;
}

namespace Hadouken
{
    namespace BitTorrent
    {
        struct TorrentHandle;

        struct TorrentStatus
        {
            enum State
            {
                QueuedForChecking,
                CheckingFiles,
                DownloadingMetadata,
                Downloading,
                Finished,
                Seeding,
                Allocating,
                CheckingResumeData
            };

            TorrentStatus(const libtorrent::torrent_status& status);

            HDKN_EXPORT int getActiveTime() const;

            HDKN_EXPORT time_t getAddedTime() const;

            HDKN_EXPORT uint64_t getAllTimeDownload() const;

            HDKN_EXPORT uint64_t getAllTimeUpload() const;

            HDKN_EXPORT Poco::Timespan getAnnounceInterval() const;

            HDKN_EXPORT int getBlockSize() const;

            HDKN_EXPORT Poco::Timestamp getCompletedTime() const;

            HDKN_EXPORT int getConnectionsLimit() const;

            HDKN_EXPORT int getConnectCandidates() const;

            HDKN_EXPORT std::string getCurrentTracker() const;

            HDKN_EXPORT float getDistributedCopies() const;

            HDKN_EXPORT int getDownloadPayloadRate() const;

            HDKN_EXPORT int getDownloadRate() const;

            HDKN_EXPORT int getDownBandwidthQueue() const;

            HDKN_EXPORT std::string getError() const;

            HDKN_EXPORT Poco::Timestamp getFinishedTime() const;

            HDKN_EXPORT TorrentHandle getHandle() const;

            HDKN_EXPORT bool hasIncoming() const;

            HDKN_EXPORT bool hasMetadata() const;

            HDKN_EXPORT std::string getInfoHash() const;

            HDKN_EXPORT bool getIpFilterApplies() const;

            HDKN_EXPORT Poco::Timespan getLastScrape() const;

            HDKN_EXPORT Poco::Timestamp getLastSeenComplete() const;

            HDKN_EXPORT int getListPeers() const;

            HDKN_EXPORT int getListSeeds() const;

            HDKN_EXPORT std::string getName() const;

            HDKN_EXPORT bool getNeedSaveResume() const;

            HDKN_EXPORT Poco::Timespan getNextAnnounce() const;

            HDKN_EXPORT int getNumComplete() const;

            HDKN_EXPORT int getNumConnections() const;

            HDKN_EXPORT int getNumIncomplete() const;

            HDKN_EXPORT int getNumPeers() const;

            HDKN_EXPORT int getNumPieces() const;

            HDKN_EXPORT int getNumSeeds() const;

            HDKN_EXPORT int getNumUploads() const;

            // TODO Bitfield getPieces const;

            HDKN_EXPORT int getPriority() const;

            HDKN_EXPORT float getProgress() const;

            HDKN_EXPORT int getQueuePosition() const;

            HDKN_EXPORT std::string getSavePath() const;

            HDKN_EXPORT Poco::Timespan getSeedingTime() const;

            // TODO seed mode

            HDKN_EXPORT int getSeedRank() const;

            // TODO share mode

            HDKN_EXPORT int getSparseRegions() const;

            HDKN_EXPORT State getState() const;

            HDKN_EXPORT uint64_t getTotalDownload() const;

            HDKN_EXPORT uint64_t getTotalUpload() const;

            HDKN_EXPORT int getUploadRate() const;

            HDKN_EXPORT bool isAutoManaged() const;

            HDKN_EXPORT bool isFinished() const;

            HDKN_EXPORT bool isMovingStorage() const;

            HDKN_EXPORT bool isPaused() const;

            HDKN_EXPORT bool isSeeding() const;

            HDKN_EXPORT bool isSequentialDownload() const;

        private:
            const libtorrent::torrent_status status_;
        };
    }
}

#endif
