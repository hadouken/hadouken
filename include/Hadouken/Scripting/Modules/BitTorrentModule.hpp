#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENTMODULE_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENTMODULE_HPP

#include <memory>

namespace Hadouken
{
    namespace BitTorrent
    {
        struct PeerInfo;
        class Session;
        class TorrentInfo;
        struct TorrentHandle;
        struct TorrentStatus;
    }
}

namespace Hadouken
{
    namespace Scripting
    {
        namespace Modules
        {
            class BitTorrentModule
            {
            public:
                static int initialize(void* ctx);

            private:
                static void handle_initialize(void* ctx, std::shared_ptr<Hadouken::BitTorrent::TorrentHandle> handle);
                static int handle_finalize(void* ctx);
                static int handle_getInfoHash(void* ctx);
                static int handle_getPeers(void* ctx);
                static int handle_getQueuePosition(void* ctx);
                static int handle_getStatus(void* ctx);
                static int handle_getTags(void* ctx);
                static int handle_getTorrentInfo(void* ctx);
                static int handle_moveStorage(void* ctx);
                static int handle_pause(void* ctx);
                static int handle_resume(void* ctx);

                static void info_initialize(void* ctx, Hadouken::BitTorrent::TorrentHandle& handle, std::unique_ptr<Hadouken::BitTorrent::TorrentInfo> info);
                static int info_finalize(void* ctx);
                static int info_getFiles(void* ctx);
                static int info_getTotalSize(void* ctx);

                static void peer_initialize(void* ctx, Hadouken::BitTorrent::PeerInfo& peer);
                static int peer_finalize(void* ctx);
                static int peer_getConnectionType(void* ctx);
                static int peer_getCountry(void* ctx);
                static int peer_getIp(void* ctx);
                static int peer_getPort(void* ctx);
                static int peer_getClient(void* ctx);
                static int peer_getProgress(void* ctx);
                static int peer_getDownloadRate(void* ctx);
                static int peer_getUploadRate(void* ctx);
                static int peer_getDownloadedBytes(void* ctx);
                static int peer_getUploadedBytes(void* ctx);

                static int session_addTorrentFile(void* ctx);
                static int session_addTorrentUri(void* ctx);
                static int session_findTorrent(void* ctx);
                static int session_getTorrents(void* ctx);
                static int session_removeTorrent(void* ctx);

                static void status_initialize(void* ctx, Hadouken::BitTorrent::TorrentStatus& status);
                static int status_finalize(void* ctx);
                static int status_getDownloadedBytes(void* ctx);
                static int status_getDownloadedBytesTotal(void* ctx);
                static int status_getDownloadRate(void* ctx);
                static int status_getName(void* ctx);
                static int status_getNumPeers(void* ctx);
                static int status_getNumSeeds(void* ctx);
                static int status_getProgress(void* ctx);
                static int status_getSavePath(void* ctx);
                static int status_getState(void* ctx);
                static int status_getUploadedBytes(void* ctx);
                static int status_getUploadedBytesTotal(void* ctx);
                static int status_getUploadRate(void* ctx);
                static int status_isFinished(void* ctx);
                static int status_isMovingStorage(void* ctx);
                static int status_isPaused(void* ctx);
                static int status_isSeeding(void* ctx);
                static int status_isSequentialDownload(void* ctx);

                template<class T>
                static T* getPointerFromThis(void* ctx, const char* fieldName);
            };
        }
    }
}

#endif