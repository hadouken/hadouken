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
                    static int finalize(void* ctx);

                    static int getInfoHash(void* ctx);
                    static int getPeers(void* ctx);
                    static int getQueuePosition(void* ctx);
                    static int getStatus(void* ctx);
                    static int getTags(void* ctx);
                    static int getTorrentInfo(void* ctx);
                    static int moveStorage(void* ctx);
                    static int pause(void* ctx);
                    static int resume(void* ctx);
                };
            }
        }
    }
}

#endif
