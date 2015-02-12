#ifndef HDKN_HTTP_REQUEST_HANDLER_HPP
#define HDKN_HTTP_REQUEST_HANDLER_HPP

#include <hadouken/config.hpp>
#include <string>

namespace hadouken
{
    namespace http
    {
        struct reply;
        struct request;

        class request_handler
        {
        public:
            request_handler(const request_handler&) = delete;
            request_handler& operator=(const request_handler&) = delete;

            explicit request_handler();

            void handle_request(const request& request, reply& reply);
        };
    }
}

#endif