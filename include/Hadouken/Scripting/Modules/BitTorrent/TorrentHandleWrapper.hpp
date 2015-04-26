#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_TORRENTHANDLEWRAPPER_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_TORRENTHANDLEWRAPPER_HPP

#include <memory>

namespace Hadouken
{
    namespace BitTorrent
    {
        struct TorrentHandle;
    }
}

namespace Hadouken
{
    namespace Scripting
    {
        namespace Modules
        {
            namespace BitTorrent
            {
                class TorrentHandleWrapper
                {
                public:
                    static void initialize(void* ctx, std::shared_ptr<Hadouken::BitTorrent::TorrentHandle> handle);

                private:
                    static int clearError(void* ctx);
                    static int forceRecheck(void* ctx);
                    static int getInfoHash(void* ctx);
                    static int getPeers(void* ctx);
                    static int getQueuePosition(void* ctx);
                    static int getStatus(void* ctx);
                    static int getTorrentInfo(void* ctx);
                    static int getTrackers(void* ctx);
                    static int moveStorage(void* ctx);
                    static int pause(void* ctx);
                    static int renameFile(void* ctx);
                    static int resume(void* ctx);

                    static int queueBottom(void* ctx);
                    static int queueDown(void* ctx);
                    static int queueTop(void* ctx);
                    static int queueUp(void* ctx);

                    static int getMetadata(void* ctx);
                    static int getMetadataKeys(void* ctx);
                    static int setMetadata(void* ctx);

                    static int getMaxConnections(void* ctx);
                    static int getMaxUploads(void* ctx);
                    static int getResolveCountries(void* ctx);
                    static int getSequentialDownload(void* ctx);
                    static int getUploadLimit(void* ctx);
                    static int getUploadMode(void* ctx);
                    static int setMaxConnections(void* ctx);
                    static int setMaxUploads(void* ctx);
                    static int setResolveCountries(void* ctx);
                    static int setSequentialDownload(void* ctx);
                    static int setUploadLimit(void* ctx);
                    static int setUploadMode(void* ctx);
                };
            }
        }
    }
}

#endif
