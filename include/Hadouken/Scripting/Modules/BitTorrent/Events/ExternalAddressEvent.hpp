#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_EXTERNALADDRESSEVENT_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_EXTERNALADDRESSEVENT_HPP

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
                    class ExternalAddressEvent : public Event
                    {
                    public:
                        ExternalAddressEvent(std::string address);
                        
                        void push(void* ctx);
                    private:
                        std::string address_;
                    };
                }
            }
        }
    }
}

#endif
