#include <Hadouken/BitTorrent/TorrentSubsystem.hpp>
#include <Hadouken/Extensions/ExtensionSubsystem.hpp>
#include <Hadouken/Http/HttpSubsystem.hpp>
#include <Poco/File.h>
#include <Poco/Util/ServerApplication.h>

#include <boost/asio/impl/src.hpp>
#include <iostream>

using namespace Poco::Util;

class HadoukenApplication : public ServerApplication
{
public:
    HadoukenApplication()
    {
        addSubsystem(new Hadouken::BitTorrent::TorrentSubsystem());
        addSubsystem(new Hadouken::Http::HttpSubsystem());
        addSubsystem(new Hadouken::Extensions::ExtensionSubsystem());
    }

protected:
    void loadServiceConfiguration(Application& app)
    {
#ifdef WIN32
        std::string serviceKey = "application.runAsService";
#else
        std::string serviceKey = "application.runAsDaemon";
#endif

        // Hadouken supports a separate configuration file that is
        // loaded if we run as a daemon or if we specify a True value
        // for the property "hadouken.service_config_force".

        std::string forceKey = "hadouken.service_config_force";
        std::string pathKey = "hadouken.service_config_path";

        if (app.config().hasProperty(pathKey)
            && ((app.config().hasProperty(serviceKey) && app.config().getBool(serviceKey))
            || app.config().hasProperty(forceKey) && app.config().getBool(forceKey)))
        {
            std::string configPath = app.config().getString(pathKey);

            if (Poco::File(configPath).exists())
            {
                app.logger().information("Loading service configuration from '%s'.", configPath);
                loadConfiguration(configPath);
            }
            else
            {
                app.logger().warning("Could not find configuration file '%s'.", configPath);
            }
        }
    }

    void initialize(Application& self)
    {
        loadConfiguration();
        loadServiceConfiguration(self);

        Application::initialize(self);
    }

    int main(const ArgVec& args)
    {
        waitForTerminationRequest();
        return Application::EXIT_OK;
    }
};

POCO_SERVER_MAIN(HadoukenApplication)
