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
        // and we are on Windows. If the file C:/ProgramData/Hadouken/config.json.template
        // exists, and not C:/ProgramData/Hadouken/config.json, we copy the template
        // file to "config.json" and then load it.

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
            logger_.information("Config file already exists at '%s'. Not copying template.", configFile.path());
            loadConfiguration(configFile.path());
            return;
        }

        Poco::Path templatePath(dataPath, "config.json.template");
        Poco::File templateFile(templatePath);

        if (!templateFile.exists())
        {
            logger_.error("Config template file '%s' does not exist.", templateFile.path());
            return;
        }

        logger_.information("Copying template '%s' to '%s'.", templateFile.path(), configFile.path());

        try
        {
            templateFile.copyTo(configFile.path());
        }
        catch (Poco::Exception& exception)
        {
            logger_.error("Could not copy config template: %s.", exception.displayText());
            return;
        }

        // Load the configuration.
        loadConfiguration(configFile.path());
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
