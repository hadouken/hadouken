#ifndef HADOUKEN_BITTORRENT_TORRENTHANDLE_HPP
#define HADOUKEN_BITTORRENT_TORRENTHANDLE_HPP

#include <Hadouken/Config.hpp>
#include <libtorrent/torrent_handle.hpp>
#include <string>

namespace Hadouken
{
    namespace BitTorrent
    {
        struct AnnounceEntry;
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

            HDKN_EXPORT std::vector<int64_t> getFileProgress() const;

            HDKN_EXPORT std::string getInfoHash() const;

            HDKN_EXPORT int getMaxConnections() const;

            HDKN_EXPORT int getMaxUploads() const;

            HDKN_EXPORT std::vector<PeerInfo> getPeers() const;

            HDKN_EXPORT int getQueuePosition() const;

            HDKN_EXPORT bool getResolveCountries() const;

            HDKN_EXPORT TorrentStatus getStatus() const;

            HDKN_EXPORT std::unique_ptr<TorrentInfo> getTorrentFile() const;

            HDKN_EXPORT std::vector<AnnounceEntry> getTrackers() const;

            HDKN_EXPORT int getUploadLimit() const;

            HDKN_EXPORT bool getUploadMode() const;

            HDKN_EXPORT bool isValid() const;

            HDKN_EXPORT void moveStorage(const std::string& savePath) const;
            
            HDKN_EXPORT void pause() const;

            HDKN_EXPORT void resume() const;

            HDKN_EXPORT void addTag(std::string tag);

            HDKN_EXPORT std::vector<std::string> getTags() const;

            HDKN_EXPORT void removeTag(std::string tag);

            HDKN_EXPORT bool hasTag(std::string tag);

            HDKN_EXPORT void queueBottom() const;

            HDKN_EXPORT void queueDown() const;

            HDKN_EXPORT void queueTop() const;

            HDKN_EXPORT void queueUp() const;

            HDKN_EXPORT void setMaxConnections(int limit) const;

            HDKN_EXPORT void setMaxUploads(int limit) const;

            HDKN_EXPORT void setResolveCountries(bool value);

            HDKN_EXPORT void setUploadLimit(int limit) const;

            HDKN_EXPORT void setUploadMode(bool mode) const;

        private:
            libtorrent::torrent_handle handle_;
            std::vector<std::string> tags_;
        };
    }
}

#endif
