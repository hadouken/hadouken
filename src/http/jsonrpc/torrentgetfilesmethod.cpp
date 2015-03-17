#include <Hadouken/Http/JsonRpc/TorrentGetFilesMethod.hpp>

#include <Hadouken/BitTorrent/FileEntry.hpp>
#include <Hadouken/BitTorrent/FileStorage.hpp>
#include <Hadouken/BitTorrent/Session.hpp>
#include <Hadouken/BitTorrent/TorrentInfo.hpp>
#include <Hadouken/BitTorrent/TorrentHandle.hpp>
#include <Hadouken/BitTorrent/TorrentSubsystem.hpp>
#include <Poco/Util/Application.h>

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Http::JsonRpc;
using namespace Poco::JSON;
using namespace Poco::Util;

Poco::Dynamic::Var::Ptr TorrentGetFilesMethod::execute(const Array::Ptr& params)
{
    Application& app = Application::instance();
    Session& sess = app.getSubsystem<TorrentSubsystem>().getSession();

    if (params->size() < 1)
    {
        return nullptr;
    }

    std::string hash = params->getElement<std::string>(0);
    TorrentHandle handle = sess.findTorrent(hash);

    if (!handle.isValid())
    {
        return nullptr;
    }

    std::unique_ptr<TorrentInfo> info = handle.getTorrentFile();
    Poco::Dynamic::Array result;

    if (!info) {
        return nullptr;
    }

    FileStorage files = info->getFiles();
    
    std::vector<int64_t> progress;
    handle.getFileProgress(progress);
    
    for (int i = 0; i < files.getNumFiles(); i++)
    {
        FileEntry entry = files.getEntryAt(i);

        Poco::DynamicStruct obj;
        obj["index"] = i;
        obj["path"] = entry.getPath();
        obj["progress"] = progress[i];
        obj["size"] = entry.getSize();

        result.push_back(obj);
    }

    return new Poco::Dynamic::Var(result);
}
