#include "console_host.hpp"

#include <boost/asio.hpp>
#include <hadouken/bittorrent/session.hpp>
#include <hadouken/plugin_manager.hpp>

using namespace hadouken;
using namespace hadouken::bittorrent;

int console_host::run()
{
    boost::asio::io_service* service = loc_.request<boost::asio::io_service*>("io_service");
    boost::asio::signal_set signals(*service, SIGINT, SIGTERM);

    session* sess = loc_.request<session*>("bt.session");
    sess->load();

    plugin_manager* manager = loc_.request<plugin_manager*>("plugin_manager");
    manager->load();

    signals.async_wait([&sess, &manager](boost::system::error_code error, int signal)
    {
        manager->unload();
        sess->unload();
    });

    service->run();

    delete loc_.request<plugin_manager*>("plugin_manager");
    delete loc_.request<session*>("bt.session");
    delete service;

    return 0;
}