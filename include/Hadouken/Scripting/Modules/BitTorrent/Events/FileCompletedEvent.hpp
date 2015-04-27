#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_FILECOMPLETEDEVENT_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_FILECOMPLETEDEVENT_HPP

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
                    class FileCompletedEvent : public TorrentEvent
                    {
                    public:
                        FileCompletedEvent(std::shared_ptr<Hadouken::BitTorrent::TorrentHandle> handle, int index);

                        void push(void* ctx);

                    private:
                        int index_;
                    };
                }
            }
        }
    }
}

#endif
