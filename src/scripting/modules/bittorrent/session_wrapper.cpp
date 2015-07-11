#include <hadouken/scripting/modules/bittorrent/session_wrapper.hpp>

#include <hadouken/scripting/modules/bittorrent/alert_wrapper.hpp>
#include <hadouken/scripting/modules/bittorrent/entry_wrapper.hpp>
#include <hadouken/scripting/modules/bittorrent/feed_handle_wrapper.hpp>
#include <hadouken/scripting/modules/bittorrent/session_settings_wrapper.hpp>
#include <hadouken/scripting/modules/bittorrent/torrent_handle_wrapper.hpp>
#include <libtorrent/alert_types.hpp>
#include <libtorrent/rss.hpp>
#include <libtorrent/session.hpp>

#include "../common.hpp"
#include "../../duktape.h"

using namespace hadouken::scripting::modules;
using namespace hadouken::scripting::modules::bittorrent;

void session_wrapper::initialize(duk_context* ctx, libtorrent::session& session)
{
    // Create session property
    duk_idx_t sessionIndex = duk_push_object(ctx);

    common::set_pointer<libtorrent::session>(ctx, sessionIndex, &session);

    duk_push_string(ctx, LIBTORRENT_VERSION);
    duk_put_prop_string(ctx, sessionIndex, "LIBTORRENT_VERSION");

    DUK_READONLY_PROPERTY(ctx, sessionIndex, isListening, is_listening);
    DUK_READONLY_PROPERTY(ctx, sessionIndex, isPaused, is_paused);
    DUK_READONLY_PROPERTY(ctx, sessionIndex, listenPort, get_listen_port);
    DUK_READONLY_PROPERTY(ctx, sessionIndex, sslListenPort, get_ssl_listen_port);

    // Session functions
    duk_function_list_entry functions[] =
    {
        { "addDhtRouter",   add_dht_router, 2 },
        { "addFeed",        add_feed,       1 },
        { "addTorrent",     add_torrent,    2 },
        { "findTorrent",    find_torrent,   1 },
        { "getAlerts",      get_alerts,     0 },
        { "getFeeds",       get_feeds,      0 },
        { "getSettings",    get_settings,   0 },
        { "getStatus",      get_status,     0 },
        { "getTorrents",    get_torrents,   0 },
        { "listenOn",       listen_on,      1 },
        { "loadCountryDb",  load_country_db,1 },
        { "loadState",      load_state,     1 },
        { "pause",          pause,          0 },
        { "postTorrentUpdates", post_torrent_updates, 0 },
        { "removeTorrent",  remove_torrent, 2 },
        { "resume",         resume,         0 },
        { "saveState",      save_state,     0 },
        { "startDht",       start_dht,      0 },
        { "startNatPmp",    start_nat_pmp,  0 },
        { "startUpnp",      start_upnp,     0 },
        { "waitForAlert",   wait_for_alert, 1 },
        { NULL, NULL, 0 }
    };

    duk_put_function_list(ctx, sessionIndex, functions);
}

duk_ret_t session_wrapper::add_dht_router(duk_context* ctx)
{
    std::string url(duk_require_string(ctx, 0));
    int port = duk_require_int(ctx, 1);

    libtorrent::session* sess = common::get_pointer<libtorrent::session>(ctx);
    sess->add_dht_router(std::make_pair(url, port));
    return 0;
}

duk_ret_t session_wrapper::add_feed(duk_context* ctx)
{
    libtorrent::session* sess = common::get_pointer<libtorrent::session>(ctx);
    libtorrent::feed_settings* feed = common::get_pointer<libtorrent::feed_settings>(ctx, 0);

    libtorrent::feed_handle handle = sess->add_feed(*feed);

    // Push feed handle? No

    return 0;
}

duk_ret_t session_wrapper::add_torrent(duk_context* ctx)
{
    libtorrent::session* sess = common::get_pointer<libtorrent::session>(ctx);
    libtorrent::add_torrent_params* p = common::get_pointer<libtorrent::add_torrent_params>(ctx, 0);

    sess->async_add_torrent(*p);

    if (p->ti)
    {
        duk_push_string(ctx, libtorrent::to_hex(p->ti->info_hash().to_string()).c_str());
    }
    else
    {
        duk_push_undefined(ctx);
    }

    return 1;
}

