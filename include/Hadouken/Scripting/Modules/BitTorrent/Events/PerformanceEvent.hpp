#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_PERFORMANCEEVENT_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_PERFORMANCEEVENT_HPP

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
                    class PerformanceEvent : public TorrentEvent
                    {
                    public:
                        PerformanceEvent(std::shared_ptr<Hadouken::BitTorrent::TorrentHandle> handle, int code);

                        void push(void* ctx);

                    private:
                        int code_;
                    };
                }
            }
        }
    }
}

#endif
