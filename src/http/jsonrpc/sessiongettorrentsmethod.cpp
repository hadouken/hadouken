#include <Hadouken/Http/JsonRpc/SessionGetTorrentsMethod.hpp>

#include <Hadouken/BitTorrent/Session.hpp>
#include <Hadouken/BitTorrent/TorrentHandle.hpp>
#include <Hadouken/BitTorrent/TorrentStatus.hpp>
#include <Hadouken/BitTorrent/TorrentSubsystem.hpp>
#include <Poco/Util/Application.h>

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Http::JsonRpc;
using namespace Poco::JSON;
using namespace Poco::Util;

Poco::Dynamic::Var::Ptr SessionGetTorrentsMethod::execute(const Array::Ptr& params)
{
    /*
    Return all torrents in this session in a dictionary where the torrents info
    hash is key.

    {
      "<hash>": {
        "name": <string>,
        ...
      },

      ...
    }
    */
    
    Application& app = Application::instance();
    Session& sess = app.getSubsystem<TorrentSubsystem>().getSession();

    std::vector<TorrentHandle> handles = sess.getTorrents();
    Poco::DynamicStruct result;

    for (auto handle : handles)
    {
        TorrentStatus status = handle.getStatus();

        Poco::DynamicStruct data;
        data["name"] = status.getName();
        data["infoHash"] = status.getInfoHash();
        data["progress"] = status.getProgress();
        data["savePath"] = status.getSavePath();
        data["downloadRate"] = status.getDownloadRate();
        data["uploadRate"] = status.getUploadRate();
        data["numPeers"] = status.getNumPeers();
        data["numSeeds"] = status.getNumSeeds();

        result[handle.getInfoHash()] = data;
    }

    return new Poco::Dynamic::Var(result);
}
