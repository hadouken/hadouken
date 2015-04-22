#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_TORRENTINFOWRAPPER_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_TORRENTINFOWRAPPER_HPP

#include <memory>

namespace Hadouken
{
    namespace BitTorrent
    {
        class TorrentInfo;
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
                class TorrentInfoWrapper
                {
                public:
                    static void initialize(void* ctx, Hadouken::BitTorrent::TorrentHandle& handle, std::unique_ptr<Hadouken::BitTorrent::TorrentInfo> torrentInfo);

                private:
                    static int finalize(void* ctx);

                    static int getFiles(void* ctx);
                    static int getTotalSize(void* ctx);
                };
            }
        }
    }
}

#endif
