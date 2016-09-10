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
            int initialization_start(boost::shared_ptr<boost::asio::io_service> io);
            int initialization_complete(int success_code);
            int wait_for_exit();
            int shutdown_start();
            int shutdown_complete(int success_code);

        private:
            boost::shared_ptr<boost::asio::io_service> io_;
        };
    }
}

#endif
