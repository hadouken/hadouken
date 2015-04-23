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

                private:
                    static int addTorrentFile(void* ctx);
                    static int addTorrentUri(void* ctx);
                    static int findTorrent(void* ctx);
                    static int getListenPort(void* ctx);
                    static int getSslListenPort(void* ctx);
                    static int getStatus(void* ctx);
                    static int getTorrents(void* ctx);
                    static int isListening(void* ctx);
                    static int isPaused(void* ctx);
                    static int pause(void* ctx);
                    static int removeTorrent(void* ctx);
                    static int resume(void* ctx);
                };
            }
        }
    }
}

#endif
