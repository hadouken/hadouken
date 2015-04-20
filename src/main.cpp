#include <Hadouken/BitTorrent/TorrentSubsystem.hpp>
#include <Hadouken/Extensions/ExtensionSubsystem.hpp>
#include <Hadouken/Http/HttpSubsystem.hpp>
#include <Hadouken/Scripting/ScriptingSubsystem.hpp>
#include <Hadouken/Platform.hpp>
#include <Poco/File.h>
#include <Poco/Path.h>
#include <Poco/Util/ServerApplication.h>

#include <boost/asio/impl/src.hpp>
#include <iostream>

using namespace Poco::Util;

class HadoukenApplication : public ServerApplication
{
public:
    HadoukenApplication()
    {
        Application::config().setString("application.logger", "hadouken");

        addSubsystem(new Hadouken::BitTorrent::TorrentSubsystem());
        addSubsystem(new Hadouken::Http::HttpSubsystem());
        addSubsystem(new Hadouken::Extensions::ExtensionSubsystem());
        addSubsystem(new Hadouken::Scripting::ScriptingSubsystem());
    }

protected:
    void defineOptions(OptionSet& options)
    {
        ServerApplication::defineOptions(options);

        // Add option for separate configuration file
        options.addOption(Option("config", "c")
            .argument("a path to a configuration file")
            .binding("configFile"));
    }

    void loadServiceConfiguration(Application& app)
    {
#ifdef WIN32
        std::string serviceKey = "application.runAsService";
#else
        std::string serviceKey = "application.runAsDaemon";
#endif

        if (!app.config().getBool(serviceKey, false))
        {
            return;
        }

        // If we run as a service (ie. config has property application.runAsService)
        // and we are on Windows, load the C:/ProgramData/Hadouken/config.json
        // configuration file. It should've been put there by the installer.

        // If we run as a daemon (ie. config has property application.runAsDaemon)
        // and we are on Linux, load the /etc/hadouken/config.json configuration
        // file.

        Poco::Path dataPath = Hadouken::Platform::getApplicationDataPath();
        
        if (dataPath.toString().empty())
        {
            app.logger().error("Could not retrieve application data path.");
            return;
        }

        Poco::Path configPath(dataPath, "config.json");
        Poco::File configFile(configPath);

        if (configFile.exists())
        {
            loadConfiguration(configFile.path());
        }
        else
        {
            app.logger().error("No config file found at '%s'.", configFile.path());
        }
    }

    void initialize(Application& self)
    {
        loadConfiguration();

        std::string configFile = self.config().getString("configFile", "");

        if (!configFile.empty())
        {
            if (Poco::File(configFile).exists())
            {
                self.logger().information("Loading configuration from %s.", configFile);
                loadConfiguration(configFile);
            }
            else
            {
                self.logger().error("Configuration file %s does not exist.", configFile);
            }
        }
        else
        {
            // Load default configuration file
            loadServiceConfiguration(self);
        }

        Application::initialize(self);
    }

    int main(const ArgVec& args)
    {
        waitForTerminationRequest();
        return Application::EXIT_OK;
    }
};

POCO_SERVER_MAIN(HadoukenApplication)
