#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_URLSEEDEVENT_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_URLSEEDEVENT_HPP

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
                    class UrlSeedEvent : public TorrentEvent
                    {
                    public:
                        UrlSeedEvent(std::shared_ptr<Hadouken::BitTorrent::TorrentHandle> handle, std::string url, std::string message);

                        void push(void* ctx);

                    private:
                        std::string url_;
                        std::string message_;
                    };
                }
            }
        }
    }
}

#endif
