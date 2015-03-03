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
            explicit TorrentInfo(const libtorrent::torrent_info& info);
            ~TorrentInfo();

            const std::string& getComment() const;

            FileStorage const getFiles() const;

            const std::string& getName() const;

            int getNumFiles() const;

            int getNumPieces() const;

            size_t getTotalSize() const;

        private:
            std::shared_ptr<libtorrent::torrent_info> info_;
        };
    }
}

#endif
