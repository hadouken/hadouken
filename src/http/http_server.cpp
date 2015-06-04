#include <hadouken/http/http_server.hpp>

#include <boost/log/trivial.hpp>
#include <hadouken/http/connection_handler.hpp>

using namespace hadouken::http;

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
        std::string privateKeyFile = config.get<std::string>("http.ssl.privateKeyFile");
        // TODO: Validate the existence of the private key file

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
        }

        ctx->use_private_key_file(privateKeyFile, boost::asio::ssl::context::pem, ec);

        if (ec)
        {
            BOOST_LOG_TRIVIAL(error) << "Error when setting private key file: " << ec.message();
        }

        if (!ec)
        {
            // Set SSL context
            opts.context(ctx);
            BOOST_LOG_TRIVIAL(info) << "HTTPS enabled";
        }
    }

    instance_ = std::make_unique<http_server_t>(opts.address(address).port(std::to_string(port)).io_service(io));
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
    // CORS enabling
    bool enable_cors = config_.get("http.cors.enabled", true);
    std::string cors_origin = config_.get("http.cors.origin", "*");
    std::string cors_headers = config_.get("http.cors.headers", "accept, authorization, content-type, x-requested-with");
    std::string cors_methods = config_.get("http.cors.methods", "POST, GET, OPTIONS");
    int cors_max_age = config_.get("http.cors.maxAge", 1728000);

    if (request.method == "OPTIONS" && enable_cors)
    {
        http_server_t::response_header headers[] =
        {
            { "Access-Control-Allow-Origin", cors_origin },
            { "Access-Control-Allow-Methods", cors_methods },
            { "Access-Control-Allow-Headers", cors_headers },
            { "Access-Control-Max-Age", std::to_string(cors_max_age) },
            { "Content-Length", std::to_string(0) }
        };

        connection->set_status(http_server_t::connection::ok);
        connection->set_headers(boost::make_iterator_range(headers, headers + 5));
        return;
    }

    if (!is_authenticated(request))
    {
        http_server_t::response_header headers[] =
        {
            { "Access-Control-Allow-Origin", cors_origin },
            { "Content-Type", "application/json" }
        };

        connection->set_status(http_server_t::connection::unauthorized);
        connection->set_headers(boost::make_iterator_range(headers, headers + 2));
        connection->write("\"Unauthorized\"");
        return;
    }

    if (request.destination != "/api")
    {
        http_server_t::response_header headers[] =
        {
            { "Access-Control-Allow-Origin", cors_origin },
            { "Content-Type", "application/json" }
        };

        connection->set_status(http_server_t::connection::bad_request);
        connection->set_headers(boost::make_iterator_range(headers, headers + 2));
        connection->write("\"Bad request\"");
        return;
    }

    boost::shared_ptr<connection_handler> handler(new connection_handler(request));
    handler->set_data_callback(boost::bind(&http_server::handle_incoming_data, this, _1, _2));
    (*handler)(connection);
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

bool http_server::is_authenticated(http_server_t::request const &request)
{
    auto header = std::find_if(request.headers.begin(), request.headers.end(), [](const boost::network::http::request_header_narrow& h)
    {
        return h.name == "Authorization";
    });

    std::string val = header == request.headers.end() ? "" : header->value;
    return auth_callback_(val);
}

void http_server::handle_incoming_data(http_server_t::connection_ptr connection, std::string data)
{
    std::string response = rpc_callback_(data);

    http_server_t::response_header headers[] =
    {
        { "Access-Control-Allow-Origin", config_.get("http.cors.origin", "*") },
        { "Connection", "close" },
        { "Content-Length", std::to_string(response.size()) },
        { "Content-Type", "application/json" }
    };

    connection->set_status(http_server_t::connection::status_t::ok);
    connection->set_headers(boost::make_iterator_range(headers, headers + 4));
    connection->write(response);
}

std::string http_server::ssl_password_callback(std::size_t max_length, boost::asio::ssl::context_base::password_purpose purpose)
{
    return config_.get("http.ssl.privateKeyPassword", "");
}
