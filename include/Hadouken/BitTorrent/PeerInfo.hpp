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
            PeerInfo(const libtorrent::peer_info& info);

            std::string getClient() const;

            int getDownSpeed() const;

            Poco::Net::SocketAddress getRemoteAddress() const;

            int getPayloadDownSpeed() const;

            int getPayloadUpSpeed() const;

            int getUpSpeed() const;

        private:
            const libtorrent::peer_info info_;
        };
    }
}

#endif
