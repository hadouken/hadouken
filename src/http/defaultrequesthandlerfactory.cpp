#include <Hadouken/Http/DefaultRequestHandlerFactory.hpp>

#include <Hadouken/Http/JsonRpcRequestHandler.hpp>
#include <Hadouken/Http/WebSocketConnectionManager.hpp>
#include <Hadouken/Http/WebSocketRequestHandler.hpp>
#include <Poco/Net/HTTPServerResponse.h>
#include <Poco/Net/HTTPServerRequest.h>

using namespace Hadouken::Http;
using namespace Poco::Net;

DefaultRequestHandlerFactory::DefaultRequestHandlerFactory(const Poco::Util::AbstractConfiguration& config)
    : config_(config)
{
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
        return new JsonRpcRequestHandler(config_);
    }

    if (request.getURI() == virtualPath_ + "events"
        && request.find("Upgrade") != request.end()
        && Poco::icompare(request["Upgrade"], "websocket") == 0)
    {
        return new WebSocketRequestHandler(*wsConnectionManager_);
    }

    return 0;
}
