#ifndef HADOUKEN_BITTORRENT_ADDTORRENTPARAMS_HPP
#define HADOUKEN_BITTORRENT_ADDTORRENTPARAMS_HPP

#include <map>
#include <string>

namespace Hadouken
{
    namespace BitTorrent
    {
        class AddTorrentParams
        {
        public:
            std::string savePath;

            std::map<std::string, std::string> data;
        };
    }
}

#endif
