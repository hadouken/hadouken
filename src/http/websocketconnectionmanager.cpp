#include <Hadouken/Http/WebSocketConnectionManager.hpp>

#include <Hadouken/BitTorrent/Session.hpp>
#include <Hadouken/BitTorrent/TorrentHandle.hpp>
#include <Hadouken/BitTorrent/TorrentInfo.hpp>
#include <Hadouken/BitTorrent/TorrentStatus.hpp>
#include <Hadouken/BitTorrent/TorrentSubsystem.hpp>
#include <Poco/Delegate.h>
#include <Poco/Dynamic/Struct.h>
#include <Poco/Util/Application.h>

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Http;
using namespace Poco::Util;

WebSocketConnectionManager::WebSocketConnectionManager()
{
    Application& app = Application::instance();
    Session& sess = app.getSubsystem<TorrentSubsystem>().getSession();

    sess.onTorrentAdded += Poco::delegate(this, &WebSocketConnectionManager::onTorrentAdded);
    sess.onTorrentRemoved += Poco::delegate(this, &WebSocketConnectionManager::onTorrentRemoved);
}

WebSocketConnectionManager::~WebSocketConnectionManager()
{
    Poco::Mutex::ScopedLock lock(socketsMutex_);

    Application& app = Application::instance();
    Session& sess = app.getSubsystem<TorrentSubsystem>().getSession();

    for (auto socket : sockets_)
    {
        socket.close();
    }

    sess.onTorrentRemoved -= Poco::delegate(this, &WebSocketConnectionManager::onTorrentRemoved);
    sess.onTorrentAdded -= Poco::delegate(this, &WebSocketConnectionManager::onTorrentAdded);
}

void WebSocketConnectionManager::connect(Poco::Net::WebSocket& webSocket)
{
    Poco::Mutex::ScopedLock lock(socketsMutex_);
    sockets_.push_back(webSocket);
}

void WebSocketConnectionManager::disconnect(Poco::Net::WebSocket& webSocket)
{
    Poco::Mutex::ScopedLock lock(socketsMutex_);

    auto it = std::find(sockets_.begin(), sockets_.end(), webSocket);

    if (it != sockets_.end())
    {
        sockets_.erase(it);
    }
}

void WebSocketConnectionManager::onTorrentAdded(const void* sender, std::shared_ptr<TorrentHandle>& handle)
{
    std::unique_ptr<TorrentInfo> info = handle->getTorrentFile();
    TorrentStatus status = handle->getStatus();

    Poco::DynamicStruct torrent;
    torrent["name"] = status.getName();
    torrent["infoHash"] = status.getInfoHash();
    torrent["progress"] = status.getProgress();
    torrent["savePath"] = status.getSavePath();
    torrent["downloadRate"] = status.getDownloadRate();
    torrent["uploadRate"] = status.getUploadRate();
    torrent["downloadedBytes"] = status.getTotalDownload();
    torrent["downloadedBytesTotal"] = status.getAllTimeDownload();
    torrent["uploadedBytes"] = status.getTotalUpload();
    torrent["uploadedBytesTotal"] = status.getAllTimeUpload();
    torrent["numPeers"] = status.getNumPeers();
    torrent["numSeeds"] = status.getNumSeeds();

    if (info) {
        torrent["totalSize"] = info->getTotalSize();
    }
    else {
        torrent["totalSize"] = -1;
    }

    torrent["state"] = (int)status.getState();
    torrent["isFinished"] = status.isFinished();
    torrent["isMovingStorage"] = status.isMovingStorage();
    torrent["isPaused"] = status.isPaused();
    torrent["isSeeding"] = status.isSeeding();
    torrent["isSequentialDownload"] = status.isSequentialDownload();
    torrent["queuePosition"] = status.getQueuePosition();
    torrent["tags"] = handle->getTags();

    Poco::DynamicStruct ev;
    ev["event"] = "torrent.added";
    ev["torrent"] = torrent;

    sendMessage(ev.toString());
}

void WebSocketConnectionManager::onTorrentRemoved(const void* sender, std::string& infoHash)
{
    Poco::DynamicStruct torrent;
    torrent["infoHash"] = infoHash;

    Poco::DynamicStruct ev;
    ev["event"] = "torrent.removed";
    ev["torrent"] = torrent;

    sendMessage(ev.toString());
}

void WebSocketConnectionManager::sendMessage(std::string message)
{
    Poco::Mutex::ScopedLock lock(socketsMutex_);

    for (auto socket : sockets_)
    {
        socket.sendFrame(message.c_str(), message.size());
    }
}
