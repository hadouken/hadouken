#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_STATECHANGEDEVENT_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_STATECHANGEDEVENT_HPP

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
                    class StateChangedEvent : public TorrentEvent
                    {
                    public:
                        StateChangedEvent(std::shared_ptr<Hadouken::BitTorrent::TorrentHandle> handle, int state, int previousState);

                        void push(void* ctx);

                    private:
                        int state_;
                        int previousState_;
                    };
                }
            }
        }
    }
}

#endif
