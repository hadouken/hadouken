#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_TORRENTSTATUSWRAPPER_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_TORRENTSTATUSWRAPPER_HPP

#include <memory>

namespace Hadouken
{
    namespace BitTorrent
    {
        struct TorrentStatus;
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
                class TorrentStatusWrapper
                {
                public:
                    static void initialize(void* ctx, Hadouken::BitTorrent::TorrentStatus& status);

                    static const char* field;

                private:
                    static int finalize(void* ctx);

                    static int getName(void* ctx);
                    static int getProgress(void* ctx);
                    static int getSavePath(void* ctx);
                    static int getDownloadRate(void* ctx);
                    static int getUploadRate(void* ctx);
                    static int getDownloadedBytes(void* ctx);
                    static int getDownloadedBytesTotal(void* ctx);
                    static int getUploadedBytes(void* ctx);
                    static int getUploadedBytesTotal(void* ctx);
                    static int getNumPeers(void* ctx);
                    static int getNumSeeds(void* ctx);
                    static int getState(void* ctx);
                    static int isFinished(void* ctx);
                    static int isMovingStorage(void* ctx);
                    static int isPaused(void* ctx);
                    static int isSeeding(void* ctx);
                    static int isSequentialDownload(void* ctx);
                };
            }
        }
    }
}

#endif
