#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_SESSIONWRAPPER_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_SESSIONWRAPPER_HPP

#include <memory>

namespace Hadouken
{
    namespace BitTorrent
    {
        class Session;
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
                class SessionWrapper
                {
                public:
                    static void initialize(void* ctx, Hadouken::BitTorrent::Session& session);

                    static const char* field;

                private:
                    static int addTorrentFile(void* ctx);
                    static int addTorrentUri(void* ctx);
                    static int findTorrent(void* ctx);
                    static int getTorrents(void* ctx);
                    static int removeTorrent(void* ctx);
                };
            }
        }
    }
}

#endif
