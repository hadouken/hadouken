#include <boost/asio.hpp>
#include <boost/exception/all.hpp>
#include <boost/filesystem.hpp>
#include <boost/program_options.hpp>
#include <boost/property_tree/json_parser.hpp>
#include <boost/property_tree/ptree.hpp>

#include <iostream>

#include <hadouken/logger.hpp>
#include <hadouken/service_locator.hpp>
#include <hadouken/bittorrent/session.hpp>
#include <hadouken/http/http_server.hpp>
#include <hadouken/hosting/console_host.hpp>

#ifdef WIN32
    #include <hadouken/hosting/service_host.hpp>
    #define DAEMON_DESCRIPTION "Run Hadouken as a Windows Service."
#else
    #define DAEMON_DESCRIPTION "Run Hadouken as a daemon."
#endif

namespace fs = boost::filesystem;
namespace po = boost::program_options;
namespace pt = boost::property_tree;

hadouken::host* get_host(bool daemon)
{
    if (daemon)
    {
#ifdef WIN32
        return new hadouken::service_host();
#endif
    }
    else
    {
        return new hadouken::console_host();
    }
}

void read_program_options(int argc, char* argv[], po::variables_map& vm)
{
    po::options_description desc("Allowed options");
    desc.add_options()
        // The "--config=<path>" argument can be used to specify a different
        // configuration file. By default, the file "hadouken.json" is used
        // if it exists next to the exe file. If not, Hadouken will fail to
        // run.
        ("config", po::value<std::string>(), "If specified, a path to a .json file with configuration values for Hadouken")
        // The "--daemon" argument is passed if we want to run Hadouken
        // as a Windows service/Linux daemon.
        ("daemon", DAEMON_DESCRIPTION);

    // This is necessary to parse program options and not fail on unknown
    // arguments. We will notify the user of any unknown option, but still
    // function normally.

    po::parsed_options parsed_options = po::command_line_parser(argc, argv).options(desc).allow_unregistered().run();
    std::vector<std::string> unknown_options = po::collect_unrecognized(parsed_options.options, po::include_positional);

    for (auto opt : unknown_options)
    {
        HDKN_LOG(warning) << "Unknown option: " << opt;
    }

    po::store(parsed_options, vm);
    po::notify(vm);
}

int main(int argc, char* argv[])
{
    using namespace hadouken;
    using namespace hadouken::bittorrent;

    // Initialize logging. Important to do this first to trap any early log
    // messages.
    logger::init();

    std::string cmd_;
    for (int i = 0; i < argc; i++)
    {
        cmd_ += argv[i];
        cmd_ += " ";
    }

    HDKN_LOG(debug) << "Arguments: " << cmd_;

    // Read our program options.
    po::variables_map vm;
    read_program_options(argc, argv, vm);

    // Parse configuration file.
    fs::path config_path;

    if (vm.count("config"))
    {
        // Try to load the file from the specified path.
        config_path = fs::path(vm["config"].as<std::string>());
    }
    else
    {
        fs::path exe_path(argv[0]);
        config_path = exe_path.remove_filename() / "hadouken.json";
    }

    if (!fs::exists(config_path))
    {
        HDKN_LOG(fatal) << "Could not find hadouken.json configuration file. Exiting.";
        return 1;
    }

    HDKN_LOG(info) << "Loading configuration from " << config_path << ".";

    pt::ptree config;

    try
    {
        pt::json_parser::read_json(config_path.string(), config);
    }
    catch (const std::exception& e)
    {
        HDKN_LOG(fatal) << "Could not parse hadouken.json: " << e.what();
        return 1;
    }

    boost::asio::io_service* io_service = new boost::asio::io_service();

    // Add our services to the service locator. This is used throughout
    // Hadouken to get hold of various services you might need.
    session* sess = new session(config, *io_service);
    service_locator* locator = new service_locator();
    locator->add_service("bt.session", sess);
    locator->add_service("io_service", io_service);

    // Load the BitTorrent session.
    sess->load();

    // Start http server
    hadouken::http::http_server http_server(*io_service, 7070);
    http_server.add_rpc_handler("session.getTorrents", boost::bind(&session::api_session_get_torrents, sess, _1, _2));
    http_server.start();

    // Get and run the host based on the --daemon argument.
    int code = get_host(vm.count("daemon") > 0)->run(*io_service);

    http_server.stop();

    // Unload the plugin manager and BitTorrent session.
    sess->unload();

    // Free memory. Muy importante. Deleting the session is what
    // will shutdown libtorrent, so better not forget it.
    delete sess;
    delete io_service;

    return code;
}