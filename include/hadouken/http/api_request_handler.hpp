#ifndef HADOUKEN_HTTP_APIREQUESTHANDLER_HPP
#define HADOUKEN_HTTP_APIREQUESTHANDLER_HPP

#include <hadouken/http/request_handler.hpp>

namespace hadouken
{
    namespace http
    {
        class api_request_handler : public request_handler
        {
        public:
            api_request_handler(boost::function<std::string(std::string)> const& rpc_callback);

            void execute(std::string virtual_path,
                         http_server_t::request const &request,
                         http_server_t::connection_ptr connection);

        protected:
            void handle_incoming_data(http_server_t::connection_ptr connection, std::string data);

        private:
            boost::function<std::string(std::string)> rpc_callback_;
        };
    }
}

#endif
