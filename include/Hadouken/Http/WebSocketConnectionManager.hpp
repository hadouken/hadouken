#ifndef HADOUKEN_HTTP_WEBSOCKETCONNECTIONMANAGER_HPP
#define HADOUKEN_HTTP_WEBSOCKETCONNECTIONMANAGER_HPP

#include <Poco/Mutex.h>
#include <Poco/Net/WebSocket.h>

namespace Hadouken
{
    namespace BitTorrent
    {
        struct TorrentHandle;
    }

    namespace Http
    {
        class WebSocketConnectionManager
        {
        public:
            WebSocketConnectionManager();
            ~WebSocketConnectionManager();
            
            void connect(Poco::Net::WebSocket& webSocket);

            void disconnect(Poco::Net::WebSocket& webSocket);

        protected:
            void onTorrentAdded(const void* sender, Hadouken::BitTorrent::TorrentHandle& handle);
            void onTorrentRemoved(const void* sender, std::string& infoHash);

            void sendMessage(std::string message);

        private:
            Poco::Mutex socketsMutex_;
            std::vector<Poco::Net::WebSocket> sockets_;
        };
    }
}

#endif
