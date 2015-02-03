#include "folder_watcher.hpp"

#include <boost/bind.hpp>
#include <boost/date_time.hpp>
#include <boost/filesystem.hpp>
#include <boost/log/trivial.hpp>
#include <boost/property_tree/ptree.hpp>
#include <boost/property_tree/json_parser.hpp>

folder_watcher::folder_watcher(hadouken::service_locator& service_locator)
    : plugin(service_locator)
{
    boost::asio::io_service* io_service = service_locator.request<boost::asio::io_service*>("io_service");

    sess_ = service_locator.request<hadouken::bittorrent::session*>("bt.session");
    timer_ = new boost::asio::deadline_timer(*io_service, boost::posix_time::seconds(5));
}

void folder_watcher::load()
{
    load_configuration();

    BOOST_LOG_TRIVIAL(debug) << "Loading folder watcher";
    timer_->async_wait(boost::bind(&folder_watcher::timer_callback, this, _1));
}

void folder_watcher::unload()
{
    BOOST_LOG_TRIVIAL(debug) << "Unloading folder watcher";
    timer_->cancel();
}

void folder_watcher::load_configuration()
{
    namespace fs = boost::filesystem;

    fs::path config_path("config/folder_watcher.json");

    if (!fs::exists(config_path))
    {
        BOOST_LOG_TRIVIAL(warning) << "No \"folder_watcher.json\" at " << fs::current_path() / "config";
        return;
    }

    boost::property_tree::ptree pt;
    boost::property_tree::read_json(config_path.string(), pt);

    for (auto child : pt)
    {
        std::string source = child.second.get<std::string>("source_folder");
        std::string save_path = child.second.get<std::string>("save_path");

        folders_.insert(std::make_pair(source, save_path));
    }
}

void folder_watcher::timer_callback(const boost::system::error_code& error)
{
    if (error && error == boost::asio::error::operation_aborted)
    {
        BOOST_LOG_TRIVIAL(trace) << "Cancelling timer callback.";
        return;
    }

    if (folders_.empty())
    {
        return;
    }

    for (auto it : folders_)
    {
        BOOST_LOG_TRIVIAL(debug) << "Checking " << it.first;
        add_torrents_from_folder(it.first, it.second);
    }

    timer_->expires_at(timer_->expires_at() + boost::posix_time::seconds(5));
    timer_->async_wait(boost::bind(&folder_watcher::timer_callback, this, _1));
}

void folder_watcher::add_torrents_from_folder(const std::string& folder, const std::string& save_path)
{
    namespace fs = boost::filesystem;
    fs::path p(folder);

    if (!fs::exists(p)) return;

    for (auto entry : fs::directory_iterator(p))
    {
        if (!fs::is_regular_file(entry)) continue;
        if (entry.path().extension() != ".torrent") continue;

        sess_->add_torrent_file(entry.path().string(), save_path);

        // Remove file after adding
        fs::remove(entry.path());
    }
}