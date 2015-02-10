#include <boost/asio.hpp>
#include <boost/program_options.hpp>

#include <hadouken/plugin_manager.hpp>
#include <hadouken/service_locator.hpp>
#include <hadouken/bittorrent/session.hpp>

#include "console_host.hpp"

#ifdef WIN32
#include "service_host.hpp"
#endif

int main(int argc, char* argv[])
{
    namespace po = boost::program_options;

    using namespace hadouken;
    using namespace hadouken::bittorrent;

    po::options_description desc("Allowed options");
    desc.add_options()
        ("daemon", "run as a daemon/service");

    po::variables_map vm;
    po::store(po::parse_command_line(argc, argv, desc), vm);
    po::notify(vm);

    boost::asio::io_service* io_service = new boost::asio::io_service();

    service_locator* locator = new service_locator();
    locator->add_service("bt.session", new session(*io_service));
    locator->add_service("plugin_manager", new plugin_manager(*locator));
    locator->add_service("io_service", io_service);

    host* host;

    if (vm.count("daemon"))
    {
#ifdef WIN32
        host = new service_host();
#endif
    }
    else
    {
        host = new console_host(*locator);
    }

    return host->run();
}