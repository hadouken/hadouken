#ifndef HADOUKEN_HOSTING_CONSOLEHOST_HPP
#define HADOUKEN_HOSTING_CONSOLEHOST_HPP

#include <hadouken/hosting/host.hpp>
#include <boost/asio.hpp>

namespace hadouken
{
    namespace hosting
    {
        class console_host : public host
        {
        public:
            int wait_for_exit(boost::shared_ptr<boost::asio::io_service> io);
        };
    }
}

#endif
