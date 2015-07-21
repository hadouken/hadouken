#include <hadouken/http/api_request_handler.hpp>

#include <hadouken/http/connection_handler.hpp>

using namespace hadouken::http;

api_request_handler::api_request_handler(boost::function<std::string(std::string)> const& rpc_callback)
    : rpc_callback_(rpc_callback)
{
}

void api_request_handler::execute(std::string virtual_path,
                                  http_server_t::request const &request,
                                  http_server_t::connection_ptr connection)
{
    boost::shared_ptr<connection_handler> handler(new connection_handler(request));
    handler->set_data_callback(boost::bind(&api_request_handler::handle_incoming_data, this, _1, _2));
    (*handler)(connection);
}

void api_request_handler::handle_incoming_data(http_server_t::connection_ptr connection, std::string data)
{
    std::string response = rpc_callback_(data);

    http_server_t::response_header headers[] =
    {
        { "Content-Length", std::to_string(response.size()) },
        { "Content-Type", "application/json" }
    };

    connection->set_status(http_server_t::connection::status_t::ok);
    connection->set_headers(boost::make_iterator_range(headers, headers + 2));
    connection->write(response);
}
