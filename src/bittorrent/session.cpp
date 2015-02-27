#include <Hadouken/BitTorrent/Session.hpp>

#include <fstream>
#include <iostream>

#include <Hadouken/BitTorrent/AddTorrentParams.hpp>
#include <Hadouken/BitTorrent/TorrentHandle.hpp>
#include <libtorrent/alert_types.hpp>
#include <libtorrent/bencode.hpp>
#include <libtorrent/create_torrent.hpp>
#include <libtorrent/session.hpp>
#include <libtorrent/version.hpp>
#include <Poco/File.h>
#include <Poco/Path.h>
#include <Poco/Util/Application.h>
#include <Poco/RunnableAdapter.h>

using namespace Hadouken::BitTorrent;
using namespace Poco::Util;

Session::Session(const Poco::Util::LayeredConfiguration& config)
    : logger_(Poco::Logger::get("hadouken.bittorrent.session")),
      config_(config),
      read_alerts_runner_(*this, &Session::readAlerts)
{
    if (config_.has("bittorrent.default_save_path"))
    {
        default_save_path_ = config_.getString("bittorrent.default_save_path");
    }
    else
    {
        default_save_path_ = ".";
    }

    sess_ = new libtorrent::session();
}

Session::~Session()
{
    delete sess_;
}

void Session::load()
{
    Application& app = Application::instance();

    loadSessionState();
    loadResumeData();

    sess_->set_alert_mask(libtorrent::alert::all_categories);

    // Start alert reader thread
    read_alerts_ = true;
    read_alerts_thread_.start(read_alerts_runner_);
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

    Poco::Path torrents_path(data_path, "Torrents");
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
    std::vector<Poco::File>::iterator it = torrent_files.begin();

    for (; it != torrent_files.end(); ++it)
    {
        Poco::Path torrent_file_path(it->path());
        if (torrent_file_path.getExtension() != "torrent") continue;

        libtorrent::add_torrent_params params;
        libtorrent::error_code ec;
        params.ti = new libtorrent::torrent_info(it->path(), ec);

        if (ec)
        {
            logger_.error("Could not load torrent '%s': %s", it->path(), ec.message());
            continue;
        }

        logger_.information("Loading torrent '%s'.", params.ti->name());

        // Set defaults
        params.save_path = default_save_path_;

        // Check if resume data exists
        Poco::File torrent_state_file(torrent_file_path.setExtension(".resume"));

        if (torrent_state_file.exists() && torrent_state_file.isFile())
        {
            std::vector<char> resume_buffer((unsigned int)torrent_state_file.getSize());

            std::ifstream state_stream(torrent_state_file.path(), std::ios::binary);
            state_stream.read(resume_buffer.data(), resume_buffer.size());

            params.resume_data = resume_buffer;
        }

        sess_->async_add_torrent(params);
    }
}

void Session::unload()
{
    // Join the thread that reads alerts.
    read_alerts_ = false;
    read_alerts_thread_.join();

    saveSessionState();
    saveResumeData();
}

std::string Session::addTorrentFile(Poco::Path& filePath, AddTorrentParams& params)
{
    std::ifstream file_stream(filePath.toString(), std::ios::binary);
    std::vector<char> file_data((std::istreambuf_iterator<char>(file_stream)), std::istreambuf_iterator<char>());

    return addTorrentFile(file_data, params);
}

