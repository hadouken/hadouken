#include <iostream>

#include <boost/asio.hpp>
#include <boost/log/trivial.hpp>
#include <boost/program_options.hpp>

#include "jsonrpc/jsonrpc_server.hpp"
#include "jsonrpc/methods/session_get_settings.hpp"
#include "jsonrpc/methods/torrents_add.hpp"
#include "http/http_server.hpp"
#include "session_manager.hpp"

namespace jsonrpc_methods = hadouken::jsonrpc::methods;
namespace po = boost::program_options;

int main(int argc, char* argv[])
{
    po::options_description options("hadouken");
    options.add_options()
        ("help", "Display available command line options")
        ("webui-path", po::value<std::string>(), "Path to the web UI files")
        ("webui-port", po::value<int>(), "Port where the web UI will listen");

    po::variables_map vm;

    try
    {
        po::store(
            po::parse_command_line(argc, argv, options),
            vm);
    }
    catch (po::unknown_option const& e)
    {
        std::cerr << e.what() << std::endl;
        return -1;
    }

    po::notify(vm);

    if (vm.count("help"))
    {
        std::cout << options << std::endl;
        return 0;
    }

    BOOST_LOG_TRIVIAL(info) << "Hadouken - built with love by Unidentified Developer";

    boost::asio::io_service io;

    // signals
    boost::asio::signal_set signals(io, SIGINT, SIGTERM);

    signals.async_wait([&io](boost::system::error_code const& ec, int sig)
    {
        if (ec)
        {
            BOOST_LOG_TRIVIAL(error) << ec;
        }

        io.stop();
    });

    auto sess = std::make_shared<hadouken::session_manager>();

    auto http = std::make_shared<hadouken::http::http_server>(
        io,
        vm["webui-port"].as<int>());

    http->jsonrpc().add_method("session.get_settings", std::make_shared<jsonrpc_methods::session_get_settings>(sess));
    http->jsonrpc().add_method("torrents.add", std::make_shared<jsonrpc_methods::torrents_add>(sess));
    http->run();

    return io.run();
}
