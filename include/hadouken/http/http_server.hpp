#ifndef HADOUKEN_HTTP_HTTPSERVER_HPP
#define HADOUKEN_HTTP_HTTPSERVER_HPP

#include <boost/asio.hpp>
#include <boost/network/protocol/http/server.hpp>
#include <boost/property_tree/ptree.hpp>
#include <hadouken/http/request_handler.hpp>
#include <map>
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
            void process_request(http_server_t::request const &request, http_server_t::connection_ptr connection);

            std::shared_ptr<request_handler> find_request_handler(std::string virtual_path);

            bool is_authenticated(std::string credentials);

            std::string ssl_password_callback(std::size_t max_length, boost::asio::ssl::context_base::password_purpose purpose);

            boost::shared_ptr<boost::asio::ssl::context> get_ssl_context();

        private:
            boost::property_tree::ptree config_;
            std::unique_ptr<http_server_t> instance_;
            std::map<std::string, std::shared_ptr<request_handler>> request_handlers_;

            boost::function<std::string(std::string)> rpc_callback_;
            boost::function<bool(std::string)> auth_callback_;
        };
    }
}

#endif
