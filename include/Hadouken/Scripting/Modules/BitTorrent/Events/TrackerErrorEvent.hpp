#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_TRACKERERROREVENT_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_TRACKERERROREVENT_HPP

#include <Hadouken/Scripting/Modules/BitTorrent/Events/TrackerEvent.hpp>

#include <Hadouken/BitTorrent/Error.hpp>
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
                    class TrackerErrorEvent : public TrackerEvent
                    {
                    public:
                        TrackerErrorEvent(std::shared_ptr<Hadouken::BitTorrent::TorrentHandle> handle, Hadouken::BitTorrent::Error error, std::string url, int times, int statusCode, std::string message);

                        void push(void* ctx);

                    private:
                        Hadouken::BitTorrent::Error error_;
                        int times_;
                        int statusCode_;
                        std::string message_;
                    };
                }
            }
        }
    }
}

#endif
