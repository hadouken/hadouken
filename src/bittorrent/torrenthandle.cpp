#include <Hadouken/BitTorrent/TorrentHandle.hpp>

#include <Hadouken/BitTorrent/TorrentStatus.hpp>
#include <libtorrent/torrent_handle.hpp>
#include <string>

using namespace Hadouken::BitTorrent;

TorrentHandle::TorrentHandle(const libtorrent::torrent_handle& handle)
    : handle_(handle)
{
}

std::string TorrentHandle::getInfoHash() const
{
    return libtorrent::to_hex(handle_.info_hash().to_string());
}

int TorrentHandle::getQueuePosition() const
{
    return handle_.queue_position();
}

TorrentStatus TorrentHandle::getStatus() const
{
    libtorrent::torrent_status status = handle_.status();
    return TorrentStatus(status);
}

bool TorrentHandle::isValid() const
{
    return handle_.is_valid();
}

void TorrentHandle::pause() const
{
    handle_.pause();
}

void TorrentHandle::resume() const
{
    handle_.resume();
}