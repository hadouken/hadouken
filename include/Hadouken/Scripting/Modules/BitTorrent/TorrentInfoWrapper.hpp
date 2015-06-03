#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_TORRENTINFOWRAPPER_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_TORRENTINFOWRAPPER_HPP

namespace libtorrent
{
    class torrent_info;
    struct torrent_handle;
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
                    static int construct(void* ctx);
                    static void initialize(void* ctx, const libtorrent::torrent_info& info);

                private:
                    static int finalize(void* ctx);

                    static int getFiles(void* ctx);
                    static int getInfoHash(void* ctx);
                    static int getName(void* ctx);
                    static int getTotalSize(void* ctx);
                };
            }
        }
    }
}

#endif
