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

            HDKN_EXPORT explicit TorrentHandle(const libtorrent::torrent_handle& handle);

            HDKN_EXPORT TorrentHandle(const TorrentHandle& h);

            HDKN_EXPORT TorrentHandle& operator=(const TorrentHandle&) = delete;

            void getFileProgress(std::vector<int64_t>& progress) const;

            HDKN_EXPORT std::string getInfoHash() const;

            void getPeerInfo(std::vector<PeerInfo>& peers) const;

            HDKN_EXPORT int getQueuePosition() const;

            HDKN_EXPORT TorrentStatus getStatus() const;

            std::unique_ptr<TorrentInfo> getTorrentFile() const;

            HDKN_EXPORT bool isValid() const;

            HDKN_EXPORT void moveStorage(const std::string& savePath) const;
            
            HDKN_EXPORT void pause() const;

            HDKN_EXPORT void resume() const;

            void addTag(std::string tag);

            void getTags(std::vector<std::string>& tags) const;

            void removeTag(std::string tag);

            bool hasTag(std::string tag);

        private:
            const libtorrent::torrent_handle handle_;
            std::vector<std::string> tags_;
        };
    }
}

#endif
