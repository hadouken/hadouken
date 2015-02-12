#include <hadouken/http/http_server.hpp>

#include <boost/bind.hpp>
#include <hadouken/http/connection.hpp>
#include <hadouken/logger.hpp>

using boost::asio::ip::tcp;
using namespace hadouken::http;

http_server::http_server(boost::asio::io_service& io_service, int port)
    : acceptor_(io_service),
    io_service_(io_service),
    socket_(io_service),
    request_handler_()
{
    acceptor_.open(tcp::v4());
    acceptor_.bind(tcp::endpoint(tcp::v4(), port));
    acceptor_.set_option(boost::asio::ip::tcp::acceptor::reuse_address(true));
    acceptor_.listen();
}

void http_server::start()
{
    do_accept();
}

void http_server::stop()
{
    acceptor_.close();
}

void http_server::do_accept()
{
    acceptor_.async_accept(socket_, [this](const boost::system::error_code& error)
    {
        if (!acceptor_.is_open())
        {
            return;
        }

        if (!error)
        {
            manager_.start(std::make_shared<connection>(boost::move(socket_),
                manager_,
                request_handler_));
        }

        do_accept();
    });
}