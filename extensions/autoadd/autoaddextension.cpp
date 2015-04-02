#define NOMINMAX

#include "autoaddextension.hpp"

#include <Hadouken/BitTorrent/AddTorrentParams.hpp>
#include <Hadouken/BitTorrent/Session.hpp>
#include <Hadouken/BitTorrent/TorrentSubsystem.hpp>
#include <limits>
#include <Poco/File.h>
#include <Poco/Path.h>

using namespace AutoAdd;
using namespace Hadouken::BitTorrent;
using namespace Poco::Util;

AutoAddExtension::AutoAddExtension()
    : logger_(Poco::Logger::get("autoadd.autoaddextension")),
      monitor_(0, 5000)
{
}

void AutoAddExtension::load(AbstractConfiguration& config)
{
    for (int i = 0; i < std::numeric_limits<int>::max(); i++)
    {
        std::string index = std::to_string(i);
        std::string query = "extensions.autoadd.folders[" + index + "]";

        if (config.has(query))
        {
            AbstractConfiguration* folderView = config.createView(query);

            if (!folderView->has("path"))
            {
                logger_.error("Missing 'path' for AutoAdd item.");
                continue;
            }

            Folder f;
            f.path = folderView->getString("path");
            f.pattern = std::regex(folderView->getString("pattern", ".*\\.torrent$"));
            f.savePath = folderView->getString("savePath", std::string());

            if (folderView->has("tags"))
            {
                for (int j = 0; j < std::numeric_limits<int>::max(); j++)
                {
                    index = std::to_string(j);
                    query = "tags[" + index + "]";

                    if (folderView->has(query))
                    {
                        f.tags.push_back(folderView->getString(query));
                    }
                    else
                    {
                        break;
                    }
                }
            }

            folders_.push_back(f);
        }
        else
        {
            break;
        }
    }

    if (config.has("extensions.autoadd.interval"))
    {
        monitor_.setPeriodicInterval((uint64_t)config.getInt64("extensions.autoadd.interval"));
    }

    logger_.information("AutoAdd loaded, monitoring %z folder(s) with interval %ldms.",
        folders_.size(),
        monitor_.getPeriodicInterval());

    monitor_.start(Poco::TimerCallback<AutoAddExtension>(*this, &AutoAddExtension::monitor));
}

void AutoAddExtension::unload()
{
    monitor_.stop();
}

void AutoAddExtension::monitor(Poco::Timer& timer)
{
    Session& sess = Application::instance().getSubsystem<TorrentSubsystem>().getSession();

    for (Folder folder : folders_)
    {
        Poco::Path sourcePath(folder.path);
        Poco::File sourceDir(sourcePath);

        if (!sourceDir.exists()) { continue; }

        std::vector<Poco::File> files;
        sourceDir.list(files);

        for (Poco::File file : files)
        {
            Poco::Path filePath(file.path());
            std::string fileName = filePath.getFileName();

            if (!std::regex_match(fileName, folder.pattern))
            {
                continue;
            }

            AddTorrentParams p;
            p.savePath = folder.savePath;
            p.tags = folder.tags;

            sess.addTorrentFile(filePath, p);
            file.remove();
        }
    }
}
