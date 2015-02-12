#ifndef HDKN_HTTP_HEADER_HPP
#define HDKN_HTTP_HEADER_HPP

#include <string>

namespace hadouken
{
    namespace http
    {
        struct header
        {
            std::string name;
            std::string value;
        };
    }
}

#endif