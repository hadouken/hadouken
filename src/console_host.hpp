#ifndef HDKN_CONSOLE_HOST_HPP
#define HDKN_CONSOLE_HOST_HPP

#include "host.hpp"

#include <hadouken/service_locator.hpp>

namespace hadouken
{
    class console_host : public host
    {
    public:
        console_host(service_locator& locator)
            : loc_(locator)
        {
        }

        int run();

    private:
        service_locator& loc_;
    };
}

#endif