#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_LAZYENTRYWRAPPER_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_LAZYENTRYWRAPPER_HPP

namespace libtorrent
{
    struct lazy_entry;
}

namespace hadouken
{
    namespace scripting
    {
        namespace modules
        {
            namespace bittorrent
            {
                class lazy_entry_wrapper
                {
                public:
                    static void initialize(void* ctx, libtorrent::lazy_entry& entry);

                private:
                    static int finalize(void* ctx);

                    static int get_type(void* ctx);
                };
            }
        }
    }
}

#endif
