#ifndef HADOUKEN_HTTP_DEFAULTREQUESTHANDLERFACTORY_HPP
#define HADOUKEN_HTTP_DEFAULTREQUESTHANDLERFACTORY_HPP

#include <map>
#include <string>

#include <Hadouken/Http/JsonRpc/RpcMethod.hpp>
#include <Poco/Net/HTTPRequestHandlerFactory.h>

namespace Hadouken
{
    namespace Http
    {
        class WebSocketConnectionManager;

        class DefaultRequestHandlerFactory : public Poco::Net::HTTPRequestHandlerFactory
        {
        public:
            DefaultRequestHandlerFactory();
            ~DefaultRequestHandlerFactory();

            Poco::Net::HTTPRequestHandler* createRequestHandler(const Poco::Net::HTTPServerRequest& request);

        private:
            std::map<std::string, Hadouken::Http::JsonRpc::RpcMethod*> methods_;
            WebSocketConnectionManager* wsConnectionManager_;
        };
    }
}

#endif