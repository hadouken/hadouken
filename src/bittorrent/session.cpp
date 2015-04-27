#include <Hadouken/BitTorrent/Session.hpp>

#include <fstream>
#include <iostream>

#include <Hadouken/BitTorrent/AddTorrentParams.hpp>
#include <Hadouken/BitTorrent/Error.hpp>
#include <Hadouken/BitTorrent/ProxySettings.hpp>
#include <Hadouken/BitTorrent/SessionStatus.hpp>
#include <Hadouken/BitTorrent/TorrentHandle.hpp>

#include <Hadouken/Scripting/ScriptingSubsystem.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/Events/BlockEvent.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/Events/DhtReplyEvent.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/Events/EmptyEvent.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/Events/EmptyPeerEvent.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/Events/ExternalAddressEvent.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/Events/FileCompletedEvent.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/Events/FileErrorEvent.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/Events/FileRenamedEvent.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/Events/FileRenameFailedEvent.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/Events/HashFailedEvent.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/Events/IncomingConnectionEvent.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/Events/ListenSucceededEvent.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/Events/MetadataFailedEvent.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/Events/PeerErrorEvent.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/Events/PerformanceEvent.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/Events/PieceFinishedEvent.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/Events/ScrapeFailedEvent.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/Events/ScrapeReplyEvent.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/Events/StateChangedEvent.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/Events/StatsEvent.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/Events/StorageMovedEvent.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/Events/StorageMoveFailedEvent.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/Events/TorrentEvent.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/Events/TorrentDeletedEvent.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/Events/TorrentDeleteFailedEvent.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/Events/TorrentErrorEvent.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/Events/TorrentRemovedEvent.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/Events/TrackerAnnounceEvent.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/Events/TrackerErrorEvent.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/Events/TrackerIdEvent.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/Events/TrackerReplyEvent.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/Events/TrackerWarningEvent.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/Events/UrlSeedEvent.hpp>

#include <libtorrent/alert_types.hpp>
#include <libtorrent/bencode.hpp>
#include <libtorrent/create_torrent.hpp>
#include <libtorrent/extensions/lt_trackers.hpp>
#include <libtorrent/extensions/smart_ban.hpp>
#include <libtorrent/extensions/ut_metadata.hpp>
#include <libtorrent/extensions/ut_pex.hpp>
#include <libtorrent/session.hpp>
#include <libtorrent/version.hpp>
#include <Poco/File.h>
#include <Poco/Path.h>
#include <Poco/Util/Application.h>

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Scripting;
using namespace Hadouken::Scripting::Modules::BitTorrent::Events;
using namespace Poco::Util;

Session::Session(const Poco::Util::AbstractConfiguration& config)
    : logger_(Poco::Logger::get("hadouken.bittorrent.session")),
      config_(config)
{
    default_save_path_ = config_.getString("bittorrent.defaultSavePath", ".");

    // Use the default LT fingerprint.
    libtorrent::fingerprint fingerprint("LT", LIBTORRENT_VERSION_MAJOR, LIBTORRENT_VERSION_MINOR, 0, 0);
    sess_ = std::unique_ptr<libtorrent::session>(new libtorrent::session(fingerprint, 0));
}

Session::~Session()
{
}

