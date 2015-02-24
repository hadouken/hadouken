#ifndef HADOUKEN_HTTP_WEBSOCKETREQUESTHANDLER_HPP
#define HADOUKEN_HTTP_WEBSOCKETREQUESTHANDLER_HPP

#include <Poco/Net/HTTPRequestHandler.h>

using namespace Poco::Net;

namespace Hadouken
{
    namespace Http
    {
        class WebSocketRequestHandler : public HTTPRequestHandler
        {
        public:
            void handleRequest(HTTPServerRequest& request, HTTPServerResponse& response);
        };
    }
}

#endif
