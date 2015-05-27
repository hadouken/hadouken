#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_SESSIONWRAPPER_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_SESSIONWRAPPER_HPP

#include <memory>

namespace libtorrent
{
    class session;
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
                    static void initialize(void* ctx, libtorrent::session& session);

                private:
                    static int addDhtRouter(void* ctx);
                    static int addTorrent(void* ctx);
                    static int findTorrent(void* ctx);
                    static int getListenPort(void* ctx);
                    static int getSslListenPort(void* ctx);
                    static int getAlerts(void* ctx);
                    static int getSettings(void* ctx);
                    static int getStatus(void* ctx);
                    static int getTorrents(void* ctx);
                    static int isListening(void* ctx);
                    static int isPaused(void* ctx);
                    static int listenOn(void* ctx);
                    static int loadState(void* ctx);
                    static int pause(void* ctx);
                    static int removeTorrent(void* ctx);
                    static int resume(void* ctx);
                    static int saveState(void* ctx);
                    static int startDht(void* ctx);
                    static int waitForAlert(void* ctx);
                };
            }
        }
    }
}

#endif
