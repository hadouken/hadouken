#ifndef HDKN_HTTP_SERVER_HPP
#define HDKN_HTTP_SERVER_HPP

#include <hadouken/config.hpp>

#include <boost/asio.hpp>
#include <boost/property_tree/ptree.hpp>
#include <boost/thread/mutex.hpp>
#include <hadouken/http/connection.hpp>
#include <hadouken/http/connection_manager.hpp>
#include <hadouken/http/reply.hpp>
#include <hadouken/http/request.hpp>

namespace pt = boost::property_tree;

namespace hadouken
{
    namespace http
    {
        typedef boost::function2<void, pt::ptree&, pt::ptree&> rpc_handler_t;
        
        class http_server
        {
        public:
            HDKN_API http_server(boost::asio::io_service& io_service, int port);

            HDKN_API ~http_server();

            void HDKN_API start();

            void HDKN_API stop();

            void HDKN_API add_rpc_handler(const std::string& method_name, rpc_handler_t handler);

        private:
            typedef std::map<std::string, rpc_handler_t> handler_map_t;

            void do_accept();

            void do_await_stop();

            bool find_rpc_handler(const std::string& method_name, rpc_handler_t& handler);

            void handle_incoming_request(const request& req, reply& reply);

            boost::asio::io_service& io_service_;
            boost::asio::ip::tcp::acceptor* acceptor_;
            boost::asio::ip::tcp::socket* socket_;

            handler_map_t handlers_;
            boost::mutex handlers_mutex_;

            connection_manager manager_;
        };
    }
}

#endif