duk_ret_t session_wrapper::find_torrent(duk_context* ctx)
{
    std::string ih(duk_require_string(ctx, 0));
    libtorrent::session* sess = common::get_pointer<libtorrent::session>(ctx);

    libtorrent::sha1_hash hash;
    libtorrent::from_hex(ih.c_str(), std::min(ih.size(), (size_t)libtorrent::sha1_hash::size * 2), (char*)&hash[0]);

    libtorrent::torrent_handle handle = sess->find_torrent(hash);
    torrent_handle_wrapper::initialize(ctx, handle);

    return 1;
}

duk_ret_t session_wrapper::get_listen_port(duk_context* ctx)
{
    libtorrent::session* sess = common::get_pointer<libtorrent::session>(ctx);
    duk_push_int(ctx, sess->listen_port());
    return 1;
}

duk_ret_t session_wrapper::get_ssl_listen_port(duk_context* ctx)
{
    libtorrent::session* sess = common::get_pointer<libtorrent::session>(ctx);
    duk_push_int(ctx, sess->ssl_listen_port());
    return 1;
}

duk_ret_t session_wrapper::get_alerts(duk_context* ctx)
{
    libtorrent::session* sess = common::get_pointer<libtorrent::session>(ctx);

    std::deque<libtorrent::alert*> alerts;
    sess->pop_alerts(&alerts);

    duk_idx_t arrIdx = duk_push_array(ctx);
    int i = 0;

    for (auto &alert : alerts)
    {
        alert_wrapper::construct(ctx, alert);
        duk_put_prop_index(ctx, arrIdx, i);

        ++i;
    }

    return 1;
}

duk_ret_t session_wrapper::get_feeds(duk_context* ctx)
{
    libtorrent::session* sess = common::get_pointer<libtorrent::session>(ctx);

    std::vector<libtorrent::feed_handle> feed_handles;
    sess->get_feeds(feed_handles);

    duk_idx_t arrIdx = duk_push_array(ctx);
    int i = 0;

    for (libtorrent::feed_handle handle : feed_handles)
    {
        feed_handle_wrapper::initialize(ctx, handle);
        duk_put_prop_index(ctx, arrIdx, i);

        ++i;
    }

    return 1;
}

duk_ret_t session_wrapper::get_settings(duk_context* ctx)
{
    libtorrent::session* sess = common::get_pointer<libtorrent::session>(ctx);
    
    libtorrent::session_settings settings = sess->settings();
    session_settings_wrapper::initialize(ctx, settings);
    
    return 1;
}

#define DUK_SESSIONSTATUS_PROP(type, method, name) \
    duk_push_##type(ctx, method); \
    duk_put_prop_string(ctx, statusIndex, name);

