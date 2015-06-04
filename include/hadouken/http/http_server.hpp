#ifndef HADOUKEN_HTTP_HTTPSERVER_HPP
#define HADOUKEN_HTTP_HTTPSERVER_HPP

#include <boost/asio.hpp>
#include <boost/network/protocol/http/server.hpp>
#include <boost/property_tree/ptree.hpp>
#include <memory>

namespace hadouken
{
    namespace http
    {
        class http_server;
        typedef boost::network::http::async_server<http_server> http_server_t;

        class http_server
        {
        public:
            http_server(boost::shared_ptr<boost::asio::io_service> io, const boost::property_tree::ptree& config);
            ~http_server();

            void start();

            void stop();

            void operator() (http_server_t::request const &request, http_server_t::connection_ptr connection);

            void log(http_server_t::string_type const &info);

            void set_rpc_callback(boost::function<std::string(std::string)> const& fun);

            void set_auth_callback(boost::function<bool(std::string)> const& fun);

        protected:
            bool is_authenticated(http_server_t::request const &request);

            void handle_incoming_data(http_server_t::connection_ptr connection, std::string data);

            std::string ssl_password_callback(std::size_t max_length, boost::asio::ssl::context_base::password_purpose purpose);

        private:
            boost::property_tree::ptree config_;
            std::unique_ptr<http_server_t> instance_;

            boost::function<std::string(std::string)> rpc_callback_;
            boost::function<bool(std::string)> auth_callback_;
        };
    }
}

#endif
