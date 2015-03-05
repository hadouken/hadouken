#ifndef HADOUKEN_BITTORRENT_PEERINFO_HPP
#define HADOUKEN_BITTORRENT_PEERINFO_HPP

#include <libtorrent/peer_info.hpp>
#include <Poco/Net/SocketAddress.h>
#include <string>

namespace Hadouken
{
    namespace BitTorrent
    {
        struct PeerInfo
        {
            enum ConnectionType
            {
                StandardBitTorrent = 0,
                WebSeed = 1,
                HttpSeed = 2
            };

            PeerInfo(const libtorrent::peer_info& info);

            std::string getClient() const;

            ConnectionType getConnectionType() const;

            std::string getCountry() const;

            int getDownSpeed() const;

            Poco::Net::SocketAddress getRemoteAddress() const;

            float getProgress() const;

            uint64_t getDownloadedBytes() const;

            uint64_t getUploadedBytes() const;

            int getUpSpeed() const;

        private:
            const libtorrent::peer_info info_;
        };
    }
}

#endif
