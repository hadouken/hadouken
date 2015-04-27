#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_LISTENSUCCEEDEDEVENT_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_LISTENSUCCEEDEDEVENT_HPP

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
                    class ListenSucceededEvent : public Event
                    {
                    public:
                        ListenSucceededEvent(std::string address, int port, int type);

                        void push(void* ctx);

                    private:
                        std::string address_;
                        int port_;
                        int type_;
                    };
                }
            }
        }
    }
}

#endif
