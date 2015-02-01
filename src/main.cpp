#include <boost/asio.hpp>

#include "plugin_manager.hpp"
#include "torrent_engine.hpp"

int main(int argc, char* argv[])
{
    using namespace hadouken;

    boost::asio::io_service io_service;
    boost::asio::signal_set signals(io_service, SIGINT, SIGTERM);

    torrent_engine* engine = new torrent_engine();
    engine->load();

    plugin_manager* manager = new plugin_manager();
    manager->load();

    signals.async_wait([&engine](boost::system::error_code error, int signal)
    {
        engine->unload();
    });

    io_service.run();

    delete manager;
    delete engine;

    return 0;
}