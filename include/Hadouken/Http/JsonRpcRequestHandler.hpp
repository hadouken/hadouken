#ifndef HADOUKEN_HTTP_JSONRPCREQUESTHANDLER_HPP
#define HADOUKEN_HTTP_JSONRPCREQUESTHANDLER_HPP

#include <Hadouken/Http/JsonRpc/RpcMethod.hpp>
#include <Poco/Net/HTTPRequestHandler.h>
#include <Poco/Util/AbstractConfiguration.h>

using namespace Poco::Net;

namespace Hadouken
{
    namespace Http
    {
        class JsonRpcRequestHandler : public HTTPRequestHandler
        {
        public:
            JsonRpcRequestHandler(const Poco::Util::AbstractConfiguration& config, std::map<std::string, Hadouken::Http::JsonRpc::RpcMethod*>& methods);

            void handleRequest(HTTPServerRequest& request, HTTPServerResponse& response);

        protected:
            bool isValidRequest(HTTPServerRequest& request) const;

        private:
            const Poco::Util::AbstractConfiguration& config_;
            std::map<std::string, Hadouken::Http::JsonRpc::RpcMethod*> methods_;
        };
    }
}

#endif
