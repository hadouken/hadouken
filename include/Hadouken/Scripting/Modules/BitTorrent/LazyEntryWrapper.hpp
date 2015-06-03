#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_LAZYENTRYWRAPPER_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_LAZYENTRYWRAPPER_HPP

namespace libtorrent
{
    struct lazy_entry;
}

namespace Hadouken
{
    namespace Scripting
    {
        namespace Modules
        {
            namespace BitTorrent
            {
                class LazyEntryWrapper
                {
                public:
                    static void initialize(void* ctx, libtorrent::lazy_entry& entry);

                private:
                    static int finalize(void* ctx);

                    static int getType(void* ctx);
                };
            }
        }
    }
}

#endif
