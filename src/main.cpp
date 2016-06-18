#include <boost/asio.hpp>
#include <boost/filesystem.hpp>
#include <boost/log/trivial.hpp>
#include <boost/program_options.hpp>
#include <boost/property_tree/ptree.hpp>
#include <boost/property_tree/json_parser.hpp>
#include <hadouken/application.hpp>
#include <hadouken/logging.hpp>
#include <hadouken/platform.hpp>
#include <hadouken/hosting/host.hpp>
#include <hadouken/hosting/console_host.hpp>

#ifdef WIN32
#include <hadouken/hosting/service_host.hpp>
#endif

namespace fs = boost::filesystem;
namespace po = boost::program_options;
namespace pt = boost::property_tree;

static po::options_description desc("Allowed options");
static po::options_description hidden_options("Hidden options");

po::variables_map load_options(int argc,char *argv[])
{
    po::options_description cmdline_options;
    cmdline_options.add(desc).add(hidden_options);

    po::variables_map vm;
    po::store(po::parse_command_line(argc, argv, cmdline_options), vm);
    po::notify(vm);

    return vm;
}

fs::path get_config_path(const po::variables_map& options)
{
    if (options.count("config"))
    {
        return options["config"].as<std::string>();
    }
    else if (options.count("daemon"))
    {
        return (hadouken::platform::data_path() / "hadouken.json");
    }
    else
    {
        return (hadouken::platform::get_current_directory() / "hadouken.json");
    }
}

std::unique_ptr<hadouken::hosting::host> get_host(const po::variables_map& options)
{
    if (options.count("daemon"))
    {
#ifdef WIN32
        return std::unique_ptr<hadouken::hosting::host>(new hadouken::hosting::service_host());
#endif
    }

    return std::unique_ptr<hadouken::hosting::host>(new hadouken::hosting::console_host());
}

int main(int argc, char *argv[])
{
    desc.add_options()
        ("help,h", "Display available commandline options")
        ("config", po::value<std::string>(), "Set path to a JSON configuration file. The default is %appdir%/hadouken.json")
        ("daemon", "Start Hadouken in daemon/service mode.")
#ifdef WIN32
        ("install-service", "Install Hadouken in the SCM.")
        ("uninstall-service", "Uninstall Hadouken from the SCM.")
#endif
        ;

    hidden_options.add_options()
#ifdef WIN32
        ("runas", "Do not attempt to relaunch as a different user when configuring the service. Used by Hadouken itself to prevent recursive relaunches.")
#endif
        ;

    po::variables_map vm;
    try
    {
        vm = load_options(argc, argv);
    }
    catch (po::unknown_option& ex)
    {
        BOOST_LOG_TRIVIAL(error) << ex.what();
        std::cerr << std::endl << desc << std::endl;

        return EXIT_FAILURE;
    }

    // Print Help and exit if we get --help or -h on the command-line.
    if (vm.count("help"))
    {
        std::cout << desc << std::endl;
        return EXIT_SUCCESS;
    }

    hadouken::logging::setup(vm);

    // Do platform-specific initialization as early as possible.
    hadouken::platform::init();

#ifdef WIN32
    bool attempt_elevation = !vm.count("runas");

    if (vm.count("install-service"))
    {
        hadouken::platform::install_service(attempt_elevation);
        return EXIT_SUCCESS;
    }
    else if (vm.count("uninstall-service"))
    {
        hadouken::platform::uninstall_service(attempt_elevation);
        return EXIT_SUCCESS;
    }
#endif

    boost::shared_ptr<boost::asio::io_service> io(new boost::asio::io_service());
    std::unique_ptr<hadouken::hosting::host> host = get_host(vm);
    int result = EXIT_FAILURE;

    // Start initialization as early as possible so that failures can be reported to the SCM if running as a service on Windows.
    // Otherwise the SCM will wait for some timeout before declaring Hadouken as failing to start.
    result = host->initialization_start(io);

    fs::path config_file = get_config_path(vm);
    pt::ptree config;

    if (!fs::exists(config_file))
    {
        BOOST_LOG_TRIVIAL(warning) << "Could not find config file at " << config_file;
    }
    else
    {
        try
        {
            pt::read_json(config_file.string(), config);
            BOOST_LOG_TRIVIAL(info) << "Loaded config file " << config_file;
            result = EXIT_SUCCESS;
        }
        catch (const std::exception& ex)
        {
            // To meet expectations, fail if the configuration file cannot be parsed. Otherwise,
            // we will revert to the default config which might be totally wrong.
            BOOST_LOG_TRIVIAL(fatal) << "Error when loading config file " << config_file << ": " << ex.what();
            result = EXIT_FAILURE;
        }
    }

    hadouken::application app(io, config);

    if (result == EXIT_SUCCESS)
    {
        try
        {
            app.script_host().define_global("__CONFIG__", config_file.string());
            app.script_host().define_global("__CONFIG_PATH__", config_file.parent_path().string());

            app.start();
        }
        catch (std::exception& ex)
        {
            BOOST_LOG_TRIVIAL(fatal) << "Error starting Hadouken " << ex.what();
            result = EXIT_FAILURE;
        }
    }

    host->initialization_complete(result);

    if (result == EXIT_SUCCESS)
    {
        result = host->wait_for_exit();
    }

    host->shutdown_start();

    try
    {
        app.stop();
    }
    catch (const std::exception&)
    {
    }

    // If running as a service, the process may be terminated at any time after calling this function
    host->shutdown_complete(result);

    return result;
}
