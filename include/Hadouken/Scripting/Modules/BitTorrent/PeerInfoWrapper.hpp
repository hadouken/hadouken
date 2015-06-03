#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_PEERINFOWRAPPER_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_PEERINFOWRAPPER_HPP

#include <memory>

namespace libtorrent
{
    struct peer_info;
}

namespace Hadouken
{
    namespace Scripting
    {
        namespace Modules
        {
            namespace BitTorrent
            {
                class PeerInfoWrapper
                {
                public:
                    static void initialize(void* ctx, libtorrent::peer_info& peer);

                private:
                    static int finalize(void* ctx);

                    static int getCountry(void* ctx);
                    static int getIp(void* ctx);
                    static int getPort(void* ctx);
                    static int getConnectionType(void* ctx);
                    static int getClient(void* ctx);
                    static int getProgress(void* ctx);
                    static int getDownloadRate(void* ctx);
                    static int getUploadRate(void* ctx);
                    static int getDownloadedBytes(void* ctx);
                    static int getUploadedBytes(void* ctx);
                };
            }
        }
    }
}

#endif
