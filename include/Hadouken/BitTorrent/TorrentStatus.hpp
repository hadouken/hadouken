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

            int getActiveTime() const;

            time_t getAddedTime() const;

            uint64_t getAllTimeDownload() const;

            uint64_t getAllTimeUpload() const;

            Poco::Timespan getAnnounceInterval() const;

            int getBlockSize() const;

            Poco::Timestamp getCompletedTime() const;

            int getConnectionsLimit() const;

            int getConnectCandidates() const;

            std::string getCurrentTracker() const;

            float getDistributedCopies() const;

            int getDownloadPayloadRate() const;

            int getDownloadRate() const;

            int getDownBandwidthQueue() const;

            std::string getError() const;

            Poco::Timestamp getFinishedTime() const;

            TorrentHandle getHandle() const;

            bool hasIncoming() const;

            bool hasMetadata() const;

            std::string getInfoHash() const;

            bool getIpFilterApplies() const;

            Poco::Timespan getLastScrape() const;

            Poco::Timestamp getLastSeenComplete() const;

            int getListPeers() const;

            int getListSeeds() const;

            HDKN_EXPORT std::string getName() const;

            bool getNeedSaveResume() const;

            Poco::Timespan getNextAnnounce() const;

            int getNumComplete() const;

            int getNumConnections() const;

            int getNumIncomplete() const;

            int getNumPeers() const;

            int getNumPieces() const;

            int getNumSeeds() const;

            int getNumUploads() const;

            // TODO Bitfield getPieces const;

            int getPriority() const;

            float getProgress() const;

            int getQueuePosition() const;

            std::string getSavePath() const;

            Poco::Timespan getSeedingTime() const;

            // TODO seed mode

            int getSeedRank() const;

            // TODO share mode

            int getSparseRegions() const;

            State getState() const;

            uint64_t getTotalDownload() const;

            uint64_t getTotalUpload() const;

            int getUploadRate() const;

            bool isAutoManaged() const;

            bool isFinished() const;

            bool isMovingStorage() const;

            bool isPaused() const;

            bool isSeeding() const;

            bool isSequentialDownload() const;

        private:
            const libtorrent::torrent_status status_;
        };
    }
}

#endif
