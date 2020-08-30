#pragma once

#include <map>
#include <memory>

#include <boost/asio.hpp>
#include <boost/beast.hpp>

namespace hadouken
{
    class session_manager;

namespace jsonrpc
{
    class jsonrpc_server;
}
namespace http
{
    class http_server : public std::enable_shared_from_this<http_server>
    {
    public:
        http_server(boost::asio::io_service& io, int port);

        hadouken::jsonrpc::jsonrpc_server& jsonrpc();
        void run();

    private:
        void begin_accept();
        void on_accept(boost::beast::error_code ec, boost::asio::ip::tcp::socket socket);

        boost::asio::io_service& m_io;
        boost::asio::ip::tcp::acceptor m_acceptor;

        std::shared_ptr<hadouken::jsonrpc::jsonrpc_server> m_jsonrpc_server;
    };
}
}
