#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_FILERENAMEDEVENT_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_EVENTS_FILERENAMEDEVENT_HPP

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
                    class FileRenamedEvent : public TorrentEvent
                    {
                    public:
                        FileRenamedEvent(std::shared_ptr<Hadouken::BitTorrent::TorrentHandle> handle, int index, std::string name);

                        void push(void* ctx);

                    private:
                        int index_;
                        std::string name_;
                    };
                }
            }
        }
    }
}

#endif
