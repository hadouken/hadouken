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

        try
        {
            AbstractConfiguration* folderView = config.createView(query);
            std::string sourcePath = folderView->getString("sourcePath");
            std::string filePattern = folderView->getString("filePattern");

            Folder fold(sourcePath, std::regex(filePattern));
            folders_.push_back(fold);
        }
        catch (Poco::Exception)
        {
            break;
        }
    }

    logger_.information("AutoAdd loaded, monitoring %z folder(s).", folders_.size());

    monitor_.start(Poco::TimerCallback<AutoAddExtension>(*this, &AutoAddExtension::monitor));
}

void AutoAddExtension::unload()
{
    monitor_.stop();
}

void AutoAddExtension::monitor(Poco::Timer& timer)
{
    logger_.debug("Checking folders.");

    Session& sess = Application::instance().getSubsystem<TorrentSubsystem>().getSession();

    for (Folder folder : folders_)
    {
        Poco::Path sourcePath(folder.getSourcePath());
        Poco::File sourceDir(sourcePath);

        if (!sourceDir.exists()) { continue; }

        std::vector<Poco::File> files;
        sourceDir.list(files);

        for (Poco::File file : files)
        {
            Poco::Path filePath(file.path());

            if (!std::regex_match(filePath.getFileName(), folder.getFilePattern()))
            {
                continue;
            }

            sess.addTorrentFile(filePath, AddTorrentParams());
            file.remove();
        }
    }
}
