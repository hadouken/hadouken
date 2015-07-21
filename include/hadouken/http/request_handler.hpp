#ifndef HADOUKEN_HTTP_REQUESTHANDLER_HPP
#define HADOUKEN_HTTP_REQUESTHANDLER_HPP

#include <boost/network/protocol/http/server.hpp>

namespace hadouken
{
    namespace http
    {
        class http_server;
        typedef boost::network::http::async_server<http_server> http_server_t;

        class request_handler
        {
        public:
            virtual void execute(std::string virtual_path,
                                 http_server_t::request const &request,
                                 http_server_t::connection_ptr connection) = 0;
        };
    }
}

#endif
