#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_TORRENTDELETEDEVENT_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_TORRENTDELETEDEVENT_HPP

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
                    class TorrentDeletedEvent : public Event
                    {
                    public:
                        TorrentDeletedEvent(std::string infoHash);

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
