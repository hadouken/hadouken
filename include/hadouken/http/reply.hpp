#ifndef HDKN_HTTP_REPLY_HPP
#define HDKN_HTTP_REPLY_HPP

#include <boost/asio.hpp>
#include <hadouken/http/header.hpp>
#include <string>
#include <vector>

namespace hadouken
{
    namespace http
    {
        struct reply
        {
            enum status_type
            {
                ok = 200,
                bad_request = 400
            } status;

            std::vector<header> headers;

            std::string content;

            std::vector<boost::asio::const_buffer> to_buffers();

            static reply stock_reply(status_type status);
        };
    }
}

#endif