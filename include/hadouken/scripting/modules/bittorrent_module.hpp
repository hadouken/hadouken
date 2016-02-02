#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENTMODULE_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENTMODULE_HPP

namespace libtorrent
{
    class session_handle;
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
                static int initialize(void* ctx, libtorrent::session_handle& session);
            };
        }
    }
}

#endif