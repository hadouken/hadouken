#ifndef HDKN_CONSOLE_HOST_HPP
#define HDKN_CONSOLE_HOST_HPP

#include <hadouken/hosting/host.hpp>

#include <boost/asio.hpp>

namespace hadouken
{
    class console_host : public host
    {
    public:
        int run(boost::asio::io_service& io_service);
    };
}

#endif