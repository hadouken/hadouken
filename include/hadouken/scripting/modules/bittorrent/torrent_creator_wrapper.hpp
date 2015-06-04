#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_TORRENTCREATORWRAPPER_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_TORRENTCREATORWRAPPER_HPP

namespace hadouken
{
    namespace scripting
    {
        namespace modules
        {
            namespace bittorrent
            {
                class torrent_creator_wrapper
                {
                public:
                    static int construct(void* ctx);

                private:
                    static int finalize(void* ctx);

                    static int generate(void* ctx);
                };
            }
        }
    }
}

#endif
