#include <hadouken/http/reply.hpp>

#include <string>

namespace hadouken
{
    namespace http
    {
        namespace status_strings
        {
            boost::asio::const_buffer to_buffer(reply::status_type status)
            {
                switch(status)
                {
                case reply::ok:
                    return boost::asio::buffer("HTTP/1.0 200 OK\r\n");
                case reply::bad_request:
                    return boost::asio::buffer("HTTP/1.0 400 Bad Request\r\n");
                default:
                    return boost::asio::buffer("HTTP/1.0 500 Internal Server Error\r\n");
                }
            }
        }

        std::vector<boost::asio::const_buffer> reply::to_buffers()
        {
            std::vector<boost::asio::const_buffer> buffers;
            buffers.push_back(status_strings::to_buffer(status));

            for(std::size_t i = 0; i < headers.size(); ++i)
            {
                header& h = headers[i];
                buffers.push_back(boost::asio::buffer(h.name));
                buffers.push_back(boost::asio::buffer(": "));
                buffers.push_back(boost::asio::buffer(h.value));
                buffers.push_back(boost::asio::buffer("\r\n"));
            }

            buffers.push_back(boost::asio::buffer("\r\n"));
            buffers.push_back(boost::asio::buffer(content));

            return buffers;
        }

        reply reply::stock_reply(reply::status_type status)
        {
            reply rep;
            rep.status = status;
            rep.content = "";

            rep.headers.resize(2);
            rep.headers[0].name = "Content-Length";
            rep.headers[0].value = std::to_string(rep.content.size());
            rep.headers[1].name = "Content-Type";
            rep.headers[1].value = "text/html";

            return rep;
        }
    }
}

