#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENTMODULE_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENTMODULE_HPP

namespace libtorrent
{
    class session;
}

namespace hadouken
{
    namespace scripting
    {
        namespace modules
        {
            class bittorrent_module
            {
            public:
                static int initialize(void* ctx, libtorrent::session& session);
            };
        }
    }
}

#endif