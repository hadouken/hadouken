#ifndef HADOUKEN_HTTP_WEBSOCKETREQUESTHANDLER_HPP
#define HADOUKEN_HTTP_WEBSOCKETREQUESTHANDLER_HPP

#include <Poco/Net/HTTPRequestHandler.h>

using namespace Poco::Net;

namespace Hadouken
{
    namespace Http
    {
        class WebSocketConnectionManager;

        class WebSocketRequestHandler : public HTTPRequestHandler
        {
        public:
            WebSocketRequestHandler(WebSocketConnectionManager& connectionManager);
            
            void handleRequest(HTTPServerRequest& request, HTTPServerResponse& response);

        private:
            WebSocketConnectionManager& connectionManager_;
        };
    }
}

#endif
