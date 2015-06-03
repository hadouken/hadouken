#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_ENTRYWRAPPER_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_ENTRYWRAPPER_HPP

namespace libtorrent
{
    class entry;
}

namespace Hadouken
{
    namespace Scripting
    {
        namespace Modules
        {
            namespace BitTorrent
            {
                class EntryWrapper
                {
                public:
                    static void initialize(void* ctx, libtorrent::entry& entry);

                private:
                    static int finalize(void* ctx);

                    static int getType(void* ctx);
                };
            }
        }
    }
}

#endif
