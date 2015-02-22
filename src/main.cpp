#include <Hadouken/BitTorrent/TorrentSubsystem.hpp>
#include <Hadouken/Http/HttpSubsystem.hpp>
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
    }

protected:
    void initialize(Application& self)
    {
        loadConfiguration();
        Application::initialize(self);
    }

    int main(const ArgVec& args)
    {
        waitForTerminationRequest();
        return Application::EXIT_OK;
    }
};

POCO_SERVER_MAIN(HadoukenApplication)
