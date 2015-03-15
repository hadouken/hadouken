#include <Hadouken/Http/DefaultRequestHandlerFactory.hpp>

#include <Hadouken/Http/JsonRpcRequestHandler.hpp>
#include <Hadouken/Http/WebSocketConnectionManager.hpp>
#include <Hadouken/Http/WebSocketRequestHandler.hpp>

#include <Hadouken/Http/JsonRpc/CoreGetSystemInfoMethod.hpp>
#include <Hadouken/Http/JsonRpc/SessionAddTorrentFileMethod.hpp>
#include <Hadouken/Http/JsonRpc/SessionAddTorrentUriMethod.hpp>
#include <Hadouken/Http/JsonRpc/SessionGetProxyMethod.hpp>
#include <Hadouken/Http/JsonRpc/SessionGetStatusMethod.hpp>
#include <Hadouken/Http/JsonRpc/SessionGetTorrentsMethod.hpp>
#include <Hadouken/Http/JsonRpc/SessionRemoveTorrentMethod.hpp>
#include <Hadouken/Http/JsonRpc/SessionSetProxyMethod.hpp>
#include <Hadouken/Http/JsonRpc/TorrentGetFilesMethod.hpp>
#include <Hadouken/Http/JsonRpc/TorrentGetPeersMethod.hpp>
#include <Hadouken/Http/JsonRpc/TorrentMoveStorageMethod.hpp>
#include <Hadouken/Http/JsonRpc/TorrentPauseMethod.hpp>
#include <Hadouken/Http/JsonRpc/TorrentResumeMethod.hpp>

#include <Poco/Net/HTTPServerResponse.h>
#include <Poco/Net/HTTPServerRequest.h>

using namespace Hadouken::Http;
using namespace Hadouken::Http::JsonRpc;
using namespace Poco::Net;

DefaultRequestHandlerFactory::DefaultRequestHandlerFactory(const Poco::Util::AbstractConfiguration& config)
    : config_(config)
{
    methods_.insert(std::make_pair("core.getSystemInfo", new CoreGetSystemInfoMethod()));
    methods_.insert(std::make_pair("session.addTorrentFile", new SessionAddTorrentFileMethod()));
    methods_.insert(std::make_pair("session.addTorrentUri", new SessionAddTorrentUriMethod()));
    methods_.insert(std::make_pair("session.getProxy", new SessionGetProxyMethod()));
    methods_.insert(std::make_pair("session.getStatus", new SessionGetStatusMethod()));
    methods_.insert(std::make_pair("session.getTorrents", new SessionGetTorrentsMethod()));
    methods_.insert(std::make_pair("session.removeTorrent", new SessionRemoveTorrentMethod()));
    methods_.insert(std::make_pair("session.setProxy", new SessionSetProxyMethod()));
    methods_.insert(std::make_pair("torrent.getFiles", new TorrentGetFilesMethod()));
    methods_.insert(std::make_pair("torrent.getPeers", new TorrentGetPeersMethod()));
    methods_.insert(std::make_pair("torrent.moveStorage", new TorrentMoveStorageMethod()));
    methods_.insert(std::make_pair("torrent.pause", new TorrentPauseMethod()));
    methods_.insert(std::make_pair("torrent.resume", new TorrentResumeMethod()));

    wsConnectionManager_ = new WebSocketConnectionManager();
}

DefaultRequestHandlerFactory::~DefaultRequestHandlerFactory()
{
    delete wsConnectionManager_;

    // Delete all registered RPC methods. 
    for (auto item : methods_)
    {
        delete item.second;
    }
}

HTTPRequestHandler* DefaultRequestHandlerFactory::createRequestHandler(const HTTPServerRequest& request)
{
    if (request.getURI() == "/api")
    {
        return new JsonRpcRequestHandler(config_, methods_);
    }

    if (request.getURI() == "/events"
        && request.find("Upgrade") != request.end()
        && Poco::icompare(request["Upgrade"], "websocket") == 0)
    {
        return new WebSocketRequestHandler(*wsConnectionManager_);
    }

    return 0;
}
