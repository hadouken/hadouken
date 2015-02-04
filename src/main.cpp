#include <boost/asio.hpp>

#include <hadouken/plugin_manager.hpp>
#include <hadouken/service_locator.hpp>
#include <hadouken/bittorrent/session.hpp>

int main(int argc, char* argv[])
{
    using namespace hadouken;
    using namespace hadouken::bittorrent;

    boost::asio::io_service* io_service = new boost::asio::io_service();
    boost::asio::signal_set signals(*io_service, SIGINT, SIGTERM);

    service_locator* locator = new service_locator();
    locator->add_service("bt.session", new session(*io_service));
    locator->add_service("io_service", io_service);

    session* sess = locator->request<session*>("bt.session");
    sess->load();

    plugin_manager* manager = new plugin_manager(*locator);
    manager->load();

    signals.async_wait([&sess, &manager](boost::system::error_code error, int signal)
    {
        manager->unload();
        sess->unload();
    });

    io_service->run();

    delete manager;
    delete sess;
    delete io_service;

    return 0;
}