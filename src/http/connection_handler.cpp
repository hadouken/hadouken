#include <hadouken/http/connection_handler.hpp>

#include <boost/log/trivial.hpp>

using namespace hadouken::http;

connection_handler::connection_handler(const http_server_t::request& request)
    : req_(request)
{
}

void connection_handler::operator()(http_server_t::connection_ptr connection)
{
    auto header = std::find_if(req_.headers.begin(), req_.headers.end(), [](const boost::network::http::request_header_narrow& h)
    {
        return h.name == "Content-Length";
    });

    if (header == req_.headers.end())
    {
        BOOST_LOG_TRIVIAL(debug) << "Could not find 'Content-Length' header.";
        return;
    }

    int contentLength = boost::lexical_cast<int>(header->value);
    read(connection, contentLength);
}

void connection_handler::set_data_callback(boost::function<void(http_server_t::connection_ptr, std::string)> const& fun)
{
    data_callback_ = fun;
}

void connection_handler::read(http_server_t::connection_ptr connection, std::size_t length)
{
    connection->read(boost::bind(&connection_handler::read_complete, connection_handler::shared_from_this(), _1, _2, _3, connection, length));
}

void connection_handler::read_complete(http_server_t::connection::input_range range, const boost::system::error_code& error, std::size_t length, http_server_t::connection_ptr connection, std::size_t remaining)
{
    if (error)
    {
        BOOST_LOG_TRIVIAL(error) << "Error when reading from HTTP connection: " << error.message();
        return;
    }

    body_.append(boost::begin(range), length);
    std::size_t left = remaining - length;

    if (left > 0)
    {
        read(connection, left);
    }
    else
    {
        data_callback_(connection, body_);
    }
}
