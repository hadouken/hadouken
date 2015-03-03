#include <Hadouken/Http/WebSocketRequestHandler.hpp>

#include <Hadouken/Http/WebSocketConnectionManager.hpp>
#include <Poco/Net/HTTPServerResponse.h>
#include <Poco/Net/HTTPServerRequest.h>
#include <Poco/Net/NetException.h>
#include <Poco/Net/WebSocket.h>
#include <Poco/Util/Application.h>

using namespace Hadouken::Http;
using namespace Poco::Net;
using namespace Poco::Util;

WebSocketRequestHandler::WebSocketRequestHandler(WebSocketConnectionManager& connectionManager)
    : connectionManager_(connectionManager)
{
}

void WebSocketRequestHandler::handleRequest(HTTPServerRequest& request, HTTPServerResponse& response)
{
    Application& app = Application::instance();

    try
    {
        WebSocket ws(request, response);
        app.logger().information("WebSocket connection established.");

        connectionManager_.connect(ws);

        char buffer[1024];
        int flags;
        int n;

        do
        {
            n = ws.receiveFrame(buffer, sizeof(buffer), flags);
        }
        while (n > 0 || (flags & WebSocket::FRAME_OP_BITMASK) != WebSocket::FRAME_OP_CLOSE);

        connectionManager_.disconnect(ws);

        app.logger().information("WebSocket connection closed.");
    }
    catch (WebSocketException& wse)
    {
        app.logger().log(wse);

        switch (wse.code())
        {
        case WebSocket::WS_ERR_HANDSHAKE_UNSUPPORTED_VERSION:
            response.set("Sec-WebSocket-Version", WebSocket::WEBSOCKET_VERSION);
            // fallthrough
        case WebSocket::WS_ERR_NO_HANDSHAKE:
        case WebSocket::WS_ERR_HANDSHAKE_NO_VERSION:
        case WebSocket::WS_ERR_HANDSHAKE_NO_KEY:
            response.setStatusAndReason(HTTPResponse::HTTP_BAD_REQUEST);
            response.setContentLength(0);
            response.send();
            break;
        }
    }
}