void Session::load()
{
    Application& app = Application::instance();

    // Add default extensions.
    sess_->add_extension(&libtorrent::create_lt_trackers_plugin);
    sess_->add_extension(&libtorrent::create_smart_ban_plugin);
    sess_->add_extension(&libtorrent::create_ut_metadata_plugin);
    sess_->add_extension(&libtorrent::create_ut_pex_plugin);

    // Set session settings
    libtorrent::session_settings settings = sess_->settings();
    settings.auto_scrape_min_interval = 60 * 15; // 15 minutes
    settings.auto_scrape_interval = 60 * 20; // 20 minutes
    settings.lazy_bitfields = true;
    settings.stop_tracker_timeout = 1;
    settings.upnp_ignore_nonrouters = true;

    // Anonymous mode
    settings.anonymous_mode = config_.getBool("bittorrent.anonymousMode", false);

    sess_->set_settings(settings);

    loadSessionState();
    loadResumeData();

    sess_->set_alert_mask(libtorrent::alert::all_categories);

    // Start DHT if enabled
    if (config_.getBool("bittorrent.dht.enabled", true))
    {
        sess_->start_dht();

        for (int i = 0; i < std::numeric_limits<int>::max(); i++)
        {
            std::string index = std::to_string(i);
            std::string query = "bittorrent.dht.routers[" + index + "]";

            if (config_.has(query))
            {
                const AbstractConfiguration* routerView = config_.createView(query);

                std::string url = routerView->getString("[0]");
                int port = routerView->getInt("[1]");

                sess_->add_dht_router(std::make_pair(url, port));
            }
            else
            {
                break;
            }
        }
    }

    // Start UPnP/NAT-PMP if enabled
    if (config_.getBool("bittorrent.upnp.enabled", true))
    {
        sess_->start_natpmp();
        sess_->start_upnp();

        // TODO: Add support for custom port mappings
    }

    // Set proxy if one is configured.
    if (config_.has("bittorrent.proxy"))
    {
        const AbstractConfiguration* proxyView = config_.createView("bittorrent.proxy");

        libtorrent::proxy_settings proxy = sess_->proxy();

        std::string type = proxyView->getString("type", "none");
        if (type == "none") proxy.type = libtorrent::proxy_settings::proxy_type::none;
        if (type == "socks4") proxy.type = libtorrent::proxy_settings::proxy_type::socks4;
        if (type == "socks5") proxy.type = libtorrent::proxy_settings::proxy_type::socks5;
        if (type == "socks5_pw") proxy.type = libtorrent::proxy_settings::proxy_type::socks5_pw;
        if (type == "http") proxy.type = libtorrent::proxy_settings::proxy_type::http;
        if (type == "http_pw") proxy.type = libtorrent::proxy_settings::proxy_type::http_pw;
        if (type == "i2p_proxy") proxy.type = libtorrent::proxy_settings::proxy_type::i2p_proxy;

        proxy.hostname = proxyView->getString("hostname", "");
        proxy.password = proxyView->getString("password", "");
        proxy.port = proxyView->getInt("port", 0);
        proxy.proxy_hostnames = proxyView->getBool("proxyHostnames", true);
        proxy.proxy_peer_connections = proxyView->getBool("proxyPeerConnections", true);
        proxy.username = proxyView->getString("username", "");

        sess_->set_proxy(proxy);
    }

    isRunning_ = true;
    alertReader_ = std::thread(std::bind(&Session::readAlerts, this));
}

void Session::loadSessionState()
{
    // Load session state
    Poco::Path data_path = getDataPath();

    Poco::Path state_path(data_path, ".session_state");
    Poco::File state_file(state_path);

    if (!state_file.exists())
    {
        logger_.information("No previous session state to load.");
        return;
    }

    if (!state_file.isFile())
    {
        logger_.error("State file is not a regular file.");
        return;
    }

    std::vector<char> buffer((unsigned int)state_file.getSize());

    std::ifstream state_file_stream(state_file.path(), std::ios::binary);
    state_file_stream.read(buffer.data(), buffer.size());

    // Load session state
    libtorrent::error_code ec;
    libtorrent::lazy_entry entry;
    libtorrent::lazy_bdecode(&buffer[0], &buffer[0] + buffer.size(), entry, ec);

    if (ec)
    {
        logger_.error("Could not load session state from file: %s.", ec.message());
        return;
    }

    sess_->load_state(entry);

    logger_.information("Loaded session state from %s", state_file.path());
}

void Session::loadResumeData()
{
    // Load existing torrents
    Poco::Path data_path = getDataPath();

    Poco::Path torrents_path(data_path, "torrents");
    Poco::File torrents_dir(torrents_path);

    if (!torrents_dir.exists())
    {
        logger_.information("Creating torrents path " + torrents_dir.path());
        torrents_dir.createDirectories();
    }

    if (!torrents_dir.isDirectory())
    {
        logger_.error("%s is not a directory.", torrents_dir.path());
        return;
    }

    std::vector<Poco::File> torrent_files;
    torrents_dir.list(torrent_files);

    for (Poco::File file : torrent_files)
    {
        Poco::Path torrent_file_path(file.path());
        if (torrent_file_path.getExtension() != "torrent") continue;

        libtorrent::add_torrent_params params = getDefaultAddTorrentParams();

        try 
        {
            params.ti = new libtorrent::torrent_info(file.path());
        }
        catch (const libtorrent::libtorrent_exception& ex)
        {
            logger_.error("Could not load torrent '%s': %s", file, ex.error().message());
            continue;
        }

        logger_.information("Loading torrent '%s'.", params.ti->name());

        // Check if resume data exists
        Poco::File torrent_state_file(torrent_file_path.setExtension("resume"));

        if (torrent_state_file.exists() && torrent_state_file.isFile())
        {
            std::vector<char> resume_buffer((unsigned int)torrent_state_file.getSize());

            std::ifstream state_stream(torrent_state_file.path(), std::ios::binary);
            state_stream.read(resume_buffer.data(), resume_buffer.size());

            params.resume_data = resume_buffer;

            logger_.debug("Loaded resume data for '%s'.", params.ti->name());
        }

        sess_->async_add_torrent(params);
    }
}

