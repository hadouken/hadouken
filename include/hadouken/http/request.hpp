#ifndef HDKN_HTTP_REQUEST_HPP
#define HDKN_HTTP_REQUEST_HPP

#include <string>
#include <vector>

#include <hadouken/http/header.hpp>

namespace hadouken
{
    namespace http
    {
        struct request
        {
            std::string method;
            std::string uri;
            int http_version_major;
            int http_version_minor;
            std::vector<header> headers;
        };
    }
}

#endif