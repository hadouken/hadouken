#ifndef HDKN_HTTP_SERVER_HPP
#define HDKN_HTTP_SERVER_HPP

#include <hadouken/config.hpp>

#include <boost/asio.hpp>
#include <hadouken/http/connection.hpp>
#include <hadouken/http/connection_manager.hpp>
#include <hadouken/http/request_handler.hpp>

namespace hadouken
{
    namespace http
    {
        class http_server
        {
        public:
            HDKN_API http_server(boost::asio::io_service& io_service, int port);

            HDKN_API ~http_server();

            void HDKN_API start();

            void HDKN_API stop();

        private:
            void do_accept();

            void do_await_stop();

            boost::asio::io_service& io_service_;
            boost::asio::ip::tcp::acceptor* acceptor_;
            boost::asio::ip::tcp::socket* socket_;

            request_handler request_handler_;
            connection_manager manager_;
        };
    }
}

#endif