void Session::loadHadoukenState(const libtorrent::torrent_handle& handle, const libtorrent::lazy_entry& entry)
{
    if (entry.type() != libtorrent::lazy_entry::entry_type_t::dict_t)
    {
        logger_.error("Hadouken-specific state was not of the correct 'dict' type.");
        return;
    }

    int64_t stateVersion = entry.dict_find_int_value("hadouken-state-version", -1);

    if (stateVersion < 0)
    {
        logger_.error("Could not read state version. Found %ld.", stateVersion);
        return;
    }

    if (stateVersion != 1)
    {
        logger_.error("State version is not correct. Should be 1 but found %ld.", stateVersion);
        return;
    }

    const libtorrent::lazy_entry* meta = entry.dict_find_dict("metadata");
    std::string hash = libtorrent::to_hex(handle.info_hash().to_string());

    if (meta)
    {
        if (torrentMetadata_.find(hash) == torrentMetadata_.end())
        {
            // Make sure we have a map for this info hash
            torrentMetadata_.insert(std::make_pair(hash, std::map<std::string, std::string>()));
        }

        for (int i = 0; i < meta->dict_size(); i++)
        {
            std::pair<std::string, const libtorrent::lazy_entry*> val = meta->dict_at(i);

            if (val.second->type() != libtorrent::lazy_entry::entry_type_t::string_t)
            {
                logger_.error("Found invalid value type in metadata dictionary.");
                continue;
            }

            torrentMetadata_[hash][val.first] = val.second->string_value();
        }
    }
}

void Session::unload()
{
    // Join the thread that reads alerts.
    isRunning_ = false;
    alertReader_.join();
    
    saveSessionState();
    saveResumeData();
}

std::string Session::addTorrentFile(std::string path, AddTorrentParams& params)
{
    std::ifstream fileStream(path, std::ios::binary);
    std::vector<char> data((std::istreambuf_iterator<char>(fileStream)), std::istreambuf_iterator<char>());

    return addTorrent(data, params);
}

std::string Session::addTorrent(std::vector<char>& buffer, AddTorrentParams& params)
{
    libtorrent::error_code ec;
    libtorrent::lazy_entry le;
    libtorrent::lazy_bdecode(&buffer[0], &buffer[0] + buffer.size(), le, ec);

    if (ec)
    {
        logger_.error("Error when bdecoding torrent: %s", ec.message());
        return std::string();
    }

    libtorrent::add_torrent_params p = getDefaultAddTorrentParams();

    if (!params.savePath.empty())
    {
        p.save_path = params.savePath;
    }
    
    try
    {
        p.ti = new libtorrent::torrent_info(le);
    }
    catch (const libtorrent::libtorrent_exception& ex)
    {
        logger_.error("Could not load torrent info from bencoded data: %s.", ex.error().message());
        return std::string();
    }

    // TODO: save extra data somewhere

    sess_->async_add_torrent(p);
    return libtorrent::to_hex(p.ti->info_hash().to_string());
}

void Session::addTorrentUri(std::string uri, AddTorrentParams& params)
{
    libtorrent::add_torrent_params p = getDefaultAddTorrentParams();

    if (!params.savePath.empty())
    {
        p.save_path = params.savePath;
    }

    p.url = uri;

    sess_->async_add_torrent(p);
}

std::shared_ptr<TorrentHandle> Session::findTorrent(const std::string& infoHash) const
{
    libtorrent::sha1_hash hash;
    libtorrent::from_hex(infoHash.c_str(), infoHash.size(), (char*)&hash[0]);

    libtorrent::torrent_handle handle = sess_->find_torrent(hash);
    return std::shared_ptr<TorrentHandle>(new TorrentHandle(handle));
}

uint16_t Session::getListenPort() const
{
    return sess_->listen_port();
}

uint16_t Session::getSslListenPort() const
{
    return sess_->ssl_listen_port();
}

std::vector<std::shared_ptr<TorrentHandle>> Session::getTorrents() const
{
    std::vector<std::shared_ptr<TorrentHandle>> th;

    for (libtorrent::torrent_handle handle : sess_->get_torrents())
    {
        th.push_back(std::shared_ptr<TorrentHandle>(new TorrentHandle(handle)));
    }

    return th;
}

std::string Session::getLibtorrentVersion() const
{
    return std::string(LIBTORRENT_VERSION);
}

ProxySettings Session::getProxy() const
{
    return ProxySettings(sess_->proxy());
}

