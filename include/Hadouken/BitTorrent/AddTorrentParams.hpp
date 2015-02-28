#ifndef HADOUKEN_BITTORRENT_ADDTORRENTPARAMS_HPP
#define HADOUKEN_BITTORRENT_ADDTORRENTPARAMS_HPP

#include <Poco/Path.h>

namespace Hadouken
{
    namespace BitTorrent
    {
        class AddTorrentParams
        {
        public:
            Poco::Path savePath;
        };
    }
}

#endif
