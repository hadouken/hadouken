#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_BDECODENODEWRAPPER_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_BDECODENODEWRAPPER_HPP

namespace libtorrent
{
    struct bdecode_node;
}

namespace hadouken
{
    namespace scripting
    {
        namespace modules
        {
            namespace bittorrent
            {
                class bdecode_node_wrapper
                {
                public:
                    static void initialize(void* ctx, libtorrent::bdecode_node& entry);

                private:
                    static int finalize(void* ctx);

                    static int get_type(void* ctx);
                };
            }
        }
    }
}

#endif