SessionStatus Session::getStatus() const
{
    libtorrent::session_status status = sess_->status();
    return SessionStatus(status);
}

bool Session::isListening() const
{
    return sess_->is_listening();
}

bool Session::isPaused() const
{
    return sess_->is_paused();
}

void Session::pause()
{
    sess_->pause();
}

void Session::removeTorrent(std::shared_ptr<TorrentHandle> handle, int options) const
{
    sess_->remove_torrent(handle->handle_, options);
}

void Session::resume()
{
    sess_->resume();
}

void Session::setProxy(ProxySettings& proxy)
{
    sess_->set_proxy(proxy.settings_);
}

std::string Session::getTorrentMetadata(std::string infoHash, std::string key)
{
    if (torrentMetadata_.find(infoHash) == torrentMetadata_.end())
    {
        return std::string();
    }

    if (torrentMetadata_[infoHash].find(key) == torrentMetadata_[infoHash].end())
    {
        return std::string();
    }

    return torrentMetadata_[infoHash][key];
}

std::vector<std::string> Session::getTorrentMetadataKeys(std::string infoHash)
{
    std::vector<std::string> result;

    if (torrentMetadata_.find(infoHash) == torrentMetadata_.end())
    {
        return result;
    }

    for (std::pair<std::string, std::string> p : torrentMetadata_[infoHash])
    {
        result.push_back(p.first);
    }

    return result;
}

void Session::setTorrentMetadata(std::string infoHash, std::string key, std::string value)
{
    if (torrentMetadata_.find(infoHash) == torrentMetadata_.end())
    {
        torrentMetadata_.insert(std::make_pair(infoHash, std::map<std::string, std::string>()));
    }

    torrentMetadata_[infoHash][key] = value;
}

void Session::saveSessionState()
{
    // Save session state
    Poco::Path data_path = getDataPath();

    Poco::Path state_path(data_path, ".session_state");
    Poco::File state_file(state_path);

    libtorrent::entry entry;
    sess_->save_state(entry);

    std::ofstream state_file_stream(state_file.path(), std::ios::binary);
    libtorrent::bencode(std::ostream_iterator<char>(state_file_stream), entry);
}

void Session::saveResumeData()
{
    sess_->pause();

    Poco::Path torrents_path = getTorrentsPath();
    if (torrents_path.toString().empty()) { return; }

    int num = 0;

    std::vector<libtorrent::torrent_status> temp;
    sess_->get_torrent_status(&temp, [](const libtorrent::torrent_status) { return true; });

    for (auto &status : temp)
    {
        if (!status.handle.is_valid()) continue;
        if (!status.has_metadata) continue;
        if (!status.need_save_resume) continue;

        ++num;
        status.handle.save_resume_data();
    }

    logger_.information("Saving resume data for %d torrents.", num);

    while (num > 0)
    {
        libtorrent::alert const* alert = sess_->wait_for_alert(libtorrent::seconds(10));
        if (alert == 0) continue;

        std::deque<libtorrent::alert*> alerts;
        sess_->pop_alerts(&alerts);

        for (auto &alert : alerts)
        {
            std::unique_ptr<libtorrent::alert> a(alert);

            if (libtorrent::alert_cast<libtorrent::torrent_paused_alert>(alert))
            {
                continue;
            }

            if (libtorrent::alert_cast<libtorrent::save_resume_data_failed_alert>(alert))
            {
                --num;
                continue;
            }

            const libtorrent::save_resume_data_alert* rd = libtorrent::alert_cast<libtorrent::save_resume_data_alert>(alert);

            if (!rd) continue;
            --num;
            if (!rd->resume_data) continue;

            // Save any specific Hadouken state
            libtorrent::entry::dictionary_type hdkn;
            saveHadoukenState(rd->handle, hdkn);
            rd->resume_data->dict().insert(std::make_pair("hdkn", hdkn));

            std::vector<char> out;
            libtorrent::bencode(std::back_inserter(out), *rd->resume_data);

            logger_.information("Saving state for %s.", rd->handle.torrent_file()->name());

            // Path to state file
            std::string hash = libtorrent::to_hex(rd->handle.info_hash().to_string());
            Poco::Path torrent_state_path(torrents_path, hash + ".resume");

            std::ofstream torrent_state_stream(torrent_state_path.toString(), std::ios::binary);
            torrent_state_stream.write(out.data(), out.size());
        }
    }
}

