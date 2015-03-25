#ifndef LAUNCHER_LAUNCHEREXTENSION_HPP
#define LAUNCHER_LAUNCHEREXTENSION_HPP

#include <Hadouken/Extensions/Extension.hpp>

#include <Poco/Logger.h>
#include <Poco/Util/AbstractConfiguration.h>
#include <map>
#include <string>

namespace Hadouken
{
    namespace BitTorrent
    {
        struct TorrentHandle;
    }
}

namespace Launcher
{
    class LauncherExtension : public Hadouken::Extensions::Extension
    {
    public:
        LauncherExtension();

        void load(Poco::Util::AbstractConfiguration& config);

        void unload();

    private:
        void launch(const std::string& eventName, Hadouken::BitTorrent::TorrentHandle& handle);

        void onTorrentAdded(const void* sender, Hadouken::BitTorrent::TorrentHandle& handle);
        void onTorrentCompleted(const void* sender, Hadouken::BitTorrent::TorrentHandle& handle);

        Poco::Logger& logger_;
        std::map<std::string, std::vector<std::string>> launchers_;
    };
}

#endif
