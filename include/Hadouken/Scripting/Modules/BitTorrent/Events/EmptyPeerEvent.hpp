#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_EMPTYPEEREVENT_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_EMPTYPEEREVENT_HPP

#include <Hadouken/Scripting/Modules/BitTorrent/Events/PeerEvent.hpp>

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
                    class EmptyPeerEvent : public PeerEvent
                    {
                    public:
                        EmptyPeerEvent(std::shared_ptr<Hadouken::BitTorrent::TorrentHandle> handle, std::string ip, int port);

                        void push(void* ctx);
                    };
                }
            }
        }
    }
}

#endif
