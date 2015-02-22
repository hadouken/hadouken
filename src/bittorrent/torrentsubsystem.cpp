#include <Hadouken/BitTorrent/TorrentSubsystem.hpp>

#include <Hadouken/BitTorrent/Session.hpp>

using namespace Hadouken::BitTorrent;
using namespace Poco::Util;

Session& TorrentSubsystem::getSession()
{
    return *sess_;
}

void TorrentSubsystem::initialize(Application& app)
{
    sess_ = new Session(app.config());
    sess_->load();
}

void TorrentSubsystem::uninitialize()
{
    sess_->unload();
    delete sess_;
}

const char* TorrentSubsystem::name() const
{
    return "Torrent";
}