duk_ret_t session_wrapper::get_status(duk_context* ctx)
{
    libtorrent::session* sess = common::get_pointer<libtorrent::session>(ctx);
    libtorrent::session_status status = sess->status();

    duk_idx_t statusIndex = duk_push_object(ctx);

    DUK_SESSIONSTATUS_PROP(boolean, status.has_incoming_connections, "hasIncomingConnections");
    DUK_SESSIONSTATUS_PROP(int, status.upload_rate, "totalUploadRate");
    DUK_SESSIONSTATUS_PROP(int, status.download_rate, "totalDownloadRate");
    DUK_SESSIONSTATUS_PROP(number, static_cast<duk_double_t>(status.total_download), "totalDownloadedBytes");
    DUK_SESSIONSTATUS_PROP(number, static_cast<duk_double_t>(status.total_upload), "totalUploadedBytes");
    DUK_SESSIONSTATUS_PROP(int, status.payload_download_rate, "payloadDownloadRate");
    DUK_SESSIONSTATUS_PROP(int, status.payload_upload_rate, "payloadUploadRate");
    DUK_SESSIONSTATUS_PROP(number, static_cast<duk_double_t>(status.total_payload_download), "payloadDownloadedBytes");
    DUK_SESSIONSTATUS_PROP(number, static_cast<duk_double_t>(status.total_payload_upload), "payloadUploadedBytes");
    DUK_SESSIONSTATUS_PROP(int, status.ip_overhead_download_rate, "ipOverheadDownloadRate");
    DUK_SESSIONSTATUS_PROP(int, status.ip_overhead_upload_rate, "ipOverheadUploadRate");
    DUK_SESSIONSTATUS_PROP(number, static_cast<duk_double_t>(status.total_ip_overhead_download), "ipOverheadDownloadedBytes");
    DUK_SESSIONSTATUS_PROP(number, static_cast<duk_double_t>(status.total_ip_overhead_upload), "ipOverheadUploadedBytes");
    DUK_SESSIONSTATUS_PROP(int, status.dht_download_rate, "dhtDownloadRate");
    DUK_SESSIONSTATUS_PROP(int, status.dht_upload_rate, "dhtUploadRate");
    DUK_SESSIONSTATUS_PROP(number, static_cast<duk_double_t>(status.total_dht_download), "dhtDownloadedBytes");
    DUK_SESSIONSTATUS_PROP(number, static_cast<duk_double_t>(status.total_dht_upload), "dhtUploadedBytes");
    DUK_SESSIONSTATUS_PROP(int, status.tracker_download_rate, "trackerDownloadRate");
    DUK_SESSIONSTATUS_PROP(int, status.tracker_upload_rate, "trackerUploadRate");
    DUK_SESSIONSTATUS_PROP(number, static_cast<duk_double_t>(status.total_tracker_download), "trackerDownloadedBytes");
    DUK_SESSIONSTATUS_PROP(number, static_cast<duk_double_t>(status.total_tracker_upload), "trackerUploadedBytes");
    DUK_SESSIONSTATUS_PROP(number, static_cast<duk_double_t>(status.total_failed_bytes), "totalFailedBytes");
    DUK_SESSIONSTATUS_PROP(number, static_cast<duk_double_t>(status.total_redundant_bytes), "totalRedundantBytes");
    DUK_SESSIONSTATUS_PROP(int, status.num_peers, "numPeers");
    DUK_SESSIONSTATUS_PROP(int, status.num_unchoked, "numUnchoked");
    DUK_SESSIONSTATUS_PROP(int, status.allowed_upload_slots, "allowedUploadSlots");
    DUK_SESSIONSTATUS_PROP(int, status.down_bandwidth_queue, "downBandwidthQueue");
    DUK_SESSIONSTATUS_PROP(int, status.up_bandwidth_queue, "upBandwidthQueue");
    DUK_SESSIONSTATUS_PROP(int, status.down_bandwidth_bytes_queue, "downBandwidthBytesQueue");
    DUK_SESSIONSTATUS_PROP(int, status.up_bandwidth_bytes_queue, "upBandwidthBytesQueue");
    DUK_SESSIONSTATUS_PROP(int, status.optimistic_unchoke_counter, "optimisticUnchokeCounter");
    DUK_SESSIONSTATUS_PROP(int, status.unchoke_counter, "unchokeCounter");
    DUK_SESSIONSTATUS_PROP(int, status.disk_read_queue, "diskReadQueue");
    DUK_SESSIONSTATUS_PROP(int, status.disk_write_queue, "diskWriteQueue");
    DUK_SESSIONSTATUS_PROP(int, status.dht_nodes, "dhtNodes");
    DUK_SESSIONSTATUS_PROP(int, status.dht_node_cache, "dhtNodeCache");
    DUK_SESSIONSTATUS_PROP(int, status.dht_torrents, "dhtTorrents");
    DUK_SESSIONSTATUS_PROP(number, static_cast<duk_double_t>(status.dht_global_nodes), "dhtGlobalNodes");
    DUK_SESSIONSTATUS_PROP(int, status.dht_total_allocations, "dhtTotalAllocations");

    return 1;
}

