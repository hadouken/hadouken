#include "folder_watcher.hpp"

#include <boost/bind.hpp>
#include <boost/date_time.hpp>
#include <boost/filesystem.hpp>
#include <boost/log/trivial.hpp>

folder_watcher::folder_watcher(hadouken::service_locator& service_locator)
    : plugin(service_locator)
{
    boost::asio::io_service* io_service = service_locator.request<boost::asio::io_service*>("io_service");

    sess_ = service_locator.request<hadouken::bittorrent::session*>("bt.session");
    timer_ = new boost::asio::deadline_timer(*io_service, boost::posix_time::seconds(5));
}

void folder_watcher::load()
{
    BOOST_LOG_TRIVIAL(debug) << "Loading folder watcher";
    timer_->async_wait(boost::bind(&folder_watcher::timer_callback, this, _1));
}

void folder_watcher::unload()
{
    BOOST_LOG_TRIVIAL(debug) << "Unloading folder watcher";
    timer_->cancel();
}

void folder_watcher::timer_callback(const boost::system::error_code& error)
{
    if (error && error == boost::asio::error::operation_aborted)
    {
        BOOST_LOG_TRIVIAL(trace) << "Cancelling timer callback.";
        return;
    }

    BOOST_LOG_TRIVIAL(debug) << "Checking watched folders";
    add_torrents_from_folder("C:\\Temp\\Torrents");

    timer_->expires_at(timer_->expires_at() + boost::posix_time::seconds(5));
    timer_->async_wait(boost::bind(&folder_watcher::timer_callback, this, _1));
}

void folder_watcher::add_torrents_from_folder(const std::string& folder)
{
    namespace fs = boost::filesystem;
    fs::path p(folder);

    if (!fs::exists(p)) return;

    for (auto entry : fs::directory_iterator(p))
    {
        if (!fs::is_regular_file(entry)) continue;
        if (entry.path().extension() != ".torrent") continue;

        sess_->add_torrent_file(entry.path().string(), "C:\\Downloads");

        // Remove file after adding
        fs::remove(entry.path());
    }
}