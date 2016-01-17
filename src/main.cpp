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

po::variables_map load_options(int argc,char *argv[])
{
    po::options_description desc("Allowed options");
    desc.add_options()
        ("help,h", "Display available commandline options")
        ("config", po::value<std::string>(), "Set path to a JSON configuration file. The default is %appdir%/hadouken.json")
        ("daemon", "Start Hadouken in daemon/service mode.")
#ifdef WIN32
        ("install-service", "Install Hadouken in the SCM.")
        ("uninstall-service", "Uninstall Hadouken from the SCM.")
#endif
        ;

    po::variables_map vm;
    po::store(po::parse_command_line(argc, argv, desc), vm);
    po::notify(vm);
    
    if (vm.count("help")) {
        cout << desc << "\n";
        return 1;
    }
    
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
    po::variables_map vm = load_options(argc, argv);
    hadouken::logging::setup(vm);

    // Do platform-specific initialization as early as possible.
    hadouken::platform::init();

#ifdef WIN32
    if (vm.count("install-service"))
    {
        hadouken::platform::install_service();
        return 0;
    }
    else if (vm.count("uninstall-service"))
    {
        hadouken::platform::uninstall_service();
        return 0;
    }
#endif

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
        }
        catch (const std::exception& ex)
        {
            // To meet expectations, fail if the configuration file cannot be parsed. Otherwise,
            // we will revert to the default config which might be totally wrong.
            BOOST_LOG_TRIVIAL(fatal) << "Error when loading config file " << config_file << ": " << ex.what();
            return EXIT_FAILURE;
        }
    }

    boost::shared_ptr<boost::asio::io_service> io(new boost::asio::io_service());
    
    hadouken::application app(io, config);
    app.script_host().define_global("__CONFIG__", config_file.string());
    app.script_host().define_global("__CONFIG_PATH__", config_file.parent_path().string());

    app.start();
    int result = get_host(vm)->wait_for_exit(io);
    app.stop();

    return result;
}
