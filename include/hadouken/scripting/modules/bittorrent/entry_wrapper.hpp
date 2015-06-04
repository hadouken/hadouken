#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_ENTRYWRAPPER_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_ENTRYWRAPPER_HPP

namespace libtorrent
{
    class entry;
}

namespace hadouken
{
    namespace scripting
    {
        namespace modules
        {
            namespace bittorrent
            {
                class entry_wrapper
                {
                public:
                    static void initialize(void* ctx, libtorrent::entry& entry);

                private:
                    static int finalize(void* ctx);

                    static int get_type(void* ctx);
                };
            }
        }
    }
}

#endif
