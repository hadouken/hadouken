#include <Hadouken/BitTorrent/TorrentInfo.hpp>

#include <Hadouken/BitTorrent/FileStorage.hpp>
#include <libtorrent/torrent_info.hpp>
#include <string>

using namespace Hadouken::BitTorrent;

TorrentInfo::TorrentInfo(const libtorrent::torrent_info& info)
    : info_(info)
{
}

const std::string& TorrentInfo::getComment() const
{
    return info_.comment();
}

FileStorage const TorrentInfo::getFiles() const
{
    return FileStorage(info_.files());
}

const std::string& TorrentInfo::getName() const
{
    return info_.name();
}

int TorrentInfo::getNumFiles() const
{
    return info_.num_files();
}

int TorrentInfo::getNumPieces() const
{
    return info_.num_pieces();
}
