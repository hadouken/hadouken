#include "http_session.hpp"

#include <boost/log/trivial.hpp>

#include "../jsonrpc/jsonrpc_server.hpp"
#include "mime_type.hpp"

using hadouken::http::http_session;
using hadouken::http::mime_type;

// Append an HTTP rel-path to a local filesystem path.
// The returned path is normalized for the platform.
std::string path_cat(
    boost::beast::string_view base,
    boost::beast::string_view path)
{
    if(base.empty())
        return std::string(path);
    std::string result(base);
#ifdef BOOST_MSVC
    char constexpr path_separator = '\\';
    if(result.back() == path_separator)
        result.resize(result.size() - 1);
    result.append(path.data(), path.size());
    for(auto& c : result)
        if(c == '/')
            c = path_separator;
#else
    char constexpr path_separator = '/';
    if(result.back() == path_separator)
        result.resize(result.size() - 1);
    result.append(path.data(), path.size());
#endif
    return result;
}

// This function produces an HTTP response for the given
// request. The type of the response object depends on the
// contents of the request, so the interface requires the
// caller to pass a generic lambda for receiving the response.
template<class Body, class Allocator, class Send>
void handle_request(
    boost::beast::string_view doc_root,
    boost::beast::http::request<Body, boost::beast::http::basic_fields<Allocator>>&& req,
    Send&& send)
{
    // Returns a bad request response
    auto const bad_request =
    [&req](boost::beast::string_view why)
    {
        boost::beast::http::response<boost::beast::http::string_body> res{boost::beast::http::status::bad_request, req.version()};
        res.set(boost::beast::http::field::server, BOOST_BEAST_VERSION_STRING);
        res.set(boost::beast::http::field::content_type, "text/html");
        res.keep_alive(req.keep_alive());
        res.body() = std::string(why);
        res.prepare_payload();
        return res;
    };

    // Returns a not found response
    auto const not_found =
    [&req](boost::beast::string_view target)
    {
        boost::beast::http::response<boost::beast::http::string_body> res{boost::beast::http::status::not_found, req.version()};
        res.set(boost::beast::http::field::server, BOOST_BEAST_VERSION_STRING);
        res.set(boost::beast::http::field::content_type, "text/html");
        res.keep_alive(req.keep_alive());
        res.body() = "The resource '" + std::string(target) + "' was not found.";
        res.prepare_payload();
        return res;
    };

    // Returns a server error response
    auto const server_error =
    [&req](boost::beast::string_view what)
    {
        boost::beast::http::response<boost::beast::http::string_body> res{boost::beast::http::status::internal_server_error, req.version()};
        res.set(boost::beast::http::field::server, BOOST_BEAST_VERSION_STRING);
        res.set(boost::beast::http::field::content_type, "text/html");
        res.keep_alive(req.keep_alive());
        res.body() = "An error occurred: '" + std::string(what) + "'";
        res.prepare_payload();
        return res;
    };

    // Check if JSONRPC
    if (req.method() == boost::beast::http::verb::post
        && (req.target() == "/api/jsonrpc" || req.target() == "/api/jsonrpc/"))
    {
        std::string body = req.body();
        printf("%s\n", body.c_str());
    }

    // Make sure we can handle the method
    if( req.method() != boost::beast::http::verb::get &&
        req.method() != boost::beast::http::verb::head)
        return send(bad_request("Unknown HTTP-method"));

    // Request path must be absolute and not contain "..".
    if( req.target().empty() ||
        req.target()[0] != '/' ||
        req.target().find("..") != boost::beast::string_view::npos)
        return send(bad_request("Illegal request-target"));

    // Build the path to the requested file
    std::string path = path_cat(doc_root, req.target());
    if(req.target().back() == '/')
        path.append("index.html");

    printf("http req: %s\n", path.c_str());

    // Attempt to open the file
    boost::beast::error_code ec;
    boost::beast::http::file_body::value_type body;
    body.open(path.c_str(), boost::beast::file_mode::scan, ec);

    // Handle the case where the file doesn't exist
    if(ec == boost::beast::errc::no_such_file_or_directory)
        return send(not_found(req.target()));

    // Handle an unknown error
    if(ec)
        return send(server_error(ec.message()));

    // Cache the size since we need it after the move
    auto const size = body.size();

    // Respond to HEAD request
    if(req.method() == boost::beast::http::verb::head)
    {
        boost::beast::http::response<boost::beast::http::empty_body> res{boost::beast::http::status::ok, req.version()};
        res.set(boost::beast::http::field::server, BOOST_BEAST_VERSION_STRING);
        res.set(boost::beast::http::field::content_type, mime_type(path));
        res.content_length(size);
        res.keep_alive(req.keep_alive());
        return send(std::move(res));
    }

    // Respond to GET request
    boost::beast::http::response<boost::beast::http::file_body> res{
        std::piecewise_construct,
        std::make_tuple(std::move(body)),
        std::make_tuple(boost::beast::http::status::ok, req.version())};
    res.set(boost::beast::http::field::server, BOOST_BEAST_VERSION_STRING);
    res.set(boost::beast::http::field::content_type, mime_type(path));
    res.content_length(size);
    res.keep_alive(req.keep_alive());
    return send(std::move(res));
}

http_session::http_session(boost::asio::ip::tcp::socket&& sock, std::shared_ptr<jsonrpc::jsonrpc_server> jsonrpc)
    : m_stream(std::move(sock)),
    m_lambda(*this),
    m_jsonrpc(jsonrpc)
{
}

void http_session::run()
{
    boost::asio::dispatch(
        m_stream.get_executor(),
        boost::beast::bind_front_handler(
            &http_session::begin_read,
            shared_from_this()));
}

void http_session::begin_read()
{
    m_req = {};
    m_stream.expires_after(std::chrono::seconds(30));
    boost::beast::http::async_read(
        m_stream,
        m_buffer,
        m_req,
        boost::beast::bind_front_handler(
            &http_session::on_read,
            shared_from_this()));
}

void http_session::close()
{
    m_stream.socket().shutdown(boost::asio::ip::tcp::socket::shutdown_send);
}

void http_session::on_read(boost::beast::error_code ec, std::size_t bytes_transferred)
{
    if (ec == boost::beast::http::error::end_of_stream)
    {
        return close();
    }

    if (ec)
    {
        printf("fail on read: %s\n", ec.message().c_str());
        return;
    }

    if (m_req.method() == boost::beast::http::verb::post
        && (m_req.target() == "/api/jsonrpc" || m_req.target() == "/api/jsonrpc/"))
    {
        auto const body = m_jsonrpc->execute(m_req.body());
        auto const size = body.size();

        boost::beast::http::response<boost::beast::http::string_body> res
        {
            boost::beast::http::status::ok,
            m_req.version()
        };
        res.set(boost::beast::http::field::server, BOOST_BEAST_VERSION_STRING);
        res.set(boost::beast::http::field::content_type, "application/json");
        res.keep_alive(m_req.keep_alive());
        res.body() = std::string(body);
        res.prepare_payload();
        return m_lambda(std::move(res));
    }

    handle_request(
        std::string(), // TODO: web ui path
        std::move(m_req),
        m_lambda);
}

void http_session::on_write(bool close, boost::beast::error_code ec, std::size_t bytes_transferred)
{
    if (ec)
    {
        BOOST_LOG_TRIVIAL(error) << "Error when writing data: " << ec.message();
        return;
    }

    if (close)
    {
        return this->close();
    }

    m_res = nullptr;

    begin_read();
}
