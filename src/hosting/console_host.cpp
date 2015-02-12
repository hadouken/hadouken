#include <hadouken/hosting/console_host.hpp>

using namespace hadouken;

int console_host::run(boost::asio::io_service& io_service)
{
    boost::asio::signal_set signals(io_service, SIGINT, SIGTERM);

    signals.async_wait([&io_service](boost::system::error_code error, int signal)
    {
        io_service.stop();
    });

    io_service.run();

    return 0;
}