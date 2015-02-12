#include <hadouken/http/request_handler.hpp>

#include <hadouken/http/reply.hpp>
#include <hadouken/http/request.hpp>
#include <string>

using namespace hadouken::http;

request_handler::request_handler()
{
}

void request_handler::handle_request(const request& request, reply& reply)
{
    // Decode url to path
    std::string request_path;

    if (request.method == "POST")
    {
        reply.content = request.body;
    }
    else
    {
        reply.content = "{  }";
    }

    reply.status = reply::ok;
    reply.headers.resize(3);
    reply.headers[0].name = "Content-Length";
    reply.headers[0].value = std::to_string(reply.content.size());
    reply.headers[1].name = "Content-Type";
    reply.headers[1].value = "application/json";
    reply.headers[2].name = "Connection";
    reply.headers[2].value = "close";
}