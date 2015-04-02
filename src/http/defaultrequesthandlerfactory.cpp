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
    methods_.insert(std::make_pair("core.getSystemInfo", std::shared_ptr<RpcMethod>(new CoreGetSystemInfoMethod())));
    methods_.insert(std::make_pair("session.addTorrentFile", std::shared_ptr<RpcMethod>(new SessionAddTorrentFileMethod())));
    methods_.insert(std::make_pair("session.addTorrentUri", std::shared_ptr<RpcMethod>(new SessionAddTorrentUriMethod())));
    methods_.insert(std::make_pair("session.getProxy", std::shared_ptr<RpcMethod>(new SessionGetProxyMethod())));
    methods_.insert(std::make_pair("session.getStatus", std::shared_ptr<RpcMethod>(new SessionGetStatusMethod())));
    methods_.insert(std::make_pair("session.getTorrents", std::shared_ptr<RpcMethod>(new SessionGetTorrentsMethod())));
    methods_.insert(std::make_pair("session.removeTorrent", std::shared_ptr<RpcMethod>(new SessionRemoveTorrentMethod())));
    methods_.insert(std::make_pair("session.setProxy", std::shared_ptr<RpcMethod>(new SessionSetProxyMethod())));
    methods_.insert(std::make_pair("torrent.getFiles", std::shared_ptr<RpcMethod>(new TorrentGetFilesMethod())));
    methods_.insert(std::make_pair("torrent.getPeers", std::shared_ptr<RpcMethod>(new TorrentGetPeersMethod())));
    methods_.insert(std::make_pair("torrent.moveStorage", std::shared_ptr<RpcMethod>(new TorrentMoveStorageMethod())));
    methods_.insert(std::make_pair("torrent.pause", std::shared_ptr<RpcMethod>(new TorrentPauseMethod())));
    methods_.insert(std::make_pair("torrent.resume", std::shared_ptr<RpcMethod>(new TorrentResumeMethod())));

    wsConnectionManager_ = std::unique_ptr<WebSocketConnectionManager>(new WebSocketConnectionManager());

    // Set up virtual path. It should never be empty and it should
    // always start and end with "/".

    virtualPath_ = config.getString("http.root", "/");
    if (virtualPath_.empty()) virtualPath_ = "/";

    if (virtualPath_.at(0) != '/')
    {
        virtualPath_ = "/" + virtualPath_;
    }

    if (virtualPath_.at(virtualPath_.size() - 1) != '/')
    {
        virtualPath_ = virtualPath_ + "/";
    }
}

HTTPRequestHandler* DefaultRequestHandlerFactory::createRequestHandler(const HTTPServerRequest& request)
{
    if (request.getURI() == virtualPath_ + "api")
    {
        return new JsonRpcRequestHandler(config_, methods_);
    }

    if (request.getURI() == virtualPath_ + "events"
        && request.find("Upgrade") != request.end()
        && Poco::icompare(request["Upgrade"], "websocket") == 0)
    {
        return new WebSocketRequestHandler(*wsConnectionManager_);
    }

    return 0;
}
