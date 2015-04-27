#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_TRACKERANNOUNCEEVENT_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_TRACKERANNOUNCEEVENT_HPP

#include <Hadouken/Scripting/Modules/BitTorrent/Events/TrackerEvent.hpp>

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
                    class TrackerAnnounceEvent : public TrackerEvent
                    {
                    public:
                        TrackerAnnounceEvent(std::shared_ptr<Hadouken::BitTorrent::TorrentHandle> handle, std::string url, int event);

                        void push(void* ctx);

                    private:
                        int event_;
                    };
                }
            }
        }
    }
}

#endif
