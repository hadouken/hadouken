#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_TORRENTREMOVEDEVENT_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_TORRENTREMOVEDEVENT_HPP

#include <Hadouken/Scripting/Event.hpp>
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
                    class TorrentRemovedEvent : public Event
                    {
                    public:
                        TorrentRemovedEvent(std::string infoHash);

                        void push(void* ctx);

                    private:
                        std::string infoHash_;
                    };
                }
            }
        }
    }
}

#endif
