#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_TORRENTEVENT_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_TORRENTEVENT_HPP

#include <Hadouken/Scripting/Event.hpp>
#include <memory>

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
                    class TorrentEvent : public Event
                    {
                    public:
                        TorrentEvent(std::shared_ptr<Hadouken::BitTorrent::TorrentHandle> handle_);

                        virtual void push(void* ctx);

                    private:
                        std::shared_ptr<Hadouken::BitTorrent::TorrentHandle> handle_;
                    };
                }
            }
        }
    }
}

#endif