duk_ret_t session_wrapper::get_torrents(duk_context* ctx)
{
    libtorrent::session* sess = common::get_pointer<libtorrent::session>(ctx);

    int arrayIndex = duk_push_array(ctx);
    int i = 0;

    for (libtorrent::torrent_handle handle : sess->get_torrents())
    {
        torrent_handle_wrapper::initialize(ctx, handle);
        duk_put_prop_index(ctx, arrayIndex, i);

        ++i;
    }

    return 1;
}

duk_ret_t session_wrapper::is_listening(duk_context* ctx)
{
    libtorrent::session* sess = common::get_pointer<libtorrent::session>(ctx);
    duk_push_boolean(ctx, sess->is_listening());
    return 1;
}

duk_ret_t session_wrapper::is_paused(duk_context* ctx)
{
    libtorrent::session* sess = common::get_pointer<libtorrent::session>(ctx);
    duk_push_boolean(ctx, sess->is_paused());
    return 1;
}

duk_ret_t session_wrapper::listen_on(duk_context* ctx)
{
    libtorrent::session* sess = common::get_pointer<libtorrent::session>(ctx);

    duk_get_prop_index(ctx, 0, 0);
    int min = duk_to_int(ctx, -1);
    duk_pop(ctx);

    duk_get_prop_index(ctx, 0, 1);
    int max = duk_to_int(ctx, -1);
    duk_pop(ctx);

    libtorrent::error_code ec;
    sess->listen_on(std::make_pair(min, max), ec);

    // TODO error checking

    return 0;
}

duk_ret_t session_wrapper::load_country_db(duk_context* ctx)
{
    common::get_pointer<libtorrent::session>(ctx)->load_country_db(duk_require_string(ctx, 0));
    return 0;
}

duk_ret_t session_wrapper::load_state(duk_context* ctx)
{
    libtorrent::lazy_entry* entry = common::get_pointer<libtorrent::lazy_entry>(ctx, 0);
    common::get_pointer<libtorrent::session>(ctx)->load_state(*entry);
    return 0;
}

duk_ret_t session_wrapper::pause(duk_context* ctx)
{
    common::get_pointer<libtorrent::session>(ctx)->pause();
    return 0;
}

duk_ret_t session_wrapper::post_torrent_updates(duk_context* ctx)
{
    common::get_pointer<libtorrent::session>(ctx)->post_torrent_updates();
    return 0;
}

duk_ret_t session_wrapper::remove_torrent(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = common::get_pointer<libtorrent::torrent_handle>(ctx, 0);
    duk_bool_t remove_data = duk_require_boolean(ctx, 1);

    libtorrent::session* sess = common::get_pointer<libtorrent::session>(ctx);
    sess->remove_torrent(*handle, remove_data ? libtorrent::session::options_t::delete_files : 0);

    return 0;
}

duk_ret_t session_wrapper::resume(duk_context* ctx)
{
    common::get_pointer<libtorrent::session>(ctx)->resume();
    return 0;
}

duk_ret_t session_wrapper::save_state(duk_context* ctx)
{
    libtorrent::session* sess = common::get_pointer<libtorrent::session>(ctx);

    libtorrent::entry entry;
    sess->save_state(entry);

    entry_wrapper::initialize(ctx, entry);
    return 1;
}

duk_ret_t session_wrapper::start_dht(duk_context* ctx)
{
    common::get_pointer<libtorrent::session>(ctx)->start_dht();
    return 0;
}

duk_ret_t session_wrapper::start_nat_pmp(duk_context* ctx)
{
    common::get_pointer<libtorrent::session>(ctx)->start_natpmp();
    return 0;
}

duk_ret_t session_wrapper::start_upnp(duk_context* ctx)
{
    common::get_pointer<libtorrent::session>(ctx)->start_upnp();
    return 0;
}

duk_ret_t session_wrapper::wait_for_alert(duk_context* ctx)
{
    uint64_t duration = duk_require_number(ctx, 0);

    libtorrent::session* sess = common::get_pointer<libtorrent::session>(ctx);
    libtorrent::alert const* alert = sess->wait_for_alert(libtorrent::milliseconds(duration));

    duk_push_boolean(ctx, alert != 0);
    return 1;
}
