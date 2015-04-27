#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_PIECEFINISHEDEVENT_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_PIECEFINISHEDEVENT_HPP

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
                    class PieceFinishedEvent : public TorrentEvent
                    {
                    public:
                        PieceFinishedEvent(std::shared_ptr<Hadouken::BitTorrent::TorrentHandle> handle, int pieceIndex);

                        void push(void* ctx);

                    private:
                        int pieceIndex_;
                    };
                }
            }
        }
    }
}

#endif
