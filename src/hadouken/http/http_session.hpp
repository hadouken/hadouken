#pragma once

#include <memory>

#include <boost/asio.hpp>
#include <boost/beast.hpp>

namespace hadouken
{
namespace jsonrpc
{
    class jsonrpc_server;
}
namespace http
{
    class http_session : public std::enable_shared_from_this<http_session>
    {
    public:
        http_session(boost::asio::ip::tcp::socket&& sock, std::shared_ptr<jsonrpc::jsonrpc_server> jsonrpc);
        void run();

    private:
        struct send_lambda
        {
            http_session& m_self;

            explicit send_lambda(http_session& self)
                : m_self(self)
            {
            }

            template<bool isRequest, class Body, class Fields>
            void operator()(boost::beast::http::message<isRequest, Body, Fields>&& msg) const
            {
                // The lifetime of the message has to extend
                // for the duration of the async operation so
                // we use a shared_ptr to manage it.
                auto sp = std::make_shared<boost::beast::http::message<isRequest, Body, Fields>>(
                    std::move(msg));

                // Store a type-erased version of the shared
                // pointer in the class to keep it alive.
                m_self.m_res = sp;

                // Write the response
                boost::beast::http::async_write(
                    m_self.m_stream,
                    *sp,
                    boost::beast::bind_front_handler(
                        &http_session::on_write,
                        m_self.shared_from_this(),
                        sp->need_eof()));
            }
        };

        void begin_read();
        void close();
        void on_read(boost::beast::error_code ec, std::size_t bytes_transferred);
        void on_write(bool close, boost::beast::error_code ec, std::size_t bytes_transferred);

        boost::beast::tcp_stream m_stream;
        boost::beast::flat_buffer m_buffer;
        boost::beast::http::request<boost::beast::http::string_body> m_req;
        std::shared_ptr<void> m_res;
        send_lambda m_lambda;
        std::shared_ptr<jsonrpc::jsonrpc_server> m_jsonrpc;
    };
}
}
