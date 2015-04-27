#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_STORAGEMOVEFAILEDEVENT_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_STORAGEMOVEFAILEDEVENT_HPP

#include <Hadouken/BitTorrent/Error.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/Events/TorrentEvent.hpp>
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
                    class StorageMoveFailedEvent : public TorrentEvent
                    {
                    public:
                        StorageMoveFailedEvent(std::shared_ptr<Hadouken::BitTorrent::TorrentHandle> handle, Hadouken::BitTorrent::Error error);

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
