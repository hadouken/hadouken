#ifndef HDKN_HOST_HPP
#define HDKN_HOST_HPP

#include <boost/asio.hpp>

namespace hadouken
{
    class host
    {
    public:
        virtual int run(boost::asio::io_service& io_service) = 0;
    };
}

#endif