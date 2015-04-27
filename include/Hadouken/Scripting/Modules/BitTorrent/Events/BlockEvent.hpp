#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_BLOCKEVENT_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_BLOCKEVENT_HPP

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
                    class BlockEvent : public PeerEvent
                    {
                    public:
                        BlockEvent(std::shared_ptr<Hadouken::BitTorrent::TorrentHandle> handle, std::string ip, int port, int pieceIndex, int blockIndex);

                        void push(void* ctx);

                    private:
                        int pieceIndex_;
                        int blockIndex_;
                    };
                }
            }
        }
    }
}

#endif
