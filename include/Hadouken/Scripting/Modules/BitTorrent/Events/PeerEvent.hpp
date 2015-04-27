#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_PEEREVENT_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_PEEREVENT_HPP

#include <Hadouken/Scripting/Modules/BitTorrent/Events/TorrentEvent.hpp>

#include <memory>
#include <string>

namespace Hadouken
{
    namespace BitTorrent
    {
        struct TorrentHandle;
    }

    namespace Scripting
    {
        namespace Modules
        {
            namespace BitTorrent
            {
                namespace Events
                {
                    class PeerEvent : public TorrentEvent
                    {
                    public:
                        PeerEvent(std::shared_ptr<Hadouken::BitTorrent::TorrentHandle> handle, std::string ip, int port);

                        void push(void* ctx, int idx);

                    private:
                        std::string ip_;
                        int port_;
                    };
                }
            }
        }
    }
}

#endif
