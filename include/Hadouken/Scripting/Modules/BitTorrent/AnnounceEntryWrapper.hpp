#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_ANNOUNCEENTRYWRAPPER_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_ANNOUNCEENTRYWRAPPER_HPP

namespace libtorrent
{
    struct announce_entry;
}

namespace Hadouken
{
    namespace Scripting
    {
        namespace Modules
        {
            namespace BitTorrent
            {
                class AnnounceEntryWrapper
                {
                public:
                    static void initialize(void* ctx, libtorrent::announce_entry& entry);

                private:
                    static int finalize(void* ctx);

                    static int getIsUpdating(void* ctx);
                    static int getIsVerified(void* ctx);
                    static int getMessage(void* ctx);
                    static int getTier(void* ctx);
                    static int getUrl(void* ctx);
                };
            }
        }
    }
}

#endif