void Session::saveHadoukenState(const libtorrent::torrent_handle& handle, libtorrent::entry::dictionary_type& entry)
{
    if (!handle.is_valid())
    {
        logger_.error("Invalid torrent handle.");
        return;
    }

    entry.insert(std::make_pair("hadouken-state-version", 1));

    std::string hash = libtorrent::to_hex(handle.info_hash().to_string());

    if (torrentMetadata_.find(hash) == torrentMetadata_.end())
    {
        return;
    }

    libtorrent::entry::dictionary_type meta;
    
    for (std::pair<std::string, std::string> data : torrentMetadata_[hash])
    {
        meta.insert(data);
    }

    entry.insert(std::make_pair("metadata", meta));
}

void Session::readAlerts()
{
    using namespace libtorrent;

    // Setting the following to true in the configuration will print all alerts
    // to the trace log.
    bool traceAlerts = config_.getBool("bittorrent.tracingEnabled", false);

    Application& app = Application::instance();
    ScriptingSubsystem& scripting = app.getSubsystem<ScriptingSubsystem>();

    while (isRunning_)
    {
        const alert* found_alert = sess_->wait_for_alert(seconds(1));
        if (!found_alert) continue;

        std::deque<alert*> alerts;
        sess_->pop_alerts(&alerts);

        for (auto &alert : alerts)
        {
            std::unique_ptr<libtorrent::alert> a(alert);

            if (traceAlerts)
            {
                logger_.information("%s", a->message());
            }

            std::string alertName = a->what();
            std::unique_ptr<Event> alertData;

            switch (a->type())
            {
            case add_torrent_alert::alert_type:
            {
                add_torrent_alert* ata = alert_cast<add_torrent_alert>(alert);

                if (ata->error)
                {
                    logger_.error("Could not add torrent '%s': %s.", ata->params.ti->name(), ata->error.message());
                    break;
                }

                // Load Hadouken-specific metadata from resume buffer
                if (ata->params.resume_data.size() > 0)
                {
                    libtorrent::lazy_entry state;
                    libtorrent::error_code ec;
                    libtorrent::lazy_bdecode(&ata->params.resume_data[0], &ata->params.resume_data[0] + ata->params.resume_data.size(), state, ec);

                    if (ec)
                    {
                        logger_.error("Could not bdecode resume buffer to read Hadouken state: %s.", ec.message());
                        break;
                    }

                    const libtorrent::lazy_entry* hdkn = state.dict_find_dict("hdkn");

                    if (hdkn)
                    {
                        loadHadoukenState(ata->handle, *hdkn);
                    }
                }
                break;
            }

            case block_downloading_alert::alert_type:
            {
                block_downloading_alert* bda = alert_cast<block_downloading_alert>(alert);
                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(bda->handle));

                alertData.reset(new BlockEvent(handle, bda->ip.address().to_string(), bda->ip.port(), bda->piece_index, bda->block_index));
                break;
            }

            case block_finished_alert::alert_type:
            {
                block_finished_alert* bfa = alert_cast<block_finished_alert>(alert);
                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(bfa->handle));

                alertData.reset(new BlockEvent(handle, bfa->ip.address().to_string(), bfa->ip.port(), bfa->piece_index, bfa->block_index));
                break;
            }

            case block_timeout_alert::alert_type:
            {
                block_timeout_alert* bta = alert_cast<block_timeout_alert>(alert);
                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(bta->handle));

                alertData.reset(new BlockEvent(handle, bta->ip.address().to_string(), bta->ip.port(), bta->piece_index, bta->block_index));
                break;
            }

            case cache_flushed_alert::alert_type:
            {
                cache_flushed_alert* cfa = alert_cast<cache_flushed_alert>(alert);
                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(cfa->handle));

                alertData.reset(new TorrentEvent(handle));
                break;
            }

            case dht_bootstrap_alert::alert_type:
            {
                alertData.reset(new EmptyEvent);
                break;
            }

            case dht_reply_alert::alert_type:
            {
                dht_reply_alert* dra = alert_cast<dht_reply_alert>(alert);
                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(dra->handle));

                alertData.reset(new DhtReplyEvent(handle, dra->url, dra->num_peers));
                break;
            }

            case external_ip_alert::alert_type:
            {
                external_ip_alert* eia = alert_cast<external_ip_alert>(alert);
                alertData.reset(new ExternalAddressEvent(eia->external_address.to_string()));
                break;
            }

            case file_completed_alert::alert_type:
            {
                file_completed_alert* fca = alert_cast<file_completed_alert>(alert);
                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(fca->handle));

                alertData.reset(new FileCompletedEvent(handle, fca->index));
                break;
            }

            case file_error_alert::alert_type:
            {
                file_error_alert* fea = alert_cast<file_error_alert>(alert);
                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(fea->handle));
                Error err(fea->error.value(), fea->error.message());

                alertData.reset(new FileErrorEvent(handle, err, fea->file));
                break;
            }

            case file_renamed_alert::alert_type:
            {
                file_renamed_alert* fra = alert_cast<file_renamed_alert>(alert);
                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(fra->handle));

                alertData.reset(new FileRenamedEvent(handle, fra->index, fra->name));
                break;
            }
            
            case file_rename_failed_alert::alert_type:
            {
                file_rename_failed_alert* frfa = alert_cast<file_rename_failed_alert>(alert);
                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(frfa->handle));
                Error err(frfa->error.value(), frfa->error.message());

                alertData.reset(new FileRenameFailedEvent(handle, frfa->index, err));
                break;
            }

            case hash_failed_alert::alert_type:
            {
                hash_failed_alert* hfa = alert_cast<hash_failed_alert>(alert);
                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(hfa->handle));
                
                alertData.reset(new HashFailedEvent(handle, hfa->piece_index));
                break;
            }

            case incoming_connection_alert::alert_type:
            {
                incoming_connection_alert* ica = alert_cast<incoming_connection_alert>(alert);
                alertData.reset(new IncomingConnectionEvent(ica->ip.address().to_string(), ica->ip.port()));
                break;
            }

            case listen_succeeded_alert::alert_type:
            {
                listen_succeeded_alert* lsa = alert_cast<listen_succeeded_alert>(alert);
                alertData.reset(new ListenSucceededEvent(lsa->endpoint.address().to_string(), lsa->endpoint.port(), lsa->sock_type));
                break;
            }

            case metadata_failed_alert::alert_type:
            {
                metadata_failed_alert* mfa = alert_cast<metadata_failed_alert>(alert);
                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(mfa->handle));
                Error err(mfa->error.value(), mfa->error.message());

                alertData.reset(new MetadataFailedEvent(handle, err));
                break;
            }
            
            case lsd_peer_alert::alert_type:
            case peer_ban_alert::alert_type:
            case peer_connect_alert::alert_type:
            case peer_disconnected_alert::alert_type:
            case peer_snubbed_alert::alert_type:
            case peer_unsnubbed_alert::alert_type:
            {
                peer_alert* pa = static_cast<peer_alert*>(alert);
                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(pa->handle));

                alertData.reset(new EmptyPeerEvent(handle, pa->ip.address().to_string(), pa->ip.port()));
                break;
            }

            case peer_error_alert::alert_type:
            {
                peer_error_alert* pea = alert_cast<peer_error_alert>(alert);
                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(pea->handle));
                Error err(pea->error.value(), pea->error.message());

                alertData.reset(new PeerErrorEvent(handle, pea->ip.address().to_string(), pea->ip.port(), err));
                break;
            }
            
            case performance_alert::alert_type:
            {
                performance_alert* pa = alert_cast<performance_alert>(alert);
                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(pa->handle));

                alertData.reset(new PerformanceEvent(handle, pa->warning_code));
                break;
            }

            case piece_finished_alert::alert_type:
            {
                piece_finished_alert* pfa = alert_cast<piece_finished_alert>(alert);
                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(pfa->handle));

                alertData.reset(new PieceFinishedEvent(handle, pfa->piece_index));
                break;
            }

            case request_dropped_alert::alert_type:
            {
                request_dropped_alert* rda = alert_cast<request_dropped_alert>(alert);
                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(rda->handle));

                alertData.reset(new BlockEvent(handle, rda->ip.address().to_string(), rda->ip.port(), rda->piece_index, rda->block_index));
                break;
            }

            case scrape_failed_alert::alert_type:
            {
                scrape_failed_alert* sfa = alert_cast<scrape_failed_alert>(alert);
                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(sfa->handle));

                alertData.reset(new ScrapeFailedEvent(handle, sfa->url, sfa->msg));
                break;
            }

            case scrape_reply_alert::alert_type:
            {
                scrape_reply_alert* sra = alert_cast<scrape_reply_alert>(alert);
                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(sra->handle));

                alertData.reset(new ScrapeReplyEvent(handle, sra->url, sra->complete, sra->incomplete));
                break;
            }

            case state_changed_alert::alert_type:
            {
                state_changed_alert* sca = alert_cast<state_changed_alert>(alert);
                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(sca->handle));

                alertData.reset(new StateChangedEvent(handle, sca->state, sca->prev_state));
                break;
            }
            
            case stats_alert::alert_type:
            {
                stats_alert* sa = alert_cast<stats_alert>(alert);
                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(sa->handle));

                alertData.reset(new StatsEvent(handle, sa->interval, sa->transferred));
                break;
            }

            case storage_moved_alert::alert_type:
            {
                storage_moved_alert* sma = alert_cast<storage_moved_alert>(alert);
                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(sma->handle));

                alertData.reset(new StorageMovedEvent(handle, sma->path));
                break;
            }

            case storage_moved_failed_alert::alert_type:
            {
                storage_moved_failed_alert* smfa = alert_cast<storage_moved_failed_alert>(alert);
                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(smfa->handle));

                Error err(smfa->error.value(), smfa->error.message());
                alertData.reset(new StorageMoveFailedEvent(handle, err));
                break;
            }

            case torrent_added_alert::alert_type:
            {
                torrent_added_alert* taa = alert_cast<torrent_added_alert>(alert);

                if (taa->handle.torrent_file())
                {
                    saveTorrentInfo(*taa->handle.torrent_file());
                }

                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(taa->handle));
                alertData.reset(new TorrentEvent(handle));
                onTorrentAdded(this, handle);
                break;
            }

            case torrent_checked_alert::alert_type:
            {
                torrent_checked_alert* tca = alert_cast<torrent_checked_alert>(alert);
                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(tca->handle));

                alertData.reset(new TorrentEvent(handle));
                break;
            }

            case torrent_delete_failed_alert::alert_type:
            {
                torrent_delete_failed_alert* tdfa = alert_cast<torrent_delete_failed_alert>(alert);
                std::string hash = to_hex(tdfa->info_hash.to_string());
                Error err(tdfa->error.value(), tdfa->error.message());

                alertData.reset(new TorrentDeleteFailedEvent(hash, err));
                break;
            }

            case torrent_deleted_alert::alert_type:
            {
                torrent_deleted_alert* tda = alert_cast<torrent_deleted_alert>(alert);
                std::string hash = to_hex(tda->info_hash.to_string());

                alertData.reset(new TorrentDeletedEvent(hash));
                break;
            }

            case torrent_error_alert::alert_type:
            {
                torrent_error_alert* tea = alert_cast<torrent_error_alert>(alert);
                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(tea->handle));
                Error err(tea->error.value(), tea->error.message());

                alertData.reset(new TorrentErrorEvent(handle, err));
                break;
            }

            case torrent_finished_alert::alert_type:
            {
                torrent_finished_alert* tfa = alert_cast<torrent_finished_alert>(alert);
                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(tfa->handle));

                alertData.reset(new TorrentEvent(handle));
                onTorrentFinished.notifyAsync(this, handle);
                break;
            }

            case torrent_need_cert_alert::alert_type:
            {
                torrent_need_cert_alert* tnca = alert_cast<torrent_need_cert_alert>(alert);
                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(tnca->handle));
                Error err(tnca->error.value(), tnca->error.message());

                alertData.reset(new TorrentErrorEvent(handle, err));
                break;
            }

            case torrent_paused_alert::alert_type:
            {
                torrent_paused_alert* tpa = alert_cast<torrent_paused_alert>(alert);
                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(tpa->handle));

                alertData.reset(new TorrentEvent(handle));
                break;
            }

            case torrent_removed_alert::alert_type:
            {
                torrent_removed_alert* removed_alert = alert_cast<torrent_removed_alert>(alert);

                std::string hash = to_hex(removed_alert->info_hash.to_string());
                Poco::Path torrents_path = getTorrentsPath();

                if (!torrents_path.toString().empty())
                {
                    Poco::Path torrent_file_path(torrents_path, hash + ".torrent");
                    Poco::File torrent_file(torrent_file_path);
                    if (torrent_file.exists()) { torrent_file.remove(); }

                    Poco::Path resume_file_path(torrents_path, hash + ".resume");
                    Poco::File resume_file(resume_file_path);
                    if (resume_file.exists()) { resume_file.remove(); }
                }

                alertData.reset(new TorrentRemovedEvent(hash));
                onTorrentRemoved.notifyAsync(this, hash);
                break;
            }

            case torrent_resumed_alert::alert_type:
            {
                torrent_resumed_alert* tpa = alert_cast<torrent_resumed_alert>(alert);
                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(tpa->handle));

                alertData.reset(new TorrentEvent(handle));
                break;
            }

            case torrent_update_alert::alert_type:
            {
                // A torrent downloaded from a URL has changed its info hash.
                torrent_update_alert* update_alert = alert_cast<torrent_update_alert>(alert);
                // TODO
                break;
            }

            case tracker_error_alert::alert_type:
            {
                tracker_error_alert* tea = alert_cast<tracker_error_alert>(alert);
                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(tea->handle));
                Error err(tea->error.value(), tea->error.message());
                
                alertData.reset(new TrackerErrorEvent(handle, err, tea->url, tea->times_in_row, tea->status_code, tea->msg));
                break;
            }

            case tracker_announce_alert::alert_type:
            {
                tracker_announce_alert* taa = alert_cast<tracker_announce_alert>(alert);
                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(taa->handle));

                alertData.reset(new TrackerAnnounceEvent(handle, taa->url, taa->event));
                break;
            }

            case trackerid_alert::alert_type:
            {
                trackerid_alert* ta = alert_cast<trackerid_alert>(alert);
                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(ta->handle));

                alertData.reset(new TrackerIdEvent(handle, ta->url, ta->trackerid));
                break;
            }

            case tracker_reply_alert::alert_type:
            {
                tracker_reply_alert* tra = alert_cast<tracker_reply_alert>(alert);
                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(tra->handle));

                alertData.reset(new TrackerReplyEvent(handle, tra->url, tra->num_peers));
                break;
            }

            case tracker_warning_alert::alert_type:
            {
                tracker_warning_alert* twa = alert_cast<tracker_warning_alert>(alert);
                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(twa->handle));

                alertData.reset(new TrackerWarningEvent(handle, twa->url, twa->msg));
                break;
            }

            case unwanted_block_alert::alert_type:
            {
                unwanted_block_alert* uba = alert_cast<unwanted_block_alert>(alert);
                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(uba->handle));

                alertData.reset(new BlockEvent(handle, uba->ip.address().to_string(), uba->ip.port(), uba->piece_index, uba->block_index));
                break;
            }

            case url_seed_alert::alert_type:
            {
                url_seed_alert* usa = alert_cast<url_seed_alert>(alert);
                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(usa->handle));

                alertData.reset(new UrlSeedEvent(handle, usa->url, usa->msg));
                break;
            }

            case metadata_received_alert::alert_type:
            {
                metadata_received_alert* mra = alert_cast<metadata_received_alert>(alert);
                saveTorrentInfo(*mra->handle.torrent_file());
                std::shared_ptr<TorrentHandle> handle(new TorrentHandle(mra->handle));

                alertData.reset(new TorrentEvent(handle));
                break;
            }
            }

            if (alertData)
            {
                scripting.emit(alertName, std::move(alertData));
            }
        }
    }

    logger_.information("Shutting down read alerts thread.");
}

