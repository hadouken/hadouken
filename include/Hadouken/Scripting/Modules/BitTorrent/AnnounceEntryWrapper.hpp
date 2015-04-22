#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_ANNOUNCEENTRYWRAPPER_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_ANNOUNCEENTRYWRAPPER_HPP

namespace Hadouken
{
    namespace BitTorrent
    {
        struct AnnounceEntry;
    }
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
                    static void initialize(void* ctx, Hadouken::BitTorrent::AnnounceEntry& announceEntry);

                private:
                    static int finalize(void* ctx);

                    static int getMessage(void* ctx);
                    static int getUrl(void* ctx);
                };
            }
        }
    }
}

#endif
