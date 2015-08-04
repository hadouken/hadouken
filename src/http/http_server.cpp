#include <hadouken/http/http_server.hpp>

#include <boost/filesystem.hpp>
#include <boost/filesystem/fstream.hpp>
#include <boost/log/trivial.hpp>
#include <hadouken/http/connection_handler.hpp>
#include <hadouken/http/api_request_handler.hpp>
#include <hadouken/http/gui_request_handler.hpp>
#include <hadouken/platform.hpp>
#include <hadouken/version.hpp>
#include <regex>

using namespace hadouken::http;
namespace fs = boost::filesystem;

http_server::http_server(boost::shared_ptr<boost::asio::io_service> io, const boost::property_tree::ptree& config)
    : config_(config)
{
    std::string address = config.get("http.address", "0.0.0.0");
    int port = config.get("http.port", 7070);

    http_server_t::options opts(*this);

    /*
    If HTTPS enabled
    */
    bool https_enabled = config.get("http.ssl.enabled", false);

    if (https_enabled)
    {
        boost::shared_ptr<boost::asio::ssl::context> ctx = get_ssl_context();

        if (ctx)
        {
            opts.context(ctx);
            BOOST_LOG_TRIVIAL(info) << "HTTPS enabled.";
        }
    }

    instance_ = std::unique_ptr<http_server_t>(
        new http_server_t(opts.address(address).port(std::to_string(port)).io_service(io)));

    // register request handlers
    request_handlers_["^\/api[\/]?$"] = std::make_shared<api_request_handler>([this](std::string f) {
        return rpc_callback_(f);
    });

    request_handlers_["^/gui.*$"] = std::make_shared<gui_request_handler>(config);
}

http_server::~http_server()
{
}

void http_server::start()
{
    instance_->listen();
}

void http_server::stop()
{
    instance_->stop();
}

void http_server::operator() (http_server_t::request const &request, http_server_t::connection_ptr connection)
{
    // Authenticate request. Every request should
    // be authenticated here before heading down to JS world.

    // 1. Check if we have the "Authorization" header.
    //    If we do, validate it. If not, send the WWW-Authenticate header.

    auto header = std::find_if(request.headers.begin(), request.headers.end(), [](const boost::network::http::request_header_narrow& h)
    {
        return h.name == "Authorization";
    });

    // We have the header and it is valid. Allow request.
    if (header != request.headers.end() && is_authenticated(header->value))
    {
        return process_request(request, connection);
    }

    // Send the WWW-Authenticate header back.
    std::string realm = "\"Hadouken v" + hadouken::version::VERSION() + "\"";

    http_server_t::response_header headers[] =
    {
        { "WWW-Authenticate", "Basic realm=" + realm }
    };

    connection->set_status(http_server_t::connection::unauthorized);
    connection->set_headers(boost::make_iterator_range(headers, headers + 1));
}

void http_server::log(http_server_t::string_type const &info)
{
    BOOST_LOG_TRIVIAL(info) << info;
}

void http_server::set_rpc_callback(boost::function<std::string(std::string)> const& fun)
{
    rpc_callback_ = fun;
}

void http_server::set_auth_callback(boost::function<bool(std::string)> const& fun)
{
    auth_callback_ = fun;
}

void http_server::process_request(http_server_t::request const &request, http_server_t::connection_ptr connection)
{
    // get path without the configured root.
    std::string configured_root = config_.get("http.root", "/");
    if (configured_root.empty()) { configured_root = "/"; }

    // the root path should start with "/" and end with "/".
    if (configured_root[0] != '/') { configured_root = "/" + configured_root; }
    if (configured_root[configured_root.size() - 1] != '/') { configured_root = configured_root + "/"; }

    std::string path = request.destination;

    // If we request the root path ('/'), 301 redirect to the gui path.
    if (path == "/")
    {
        http_server_t::response_header headers[] =
        {
            { "Location", configured_root + "gui/index.html" }
        };

        connection->set_status(http_server_t::connection::moved_permanently);
        connection->set_headers(boost::make_iterator_range(headers, headers + 1));
        return;
    }

    // make sure the requested path is for the configured root path
    if (path.size() < configured_root.size()
        || path.substr(0, configured_root.size()) != configured_root)
    {
        connection->set_status(http_server_t::connection::not_found);
        connection->write("404 - Could not find '" + path + "'");
        return;
    }

    std::string virtual_path = path.substr(configured_root.size());
    if (virtual_path.empty()) { virtual_path = "/"; }
    if (virtual_path[0] != '/') { virtual_path = "/" + virtual_path; }

    std::shared_ptr<request_handler> handler = find_request_handler(virtual_path);

    if (!handler)
    {
        connection->set_status(http_server_t::connection::not_found);
        connection->write("404 - Could not find '" + path + "'");
        return;
    }

    // Execute handler
    handler->execute(virtual_path, request, connection);
}

std::shared_ptr<request_handler> http_server::find_request_handler(std::string virtual_path)
{
    for (auto pair : request_handlers_)
    {
        std::regex rx(pair.first);

        if (std::regex_match(virtual_path, rx))
        {
            return pair.second;
        }
    }

    return nullptr;
}

bool http_server::is_authenticated(std::string credentials)
{
    return auth_callback_(credentials);
}

std::string http_server::ssl_password_callback(std::size_t max_length, boost::asio::ssl::context_base::password_purpose purpose)
{
    return config_.get("http.ssl.privateKeyPassword", "");
}

boost::shared_ptr<boost::asio::ssl::context> http_server::get_ssl_context()
{
    std::string privateKeyFile = config_.get<std::string>("http.ssl.privateKeyFile");

    if (!fs::exists(privateKeyFile))
    {
        BOOST_LOG_TRIVIAL(error) << "Private key file does not exist.";
        return nullptr;
    }

    boost::shared_ptr<boost::asio::ssl::context> ctx = boost::make_shared<boost::asio::ssl::context>(boost::asio::ssl::context::sslv23);
    ctx->set_options(
        boost::asio::ssl::context::default_workarounds
        | boost::asio::ssl::context::no_sslv2
        | boost::asio::ssl::context::single_dh_use);

    ctx->set_password_callback(boost::bind(&http_server::ssl_password_callback, this, _1, _2));

    boost::system::error_code ec;
    ctx->use_certificate_chain_file(privateKeyFile, ec);

    if (ec)
    {
        BOOST_LOG_TRIVIAL(error) << "Error when setting certificate chain file: " << ec.message();
        return nullptr;
    }

    ctx->use_private_key_file(privateKeyFile, boost::asio::ssl::context::pem, ec);

    if (ec)
    {
        BOOST_LOG_TRIVIAL(error) << "Error when setting private key file: " << ec.message();
        return nullptr;
    }

    return ctx;
}
