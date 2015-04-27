#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_PEERERROREVENT_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_PEERERROREVENT_HPP

#include <Hadouken/Scripting/Modules/BitTorrent/Events/PeerEvent.hpp>

#include <Hadouken/BitTorrent/Error.hpp>
#include <memory>

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
                    class PeerErrorEvent : public PeerEvent
                    {
                    public:
                        PeerErrorEvent(std::shared_ptr<Hadouken::BitTorrent::TorrentHandle> handle, std::string ip, int port, Hadouken::BitTorrent::Error error);

                        void push(void* ctx);

                    private:
                        Hadouken::BitTorrent::Error error_;
                    };
                }
            }
        }
    }
}

#endif
