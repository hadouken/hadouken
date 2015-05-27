#include <Hadouken/Http/DefaultRequestHandlerFactory.hpp>

#include <Hadouken/Http/JsonRpcRequestHandler.hpp>
#include <Poco/Net/HTTPServerResponse.h>
#include <Poco/Net/HTTPServerRequest.h>

using namespace Hadouken::Http;
using namespace Poco::Net;

DefaultRequestHandlerFactory::DefaultRequestHandlerFactory(const Poco::Util::AbstractConfiguration& config)
    : config_(config)
{
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

    return 0;
}
