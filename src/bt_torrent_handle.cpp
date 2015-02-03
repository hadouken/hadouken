#include <hadouken/bittorrent/torrent_handle.hpp>

#include <libtorrent/torrent_handle.hpp>

using namespace hadouken::bittorrent;

torrent_handle::torrent_handle(const libtorrent::torrent_handle& handle)
    : handle_(handle)
{
}

void torrent_handle::move_storage(const std::string& save_path, int flags) const
{
    handle_.move_storage(save_path, flags);
}

std::string torrent_handle::name() const
{
    return handle_.status(libtorrent::torrent_handle::query_name).name;
}