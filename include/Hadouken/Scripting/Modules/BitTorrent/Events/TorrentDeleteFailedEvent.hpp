#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_TORRENTDELETEFAILEDEVENT_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_TORRENTDELETEFAILEDEVENT_HPP

#include <Hadouken/Scripting/Event.hpp>

#include <Hadouken/BitTorrent/Error.hpp>
#include <string>

namespace Hadouken
{
    namespace Scripting
    {
        namespace Modules
        {
            namespace BitTorrent
            {
                namespace Events
                {
                    class TorrentDeleteFailedEvent : public Event
                    {
                    public:
                        TorrentDeleteFailedEvent(std::string infoHash, Hadouken::BitTorrent::Error error);

                        void push(void* ctx);

                    private:
                        std::string infoHash_;
                        Hadouken::BitTorrent::Error error_;
                    };
                }
            }
        }
    }
}

#endif
