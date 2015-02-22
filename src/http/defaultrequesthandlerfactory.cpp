#include <Hadouken/Http/DefaultRequestHandlerFactory.hpp>

#include <Hadouken/Http/JsonRpcRequestHandler.hpp>
#include <Hadouken/Http/JsonRpc/SessionAddTorrentFileMethod.hpp>
#include <Hadouken/Http/JsonRpc/SessionGetTorrentsMethod.hpp>

#include <Poco/Net/HTTPServerResponse.h>
#include <Poco/Net/HTTPServerRequest.h>

using namespace Hadouken::Http;
using namespace Hadouken::Http::JsonRpc;
using namespace Poco::Net;

DefaultRequestHandlerFactory::DefaultRequestHandlerFactory()
{
    methods_.insert(std::make_pair("session.addTorrentFile", new SessionAddTorrentFileMethod()));
    methods_.insert(std::make_pair("session.getTorrents", new SessionGetTorrentsMethod()));
}

DefaultRequestHandlerFactory::~DefaultRequestHandlerFactory()
{
    // Delete all registered RPC methods. 
    for (auto item : methods_)
    {
        delete item.second;
    }
}

HTTPRequestHandler* DefaultRequestHandlerFactory::createRequestHandler(const HTTPServerRequest& request)
{
    if (request.getURI() == "/api"
        && request.getMethod() == "POST")
    {
        return new JsonRpcRequestHandler(methods_);
    }

    return 0;
}
