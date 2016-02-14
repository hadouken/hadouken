#include <hadouken/hosting/console_host.hpp>

using namespace hadouken::hosting;

int console_host::initialization_start(boost::shared_ptr<boost::asio::io_service> io)
{
    io_ = io;

    return EXIT_SUCCESS;
}

int console_host::initialization_complete(int success_code)
{
    return EXIT_SUCCESS;
}

int console_host::wait_for_exit()
{
    boost::asio::signal_set signals(*io_, SIGINT, SIGTERM);

    signals.async_wait([this](const boost::system::error_code& ec, int sig)
    {
        io_->stop();
    });

    io_->run();

    return EXIT_SUCCESS;
}

int console_host::shutdown_start()
{
    return EXIT_SUCCESS;
}

int console_host::shutdown_complete(int success_code)
{
    return EXIT_SUCCESS;
}
