#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_FILERENAMEFAILEDEVENT_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_FILERENAMEFAILEDEVENT_HPP

#include <Hadouken/BitTorrent/Error.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/Events/TorrentEvent.hpp>
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
                    class FileRenameFailedEvent : public TorrentEvent
                    {
                    public:
                        FileRenameFailedEvent(std::shared_ptr<Hadouken::BitTorrent::TorrentHandle> handle, int index, Hadouken::BitTorrent::Error error);

                        void push(void* ctx);

                    private:
                        Hadouken::BitTorrent::Error error_;
                        int index_;
                    };
                }
            }
        }
    }
}

#endif
