#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_METADATAFAILEDEVENT_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_METADATAFAILEDEVENT_HPP

#include <Hadouken/Scripting/Modules/BitTorrent/Events/TorrentEvent.hpp>

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
                    class MetadataFailedEvent : public TorrentEvent
                    {
                    public:
                        MetadataFailedEvent(std::shared_ptr<Hadouken::BitTorrent::TorrentHandle> handle, Hadouken::BitTorrent::Error error);

                        void push(void* ctx);

                    private:
                        Hadouken::BitTorrent::Error error_;
                    };
                }
            }
        }
    }
}

#endif
