#ifndef HADOUKEN_BITTORRENT_SESSION_HPP
#define HADOUKEN_BITTORRENT_SESSION_HPP

#include <Hadouken/Config.hpp>
#include <Poco/BasicEvent.h>
#include <Poco/Logger.h>
#include <Poco/Path.h>
#include <Poco/Util/LayeredConfiguration.h>

#include <memory>
#include <string>
#include <thread>
#include <vector>

namespace libtorrent
{
    struct add_torrent_params;
    
    class entry;
    typedef std::map<std::string, entry> dictionary_type;

    struct lazy_entry;
    class session;
    class sha1_hash;
    struct torrent_handle;
    class torrent_info;
}

namespace Hadouken
{
    namespace BitTorrent
    {
        class AddTorrentParams;
        struct ProxySettings;
        struct SessionStatus;
        struct TorrentHandle;

        class Session
        {
        public:
            Session(const Poco::Util::AbstractConfiguration& config);
            ~Session();

            void load();
            void unload();

            HDKN_EXPORT std::string addTorrent(std::vector<char>& buffer, AddTorrentParams& params);

            HDKN_EXPORT std::string addTorrentFile(std::string path, AddTorrentParams& params);

            HDKN_EXPORT void addTorrentUri(std::string uri, AddTorrentParams& params);

            std::shared_ptr<TorrentHandle> findTorrent(const std::string& infoHash) const;

            HDKN_EXPORT uint16_t getListenPort() const;

            HDKN_EXPORT uint16_t getSslListenPort() const;

            HDKN_EXPORT std::vector<std::shared_ptr<TorrentHandle>> getTorrents() const;

            HDKN_EXPORT std::string getLibtorrentVersion() const;

            ProxySettings getProxy() const;

            SessionStatus getStatus() const;

            HDKN_EXPORT bool isListening() const;

            HDKN_EXPORT bool isPaused() const;

            HDKN_EXPORT void pause();

            void removeTorrent(std::shared_ptr<TorrentHandle> handle, int options = 0) const;

            HDKN_EXPORT void resume();

            HDKN_EXPORT std::string getTorrentMetadata(std::string infoHash, std::string key);

            HDKN_EXPORT std::vector<std::string> getTorrentMetadataKeys(std::string infoHash);

            HDKN_EXPORT void setTorrentMetadata(std::string infoHash, std::string key, std::string value);

            void setProxy(ProxySettings& proxy);

            Poco::BasicEvent<std::shared_ptr<TorrentHandle>> onTorrentAdded;

            Poco::BasicEvent<std::shared_ptr<TorrentHandle>> onTorrentFinished;

            Poco::BasicEvent<std::string> onTorrentRemoved;

        protected:
            void loadSessionState();
            void loadResumeData();
            void loadHadoukenState(const libtorrent::torrent_handle& handle, const libtorrent::lazy_entry& entry);

            void saveSessionState();
            void saveResumeData();
            void saveHadoukenState(const libtorrent::torrent_handle& handle, libtorrent::dictionary_type& entry);

            void readAlerts();

            void saveTorrentInfo(const libtorrent::torrent_info& info);

            libtorrent::add_torrent_params getDefaultAddTorrentParams();
            Poco::Path getDataPath();
            Poco::Path getTorrentsPath();

        private:
            Poco::Logger& logger_;
            const Poco::Util::AbstractConfiguration& config_;
            std::unique_ptr<libtorrent::session> sess_;
            std::map<std::string, std::map<std::string, std::string>> torrentMetadata_;

            // default settings
            std::string default_save_path_;
            
            bool isRunning_;
            std::thread alertReader_;
        };
    }
}

#endif