std::string Session::addTorrentFile(std::vector<char>& buffer, AddTorrentParams& params)
{
    libtorrent::error_code ec;
    libtorrent::lazy_entry le;
    libtorrent::lazy_bdecode(&buffer[0], &buffer[0] + buffer.size(), le, ec);

    if (ec)
    {
        logger_.error("Error when bdecoding torrent: %s", ec.message());
        return nullptr;
    }

    libtorrent::add_torrent_params p;

    if (!params.savePath.isDirectory())
    {
        p.save_path = default_save_path_;
    }
    else
    {
        p.save_path = params.savePath.toString();
    }

    /*
    Set storage mode. This is configured in the properties
    and defaults to sparse files.
    */

    if (config_.has("bittorrent.storage_allocate")
        && config_.getBool("bittorrent.storage_allocate"))
    {
        p.storage_mode = libtorrent::storage_mode_allocate;
    }
    else
    {
        p.storage_mode = libtorrent::storage_mode_sparse;
    }
    
    p.ti = new libtorrent::torrent_info(le);

    std::string hash = libtorrent::to_hex(p.ti->info_hash().to_string());

    sess_->async_add_torrent(p);

    return hash;
}

TorrentHandle Session::findTorrent(const std::string& infoHash) const
{
    libtorrent::sha1_hash hash;
    libtorrent::from_hex(infoHash.c_str(), infoHash.size(), (char*)&hash[0]);
    libtorrent::torrent_handle handle = sess_->find_torrent(hash);
    
    return TorrentHandle(handle);
}

std::vector<TorrentHandle> Session::getTorrents() const
{
    std::vector<libtorrent::torrent_handle> handles = sess_->get_torrents();
    std::vector<TorrentHandle> th;

    for (auto handle : handles)
    {
        th.push_back(TorrentHandle(handle));
    }

    return th;
}

std::string Session::getLibtorrentVersion() const
{
    return std::string(LIBTORRENT_VERSION);
}

void Session::removeTorrent(const TorrentHandle& handle, int options) const
{
    sess_->remove_torrent(handle.handle_, options);
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
    // Save resume data
    Poco::Path data_path = getDataPath();

    Poco::Path torrents_path(data_path, "Torrents");
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

    sess_->pause();

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

            std::vector<char> out;
            libtorrent::bencode(std::back_inserter(out), *rd->resume_data);

            std::string hash = libtorrent::to_hex(rd->handle.info_hash().to_string());

            logger_.information("Saving state for %s.", rd->handle.torrent_file()->name());

            // Path to state file
            Poco::Path torrent_state_path(torrents_path, hash + ".resume");

            std::ofstream torrent_state_stream(torrent_state_path.toString(), std::ios::binary);
            torrent_state_stream.write(out.data(), out.size());
        }
    }
}

void Session::readAlerts()
{
    // If you really want to have verbose output

    bool traceAlerts = (config_.hasProperty("bittorrent.trace_alerts")
        && config_.getBool("bittorrent.trace_alerts"));

    while (read_alerts_)
    {
        const libtorrent::alert* alert = sess_->wait_for_alert(libtorrent::seconds(1));
        if (!alert) continue;

        std::deque<libtorrent::alert*> alerts;
        sess_->pop_alerts(&alerts);

        for (auto &alert : alerts)
        {
            std::unique_ptr<libtorrent::alert> a(alert);

            if (traceAlerts)
            {
                logger_.trace("%s", a->message());
            }

            switch (a->type())
            {
            case libtorrent::torrent_added_alert::alert_type:
            {
                // save torrent file to state path if it doesn't
                // already exist there. then publish an added event.
                libtorrent::torrent_added_alert* added_alert = libtorrent::alert_cast<libtorrent::torrent_added_alert>(alert);
                saveTorrentInfo(*added_alert->handle.torrent_file());
                break;
            }

            case libtorrent::metadata_received_alert::alert_type:
            {
                libtorrent::metadata_received_alert* metadata_alert = libtorrent::alert_cast<libtorrent::metadata_received_alert>(alert);
                saveTorrentInfo(*metadata_alert->handle.torrent_file());
                break;
            }
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

    Poco::Path data_path = getDataPath();

    Poco::Path torrents_path(data_path, "Torrents");
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

Poco::Path Session::getDataPath()
{
    std::string configured_data_path = "state";

    if (config_.has("bittorrent.state_path"))
    {
        configured_data_path = config_.getString("bittorrent.state_path");
    }

    // Save session state
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
