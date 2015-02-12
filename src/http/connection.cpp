#include <hadouken/http/connection.hpp>

#include <hadouken/logger.hpp>
#include <hadouken/http/connection_manager.hpp>
#include <hadouken/http/request_handler.hpp>
#include <utility>
#include <boost/move/move.hpp>

using namespace hadouken::http;

struct request_t
{
    request req;
    std::string current_header_field;
};

int parser_on_body(http_parser* parser, const char* at, size_t length)
{
    request_t* r = (request_t*)parser->data;
    r->req.body.append(std::string(at, length));

    return 0;
}

int parser_on_headers_complete(http_parser* parser)
{
    return 0;
}

int parser_on_header_field(http_parser* parser, const char* at, size_t length)
{
    request_t* r = (request_t*)parser->data;
    r->current_header_field = std::string(at, length);

    return 0;
}

int parser_on_header_value(http_parser* parser, const char* at, size_t length)
{
    request_t* r = (request_t*)parser->data;
    std::string value = std::string(at, length);

    header h;
    h.name = r->current_header_field;
    h.value = value;

    r->req.headers.push_back(h);

    return 0;
}

int parser_on_message_begin(http_parser* parser)
{
    return 0;
}

int parser_on_message_complete(http_parser* parser)
{
    request_t* r = (request_t*)parser->data;
    r->req.http_version_major = parser->http_major;
    r->req.http_version_minor = parser->http_minor;
    r->req.method = std::string(http_method_str((http_method)parser->method));

    return 0;
}

int parser_on_url(http_parser* parser, const char* at, size_t length)
{
    request_t* r = (request_t*)parser->data;
    r->req.uri = std::string(at, length);

    return 0;
}

connection::connection(boost::asio::ip::tcp::socket socket,
    connection_manager& manager,
    request_handler& handler)
    : socket_(boost::move(socket)),
      manager_(manager),
      request_handler_(handler)
{
    parser_settings_.on_body = parser_on_body;
    parser_settings_.on_headers_complete = parser_on_headers_complete;
    parser_settings_.on_header_field = parser_on_header_field;
    parser_settings_.on_header_value = parser_on_header_value;
    parser_settings_.on_message_begin = parser_on_message_begin;
    parser_settings_.on_message_complete = parser_on_message_complete;
    parser_settings_.on_url = parser_on_url;

    http_parser_init(&parser_, HTTP_REQUEST);
}

void connection::start()
{
    do_read();
}

void connection::stop()
{
    socket_.close();
}

void connection::do_read()
{
    auto self(shared_from_this());

    socket_.async_read_some(boost::asio::buffer(buffer_),
        [this, self](boost::system::error_code error, std::size_t bytes_transferred)
        {
            if(!error)
            {
                if (bytes_transferred > 0)
                {
                    request_t r;
                    r.req = request();

                    parser_.data = &r;
                    size_t len = http_parser_execute(&parser_, &parser_settings_, buffer_.data(), bytes_transferred);

                    request_handler_.handle_request(r.req, reply_);
                    do_write();
                }
            }
            else if(error != boost::asio::error::operation_aborted)
            {
                manager_.stop(shared_from_this());
            }
        });
}

void connection::do_write()
{
    auto self(shared_from_this());

    boost::asio::async_write(socket_, reply_.to_buffers(),
        [this, self](boost::system::error_code error, std::size_t)
        {
            if(!error)
            {
                boost::system::error_code ignored_ec;
                socket_.shutdown(boost::asio::ip::tcp::socket::shutdown_both, ignored_ec);
            }

            if(error != boost::asio::error::operation_aborted)
            {
                manager_.stop(shared_from_this());
            }
    });
}
