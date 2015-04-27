#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_TRACKEREVENT_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_TRACKEREVENT_HPP

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
                    class TrackerEvent : public TorrentEvent
                    {
                    public:
                        TrackerEvent(std::shared_ptr<Hadouken::BitTorrent::TorrentHandle> handle, std::string url);

                        void push(void* ctx, int idx);

                    private:
                        std::string url_;
                    };
                }
            }
        }
    }
}

#endif
