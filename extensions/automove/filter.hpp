#ifndef AUTOMOVE_FILTER_HPP
#define AUTOMOVE_FILTER_HPP

#include <string>

namespace Hadouken
{
    namespace BitTorrent
    {
        struct TorrentHandle;
    }
}

namespace AutoMove
{
    struct Filter
    {
        virtual bool isMatch(Hadouken::BitTorrent::TorrentHandle& handle) = 0;

        std::string path;
    };
}

#endif
