#include <Hadouken/Http/JsonRpc/SessionGetTorrentsMethod.hpp>

#include <Hadouken/BitTorrent/Session.hpp>
#include <Hadouken/BitTorrent/TorrentInfo.hpp>
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
    Return torrents in this session as a dictionary where the torrents info
    hash is key.

    {
      "<hash>": {
        "name": <string>,
        ...
      },

      ...
    }

    The input to this method is either an empty array [] which means that all
    torrents should be returned. If the array is non-empty, the elements of
    that array are hex-encoded info hashes telling which torrents to return.
    */
    
    Application& app = Application::instance();
    Session& sess = app.getSubsystem<TorrentSubsystem>().getSession();

    std::vector<TorrentHandle> handles;

    if (params->size() > 0)
    {
        for (Poco::Dynamic::Var item : *params)
        {
            std::string hash = item.extract<std::string>();
            TorrentHandle foundHandle = sess.findTorrent(hash);

            if (foundHandle.isValid())
            {
                handles.push_back(foundHandle);
            }
        }
    }
    else
    {
        handles = sess.getTorrents();
    }

    Poco::DynamicStruct result;

    for (TorrentHandle handle : handles)
    {
        std::unique_ptr<TorrentInfo> info = handle.getTorrentFile();
        TorrentStatus status = handle.getStatus();

        Poco::DynamicStruct data;
        data["name"] = status.getName();
        data["infoHash"] = status.getInfoHash();
        data["progress"] = status.getProgress();
        data["savePath"] = status.getSavePath();
        data["downloadRate"] = status.getDownloadRate();
        data["uploadRate"] = status.getUploadRate();
        data["downloadedBytes"] = status.getTotalDownload();
        data["downloadedBytesTotal"] = status.getAllTimeDownload();
        data["uploadedBytes"] = status.getTotalUpload();
        data["uploadedBytesTotal"] = status.getAllTimeUpload();
        data["numPeers"] = status.getNumPeers();
        data["numSeeds"] = status.getNumSeeds();

        if (info) {
            data["totalSize"] = info->getTotalSize();
        }
        else {
            data["totalSize"] = -1;
        }

        data["state"] = (int)status.getState();
        data["isFinished"] = status.isFinished();
        data["isMovingStorage"] = status.isMovingStorage();
        data["isPaused"] = status.isPaused();
        data["isSeeding"] = status.isSeeding();
        data["isSequentialDownload"] = status.isSequentialDownload();
        data["queuePosition"] = status.getQueuePosition();

        Poco::Dynamic::Array tags;
        std::vector<std::string> t;
        handle.getTags(t);

        for (std::string tag : t)
        {
            tags.push_back(tag);
        }

        data["tags"] = tags;

        result[handle.getInfoHash()] = data;
    }

    return new Poco::Dynamic::Var(result);
}
