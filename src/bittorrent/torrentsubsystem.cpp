#include <Hadouken/BitTorrent/TorrentSubsystem.hpp>

#include <libtorrent/extensions/lt_trackers.hpp>
#include <libtorrent/extensions/smart_ban.hpp>
#include <libtorrent/extensions/ut_metadata.hpp>
#include <libtorrent/extensions/ut_pex.hpp>
#include <libtorrent/session.hpp>

using namespace Hadouken::BitTorrent;
using namespace Poco::Util;

libtorrent::session& TorrentSubsystem::getSession()
{
    return *session_;
}

void TorrentSubsystem::initialize(Application& app)
{
    libtorrent::fingerprint fingerprint("LT", LIBTORRENT_VERSION_MAJOR, LIBTORRENT_VERSION_MINOR, 0, 0);
    session_ = new libtorrent::session(fingerprint, 0);

    // alert mask
    // TODO move to JS
    session_->set_alert_mask(libtorrent::alert::category_t::all_categories);

    // load default extensions
    session_->add_extension(&libtorrent::create_lt_trackers_plugin);
    session_->add_extension(&libtorrent::create_smart_ban_plugin);
    session_->add_extension(&libtorrent::create_ut_metadata_plugin);
    session_->add_extension(&libtorrent::create_ut_pex_plugin);

    // start reading alerts
}

void TorrentSubsystem::uninitialize()
{
    // unload

    delete session_;
}

const char* TorrentSubsystem::name() const
{
    return "Torrent";
}
