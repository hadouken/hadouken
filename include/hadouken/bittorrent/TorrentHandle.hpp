#ifndef HADOUKEN_BITTORRENT_TORRENTHANDLE_HPP
#define HADOUKEN_BITTORRENT_TORRENTHANDLE_HPP

#include <libtorrent/torrent_handle.hpp>
#include <string>

namespace Hadouken
{
    namespace BitTorrent
    {
        struct PeerInfo;
        class Session;
        class TorrentInfo;
        struct TorrentStatus;

        struct TorrentHandle
        {
            friend class Session;

            explicit TorrentHandle(const libtorrent::torrent_handle& handle);

            void getFileProgress(std::vector<size_t>& progress) const;

            std::string getInfoHash() const;

            void getPeerInfo(std::vector<PeerInfo>& peers) const;

            int getQueuePosition() const;

            TorrentStatus getStatus() const;

            TorrentInfo getTorrentFile() const;

            bool isValid() const;

            void moveStorage(const std::string& savePath) const;
            
            void pause() const;

            void resume() const;

        private:
            const libtorrent::torrent_handle handle_;
        };
    }
}

#endif
