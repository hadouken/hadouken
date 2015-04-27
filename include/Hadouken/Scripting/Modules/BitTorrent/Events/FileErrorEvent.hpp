#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_FILEERROREVENT_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_FILEERROREVENT_HPP

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
                    class FileErrorEvent : public TorrentEvent
                    {
                    public:
                        FileErrorEvent(std::shared_ptr<Hadouken::BitTorrent::TorrentHandle> handle, Hadouken::BitTorrent::Error error, std::string file);

                        void push(void* ctx);

                    private:
                        Hadouken::BitTorrent::Error error_;
                        std::string file_;
                    };
                }
            }
        }
    }
}

#endif
