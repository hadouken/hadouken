#ifndef HADOUKEN_HOSTING_HOST_HPP
#define HADOUKEN_HOSTING_HOST_HPP

#include <boost/asio.hpp>

namespace hadouken
{
    namespace hosting
    {
        class host
        {
        public:
            virtual int wait_for_exit(boost::shared_ptr<boost::asio::io_service> io) = 0;
        };
    }
}

#endif
