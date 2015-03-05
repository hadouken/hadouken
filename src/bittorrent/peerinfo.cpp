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

PeerInfo::ConnectionType PeerInfo::getConnectionType() const
{
    return (PeerInfo::ConnectionType)(int)info_.connection_type;
}

std::string PeerInfo::getCountry() const
{
    return std::string(info_.country, 2);
}

uint64_t PeerInfo::getDownloadedBytes() const
{
    return info_.total_download;
}

int PeerInfo::getDownSpeed() const
{
    return info_.down_speed;
}

float PeerInfo::getProgress() const
{
    return info_.progress;
}

uint64_t PeerInfo::getUploadedBytes() const
{
    return info_.total_upload;
}

int PeerInfo::getUpSpeed() const
{
    return info_.up_speed;
}

Poco::Net::SocketAddress PeerInfo::getRemoteAddress() const
{
    return Poco::Net::SocketAddress(info_.ip.address().to_string(), info_.ip.port());
}
