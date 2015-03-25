#include "launcherextension.hpp"

#include <Hadouken/BitTorrent/Session.hpp>
#include <Hadouken/BitTorrent/TorrentHandle.hpp>
#include <Hadouken/BitTorrent/TorrentStatus.hpp>
#include <Hadouken/BitTorrent/TorrentSubsystem.hpp>
#include <Poco/Delegate.h>
#include <Poco/Process.h>

using namespace Launcher;
using namespace Hadouken::BitTorrent;
using namespace Poco::Util;

LauncherExtension::LauncherExtension()
    : logger_(Poco::Logger::get("launcher.launcherextension"))
{
}

void LauncherExtension::load(AbstractConfiguration& config)
{
    for (int i = 0; i < std::numeric_limits<int>::max(); i++)
    {
        std::string index = std::to_string(i);
        std::string query = "extensions.launcher.apps[" + index + "]";

        if (config.has(query))
        {
            AbstractConfiguration* appView = config.createView(query);

            std::string eventName = appView->getString("[0]");
            std::string app = appView->getString("[1]");

            if (launchers_.count(eventName))
            {
                launchers_.at(eventName).push_back(app);
            }
            else
            {
                std::vector<std::string> apps;
                apps.push_back(app);

                launchers_.insert(std::make_pair(eventName, apps));
            }
        }
        else
        {
            break;
        }
    }

    Session& sess = Application::instance().getSubsystem<TorrentSubsystem>().getSession();
    sess.onTorrentAdded += Poco::delegate(this, &LauncherExtension::onTorrentAdded);
    sess.onTorrentFinished += Poco::delegate(this, &LauncherExtension::onTorrentCompleted);
}

void LauncherExtension::unload()
{
    Session& sess = Application::instance().getSubsystem<TorrentSubsystem>().getSession();
    sess.onTorrentAdded -= Poco::delegate(this, &LauncherExtension::onTorrentAdded);
    sess.onTorrentFinished -= Poco::delegate(this, &LauncherExtension::onTorrentCompleted);
}

void LauncherExtension::launch(const std::string& eventName, TorrentHandle& handle)
{
    if (!launchers_.count(eventName))
    {
        return;
    }

    std::vector<std::string> apps = launchers_.at(eventName);

    std::vector<std::string> args;
    args.push_back(handle.getInfoHash());
    args.push_back(handle.getStatus().getName());
    args.push_back(handle.getStatus().getSavePath());

    for (std::string app : apps)
    {
        Poco::ProcessHandle handle = Poco::Process::launch(app, args);
        int result = handle.wait();

        if (result != 0)
        {
            logger_.error("Launcher app %s failed with error code %d.", app, result);
        }
    }
}

void LauncherExtension::onTorrentAdded(const void* sender, TorrentHandle& handle)
{
    launch("torrent.added", handle);
}

void LauncherExtension::onTorrentCompleted(const void* sender, TorrentHandle& handle)
{
    launch("torrent.finished", handle);
}
