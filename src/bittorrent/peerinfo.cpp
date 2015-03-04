#include <Hadouken/BitTorrent/PeerInfo.hpp>

#include <libtorrent/peer_info.hpp>

using namespace Hadouken::BitTorrent;

PeerInfo::PeerInfo(const libtorrent::peer_info& info)
    : info_(info)
{
}

std::string PeerInfo::getClient() const
{
    return info_.client;
}

int PeerInfo::getDownSpeed() const
{
    return info_.down_speed;
}

int PeerInfo::getUpSpeed() const
{
    return info_.up_speed;
}

Poco::Net::SocketAddress PeerInfo::getRemoteAddress() const
{
    return Poco::Net::SocketAddress(info_.ip.address().to_string(), info_.ip.port());
}
