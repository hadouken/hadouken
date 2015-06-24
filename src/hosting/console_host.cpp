#include <hadouken/hosting/console_host.hpp>

using namespace hadouken::hosting;

int console_host::wait_for_exit(boost::shared_ptr<boost::asio::io_service> io)
{
    boost::asio::signal_set signals(*io, SIGINT, SIGTERM);

    signals.async_wait([io](const boost::system::error_code& ec, int sig)
    {
        io->stop();
    });

    io->run();

    return 0;
}
