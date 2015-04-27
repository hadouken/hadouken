#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_TRACKERIDEVENT_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_TRACKERIDEVENT_HPP

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
                    class TrackerIdEvent : public TrackerEvent
                    {
                    public:
                        TrackerIdEvent(std::shared_ptr<Hadouken::BitTorrent::TorrentHandle> handle, std::string url, std::string trackerId);

                        void push(void* ctx);

                    private:
                        std::string trackerId_;
                    };
                }
            }
        }
    }
}

#endif
