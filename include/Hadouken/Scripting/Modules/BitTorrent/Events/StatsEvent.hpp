#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_STATSEVENT_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_STATSEVENT_HPP

#include <Hadouken/Scripting/Modules/BitTorrent/Events/TorrentEvent.hpp>
#include <memory>
#include <vector>

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
                    class StatsEvent : public TorrentEvent
                    {
                    public:
                        StatsEvent(std::shared_ptr<Hadouken::BitTorrent::TorrentHandle> handle, int interval, int (&transferred)[10]);

                        void push(void* ctx);

                    private:
                        int interval_;
                        std::vector<int> transferred_;
                    };
                }
            }
        }
    }
}

#endif
