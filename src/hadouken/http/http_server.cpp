#include "http_server.hpp"

#include <boost/beast.hpp>

#include "../jsonrpc/jsonrpc_server.hpp"
#include "http_session.hpp"
#include "../session_manager.hpp"

using hadouken::http::http_server;
using hadouken::http::http_session;

http_server::http_server(boost::asio::io_service& io, int port)
    : m_io(io),
    m_acceptor(boost::asio::make_strand(io)),
    m_jsonrpc_server(std::make_shared<hadouken::jsonrpc::jsonrpc_server>())
{
    boost::asio::ip::tcp::endpoint endp
    {
        boost::asio::ip::make_address("127.0.0.1"),
        static_cast<unsigned short>(port)
    };

    m_acceptor.open(endp.protocol());
    m_acceptor.set_option(boost::asio::socket_base::reuse_address(true));
    m_acceptor.bind(endp);
    m_acceptor.listen(boost::asio::socket_base::max_listen_connections);
}

hadouken::jsonrpc::jsonrpc_server& http_server::jsonrpc()
{
    return *m_jsonrpc_server.get();
}

void http_server::run()
{
    begin_accept();
}

void http_server::begin_accept()
{
    m_acceptor.async_accept(
        boost::asio::make_strand(m_io),
        boost::beast::bind_front_handler(
            &http_server::on_accept,
            shared_from_this()));
}

void http_server::on_accept(boost::beast::error_code ec, boost::asio::ip::tcp::socket sock)
{
    if (ec)
    {
        printf("error when accepting connection: %s", ec.message().c_str());
    }
    else
    {
        std::make_shared<http_session>(
            std::move(sock),
            m_jsonrpc_server)->run();
    }

    begin_accept();
}
