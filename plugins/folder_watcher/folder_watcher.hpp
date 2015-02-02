#ifndef FOLDER_WATCHER_HPP
#define FOLDER_WATCHER_HPP

#include <hadouken/plugin.hpp>
#include <hadouken/bittorrent/session.hpp>

#include <boost/asio/deadline_timer.hpp>

class folder_watcher : public hadouken::plugin
{
public:
    folder_watcher(hadouken::service_locator& service_locator);

    void load();
    void unload();

private:
    void timer_callback(const boost::system::error_code& error);

    hadouken::bittorrent::session* sess_;
    boost::asio::deadline_timer* timer_;
};

HADOUKEN_PLUGIN(folder_watcher)

#endif
