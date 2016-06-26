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
            virtual int initialization_start(boost::shared_ptr<boost::asio::io_service> io) = 0;
            virtual int initialization_complete(int success_code) = 0;
            virtual int wait_for_exit() = 0;
            virtual int shutdown_start() = 0;
            virtual int shutdown_complete(int success_code) = 0;
        };
    }
}

#endif
