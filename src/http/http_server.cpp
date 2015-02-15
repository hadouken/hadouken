#include <hadouken/http/http_server.hpp>

#include <boost/bind.hpp>
#include <boost/property_tree/ptree.hpp>
#include <boost/property_tree/json_parser.hpp>
#include <hadouken/http/connection.hpp>
#include <hadouken/logger.hpp>

namespace pt = boost::property_tree;
using boost::asio::ip::tcp;
using namespace hadouken::http;

http_server::http_server(const pt::ptree& config, boost::asio::io_service& io_service)
    : config_(config),
    io_service_(io_service)
{
    acceptor_ = new tcp::acceptor(io_service);
    socket_ = new tcp::socket(io_service_);
}

http_server::~http_server()
{
    // For some reason, these two lines causes an access violation exception.
    return;

    if (socket_)
    {
        delete socket_;
    }

    if (acceptor_)
    {
        delete acceptor_;
    }
}

void http_server::start()
{
    int port = -1;

    try
    {
        pt::ptree server_config = config_.get_child("http");
        port = server_config.get<int>("port");
    }
    catch (const std::exception& e)
    {
        HDKN_LOG(error) << "Could not parse HTTP port: " << e.what() << ".";
        return;
    }

    acceptor_->open(tcp::v4());
    acceptor_->bind(tcp::endpoint(tcp::v4(), port));
    acceptor_->set_option(boost::asio::ip::tcp::acceptor::reuse_address(true));
    acceptor_->listen();

    HDKN_LOG(info) << "HTTP server listening on port " << port << ".";

    do_accept();
}

void http_server::stop()
{
    acceptor_->close();
    manager_.stop_all();
}

void http_server::add_rpc_handler(const std::string& method_name, rpc_handler_t handler)
{
    boost::mutex::scoped_lock handlers_lock(handlers_mutex_);
    handlers_.insert(std::make_pair(method_name, handler));
}

bool http_server::find_rpc_handler(const std::string& method_name, rpc_handler_t& handler)
{
    boost::mutex::scoped_lock handlers_lock(handlers_mutex_);

    if (handlers_.empty())
    {
        return false;
    }

    handler_map_t::iterator it = handlers_.find(method_name);

    if (it != handlers_.end())
    {
        handler = it->second;
        return true;
    }

    return false;
}

void http_server::handle_incoming_request(const request& request, reply& reply)
{
    /*
    The main request handler. Checks if it is an API request and then
    constructs an appropriate response.
    */

    if (request.method == "POST"
        && (request.uri == "/api" || request.uri == "/api/"))
    {
        // Parse incoming JSON request.
        pt::ptree rpc_request;
        std::stringstream ss;
        ss << request.body;

        pt::json_parser::read_json(ss, rpc_request);
        std::string method = rpc_request.get<std::string>("method");

        rpc_handler_t handler;

        if (find_rpc_handler(method, handler))
        {
            pt::ptree result;
            handler(rpc_request.get_child("params"), result);

            std::stringstream out;
            pt::json_parser::write_json(out, result);

            reply.status = reply::ok;
            reply.content = out.str();
        }
        else
        {
            reply.content = "Not found.";
            reply.status = reply::ok;
        }
    }
}

void http_server::do_accept()
{
    acceptor_->async_accept(*socket_, [this](const boost::system::error_code& error)
    {
        if (!acceptor_->is_open())
        {
            return;
        }

        if (!error)
        {
            connection_ptr conn = std::make_shared<connection>(boost::move(*socket_),
                manager_);

            conn->on_incoming_request(boost::bind(&http_server::handle_incoming_request, this, _1, _2));

            manager_.start(conn);
        }

        do_accept();
    });
}
