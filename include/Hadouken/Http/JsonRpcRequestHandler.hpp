#ifndef HADOUKEN_HTTP_JSONRPCREQUESTHANDLER_HPP
#define HADOUKEN_HTTP_JSONRPCREQUESTHANDLER_HPP

#include <Hadouken/Http/JsonRpc/RpcMethod.hpp>
#include <Poco/Net/HTTPRequestHandler.h>

using namespace Poco::Net;

namespace Hadouken
{
    namespace Http
    {
        class JsonRpcRequestHandler : public HTTPRequestHandler
        {
        public:
            JsonRpcRequestHandler(std::map<std::string, Hadouken::Http::JsonRpc::RpcMethod*>& methods);

            void handleRequest(HTTPServerRequest& request, HTTPServerResponse& response);

        private:
            std::map<std::string, Hadouken::Http::JsonRpc::RpcMethod*> methods_;
        };
    }
}

#endif
