#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_INCOMINGCONNECTIONEVENT_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_INCOMINGCONNECTIONEVENT_HPP

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
                    class IncomingConnectionEvent : public Event
                    {
                    public:
                        IncomingConnectionEvent(std::string address, int port);

                        void push(void* ctx);

                    private:
                        std::string address_;
                        int port_;
                    };
                }
            }
        }
    }
}

#endif
