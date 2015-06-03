#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_TORRENTCREATORWRAPPER_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_TORRENTCREATORWRAPPER_HPP

namespace Hadouken
{
    namespace Scripting
    {
        namespace Modules
        {
            namespace BitTorrent
            {
                class TorrentCreatorWrapper
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
