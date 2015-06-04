#ifndef HADOUKEN_HTTP_CONNECTIONHANDLER_HPP
#define HADOUKEN_HTTP_CONNECTIONHANDLER_HPP

#include <hadouken/http/http_server.hpp>

namespace hadouken
{
    namespace http
    {
        class connection_handler : public boost::enable_shared_from_this<connection_handler>
        {
        public:
            connection_handler(const http_server_t::request& request);

            void operator()(http_server_t::connection_ptr connection);

            void set_data_callback(boost::function<void(http_server_t::connection_ptr, std::string)> const& fun);

        protected:
            void read(http_server_t::connection_ptr connection, std::size_t length);

            void read_complete(http_server_t::connection::input_range, const boost::system::error_code& error, std::size_t length, http_server_t::connection_ptr connection, std::size_t remaining);

        private:
            const http_server_t::request& req_;
            std::string body_;
            boost::function<void(http_server_t::connection_ptr, std::string)> data_callback_;
        };
    }
}

#endif
