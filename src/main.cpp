#include <Hadouken/BitTorrent/TorrentSubsystem.hpp>
#include <Hadouken/Extensions/ExtensionSubsystem.hpp>
#include <Hadouken/Http/HttpSubsystem.hpp>
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
        : logger_(Poco::Logger::get("hadouken"))
    {
        setLogger(logger_);

        addSubsystem(new Hadouken::BitTorrent::TorrentSubsystem());
        addSubsystem(new Hadouken::Http::HttpSubsystem());
        addSubsystem(new Hadouken::Extensions::ExtensionSubsystem());
    }

protected:
#ifdef WIN32
    void loadServiceConfiguration(Application& app)
    {
        std::string serviceKey = "application.runAsService";

        if (!app.config().has(serviceKey)
            || !app.config().getBool(serviceKey))
        {
            return;
        }

        // If we run as a service (ie. config has property application.runAsService)
        // and we are on Windows, load the C:/ProgramData/Hadouken/config.json
        // configuration file. It should've been put there by the installer.

        Poco::Path dataPath = Hadouken::Platform::getApplicationDataPath();
        
        if (dataPath.toString().empty())
        {
            logger_.error("Could not retrieve application data path.");
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
            logger_.error("No config file found at '%s'.", configFile.path());
        }
    }
#endif

    void initialize(Application& self)
    {
        loadConfiguration();
#ifdef WIN32
        loadServiceConfiguration(self);
#endif

        Application::initialize(self);
    }

    int main(const ArgVec& args)
    {
        waitForTerminationRequest();
        return Application::EXIT_OK;
    }

private:
    Poco::Logger& logger_;
};

POCO_SERVER_MAIN(HadoukenApplication)
