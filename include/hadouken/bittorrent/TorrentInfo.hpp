#ifndef HADOUKEN_BITTORRENT_TORRENTINFO_HPP
#define HADOUKEN_BITTORRENT_TORRENTINFO_HPP

#include <memory>
#include <string>

namespace libtorrent
{
    class torrent_info;
}

namespace Hadouken
{
    namespace BitTorrent
    {
        class FileStorage;

        class TorrentInfo
        {
        public:
            explicit TorrentInfo(libtorrent::torrent_info const* info);
            ~TorrentInfo();

            const std::string& getComment() const;

            FileStorage const getFiles() const;

            const std::string& getName() const;

            int getNumFiles() const;

            int getNumPieces() const;

        private:
            libtorrent::torrent_info const* info_;
        };
    }
}

#endif
