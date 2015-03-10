#ifndef HADOUKEN_BITTORRENT_TORRENTHANDLE_HPP
#define HADOUKEN_BITTORRENT_TORRENTHANDLE_HPP

#include <Hadouken/Config.hpp>
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

            void getFileProgress(std::vector<int64_t>& progress) const;

            HDKN_EXPORT std::string getInfoHash() const;

            void getPeerInfo(std::vector<PeerInfo>& peers) const;

            int getQueuePosition() const;

            HDKN_EXPORT TorrentStatus getStatus() const;

            TorrentInfo getTorrentFile() const;

            bool isValid() const;

            HDKN_EXPORT void moveStorage(const std::string& savePath) const;
            
            void pause() const;

            void resume() const;

            void addTag(std::string tag);

            void getTags(std::vector<std::string> tags);

            void removeTag(std::string tag);

            bool hasTag(std::string tag);

        private:
            const libtorrent::torrent_handle handle_;
            std::vector<std::string> tags_;
        };
    }
}

#endif
