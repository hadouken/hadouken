#include <Hadouken/BitTorrent/TorrentHandle.hpp>

#include <Hadouken/BitTorrent/AnnounceEntry.hpp>
#include <Hadouken/BitTorrent/PeerInfo.hpp>
#include <Hadouken/BitTorrent/TorrentInfo.hpp>
#include <Hadouken/BitTorrent/TorrentStatus.hpp>
#include <libtorrent/peer_info.hpp>
#include <libtorrent/torrent_handle.hpp>
#include <string>

using namespace Hadouken::BitTorrent;

TorrentHandle::TorrentHandle(const libtorrent::torrent_handle& handle)
    : handle_(handle)
{
}

TorrentHandle::TorrentHandle(const TorrentHandle& handle)
    : handle_(handle.handle_)
{
}

void TorrentHandle::clearError() const
{
    handle_.clear_error();
}

void TorrentHandle::forceRecheck() const
{
    handle_.force_recheck();
}

std::vector<int64_t> TorrentHandle::getFileProgress() const
{
    std::vector<libtorrent::size_type> p;
    handle_.file_progress(p);

    return p;
}

std::string TorrentHandle::getInfoHash() const
{
    return libtorrent::to_hex(handle_.info_hash().to_string());
}

int TorrentHandle::getMaxConnections() const
{
    return handle_.max_connections();
}

int TorrentHandle::getMaxUploads() const
{
    return handle_.max_uploads();
}

std::vector<PeerInfo> TorrentHandle::getPeers() const
{
    std::vector<libtorrent::peer_info> p;
    handle_.get_peer_info(p);

    std::vector<PeerInfo> result;

    for (libtorrent::peer_info inf : p)
    {
        result.push_back(PeerInfo(inf));
    }

    return result;
}

int TorrentHandle::getQueuePosition() const
{
    return handle_.queue_position();
}

bool TorrentHandle::getResolveCountries() const
{
    return handle_.resolve_countries();
}

TorrentStatus TorrentHandle::getStatus() const
{
    libtorrent::torrent_status status = handle_.status();
    return TorrentStatus(status);
}

std::unique_ptr<TorrentInfo> TorrentHandle::getTorrentFile() const
{
    if (handle_.torrent_file())
    {
        boost::intrusive_ptr<libtorrent::torrent_info const> info = handle_.torrent_file();
        return std::unique_ptr<TorrentInfo>(new TorrentInfo(*info));
    }

    return std::unique_ptr<TorrentInfo>();
}

std::vector<AnnounceEntry> TorrentHandle::getTrackers() const
{
    std::vector<AnnounceEntry> result;

    for (libtorrent::announce_entry entry : handle_.trackers())
    {
        result.push_back(AnnounceEntry(entry));
    }

    return result;
}

int TorrentHandle::getUploadLimit() const
{
    return handle_.upload_limit();
}

bool TorrentHandle::getUploadMode() const
{
    return handle_.status().upload_mode;
}

bool TorrentHandle::isValid() const
{
    return handle_.is_valid();
}

void TorrentHandle::moveStorage(const std::string& savePath) const
{
    handle_.move_storage(savePath);
}

void TorrentHandle::pause() const
{
    if (handle_.status(0x0).paused) {
        return;
    }

    handle_.auto_managed(false);
    handle_.pause();
}

void TorrentHandle::renameFile(int index, std::string const& name) const
{
    handle_.rename_file(index, name);
}

void TorrentHandle::queueBottom() const
{
    handle_.queue_position_bottom();
}

void TorrentHandle::queueDown() const
{
    handle_.queue_position_down();
}

void TorrentHandle::queueTop() const
{
    handle_.queue_position_top();
}

void TorrentHandle::queueUp() const
{
    handle_.queue_position_up();
}

void TorrentHandle::resume() const
{
    handle_.auto_managed(true);
    handle_.resume();
}

void TorrentHandle::setMaxConnections(int limit) const
{
    handle_.set_max_connections(limit);
}

void TorrentHandle::setMaxUploads(int limit) const
{
    handle_.set_max_uploads(limit);
}

void TorrentHandle::setResolveCountries(bool value)
{
    handle_.resolve_countries(value);
}

void TorrentHandle::setSequentialDownload(bool value) const
{
    handle_.set_sequential_download(value);
}

void TorrentHandle::setUploadLimit(int limit) const
{
    handle_.set_upload_limit(limit);
}

void TorrentHandle::setUploadMode(bool mode) const
{
    handle_.set_upload_mode(mode);
}
