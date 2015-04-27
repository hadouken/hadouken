#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_EMPTYEVENT_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_EMPTYEVENT_HPP

#include <Hadouken/Scripting/Event.hpp>

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
                    class EmptyEvent : public Event
                    {
                    public:
                        void push(void* ctx);
                    };
                }
            }
        }
    }
}

#endif