void Session::saveTorrentInfo(const libtorrent::torrent_info& info)
{
    if (!info.is_valid())
    {
        return;
    }

    Poco::Path torrents_path = getTorrentsPath();
    std::string hash = libtorrent::to_hex(info.info_hash().to_string());

    // If the torrent file exists, do nothing.
    Poco::Path torrent_file_path(torrents_path, hash + ".torrent");

    if (Poco::File(torrent_file_path).exists())
    {
        return;
    }

    libtorrent::create_torrent creator(info);
    libtorrent::entry ent = creator.generate();

    std::vector<char> out;
    libtorrent::bencode(std::back_inserter(out), ent);

    std::ofstream torrent_state_stream(torrent_file_path.toString(), std::ios::binary);
    torrent_state_stream.write(out.data(), out.size());
}

libtorrent::add_torrent_params Session::getDefaultAddTorrentParams()
{
    libtorrent::add_torrent_params p;
    p.flags |= libtorrent::add_torrent_params::flags_t::flag_use_resume_save_path;
    p.save_path = default_save_path_;

    /*
    Set storage mode. This is configured in the properties
    and defaults to sparse files.
    */

    if (config_.getBool("bittorrent.storage.sparse", true))
    {
        p.storage_mode = libtorrent::storage_mode_t::storage_mode_sparse;
    }
    else
    {
        p.storage_mode = libtorrent::storage_mode_t::storage_mode_allocate;
    }

    return p;
}

Poco::Path Session::getDataPath()
{
    std::string configured_data_path = config_.getString("bittorrent.statePath", "state");

    Poco::Path data_path(configured_data_path);
    Poco::File data_dir(data_path);

    if (!data_dir.exists())
    {
        data_dir.createDirectories();
    }

    if (!data_dir.isDirectory())
    {
        return Poco::Path();
    }

    return data_path;
}

Poco::Path Session::getTorrentsPath()
{
    Poco::Path data_path = getDataPath();

    Poco::Path torrents_path(data_path, "torrents");
    Poco::File torrents_dir(torrents_path);

    if (!torrents_dir.exists())
    {
        logger_.information("Creating torrents path " + torrents_dir.path());
        torrents_dir.createDirectories();
    }

    if (!torrents_dir.isDirectory())
    {
        logger_.error("%s is not a directory.", torrents_dir.path());
        return Poco::Path();
    }

    return torrents_path;
}
