#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_STORAGEMOVEDEVENT_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_STORAGEMOVEDEVENT_HPP

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
                    class StorageMovedEvent : public TorrentEvent
                    {
                    public:
                        StorageMovedEvent(std::shared_ptr<Hadouken::BitTorrent::TorrentHandle> handle, std::string path);

                        void push(void* ctx);

                    private:
                        std::string path_;
                    };
                }
            }
        }
    }
}

#endif
