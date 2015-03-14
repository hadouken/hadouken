#ifndef HADOUKEN_BITTORRENT_ADDTORRENTPARAMS_HPP
#define HADOUKEN_BITTORRENT_ADDTORRENTPARAMS_HPP

#include <Poco/Path.h>
#include <string>
#include <vector>

namespace Hadouken
{
    namespace BitTorrent
    {
        class AddTorrentParams
        {
        public:
            Poco::Path savePath;

            std::vector<std::string> tags;
        };
    }
}

#endif
