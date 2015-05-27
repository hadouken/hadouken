#include <Hadouken/Scripting/Modules/BitTorrent/SessionWrapper.hpp>

#include <Hadouken/Scripting/Modules/BitTorrent/AlertWrapper.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/EntryWrapper.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/SessionSettingsWrapper.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/TorrentHandleWrapper.hpp>
#include <libtorrent/alert_types.hpp>
#include <libtorrent/session.hpp>

#include "../common.hpp"
#include "../../duktape.h"

using namespace Hadouken::Scripting::Modules;
using namespace Hadouken::Scripting::Modules::BitTorrent;

void SessionWrapper::initialize(duk_context* ctx, libtorrent::session& session)
{
    // Create session property
    duk_idx_t sessionIndex = duk_push_object(ctx);

    Common::setPointer<libtorrent::session>(ctx, sessionIndex, &session);

    duk_push_string(ctx, LIBTORRENT_VERSION);
    duk_put_prop_string(ctx, sessionIndex, "LIBTORRENT_VERSION");

    DUK_READONLY_PROPERTY(ctx, sessionIndex, isListening, isListening);
    DUK_READONLY_PROPERTY(ctx, sessionIndex, isPaused, isPaused);
    DUK_READONLY_PROPERTY(ctx, sessionIndex, listenPort, getListenPort);
    DUK_READONLY_PROPERTY(ctx, sessionIndex, sslListenPort, getSslListenPort);

    // Session functions
    duk_function_list_entry functions[] =
    {
        { "addDhtRouter",   addDhtRouter,   2 },
        { "addTorrent",     addTorrent,     2 },
        { "findTorrent",    findTorrent,    1 },
        { "getAlerts",      getAlerts,      0 },
        { "getSettings",    getSettings,    0 },
        { "getStatus",      getStatus,      0 },
        { "getTorrents",    getTorrents,    0 },
        { "listenOn",       listenOn,       1 },
        { "loadState",      loadState,      1 },
        { "pause",          pause,          0 },
        { "removeTorrent",  removeTorrent,  2 },
        { "resume",         resume,         0 },
        { "saveState",      saveState,      0 },
        { "startDht",       startDht,       0 },
        { "waitForAlert",   waitForAlert,   1 },
        { NULL, NULL, 0 }
    };

    duk_put_function_list(ctx, sessionIndex, functions);
}

duk_ret_t SessionWrapper::addDhtRouter(duk_context* ctx)
{
    std::string url(duk_require_string(ctx, 0));
    int port = duk_require_int(ctx, 1);

    libtorrent::session* sess = Common::getPointer<libtorrent::session>(ctx);
    sess->add_dht_router(std::make_pair(url, port));
    return 0;
}

duk_ret_t SessionWrapper::addTorrent(duk_context* ctx)
{
    libtorrent::session* sess = Common::getPointer<libtorrent::session>(ctx);
    libtorrent::add_torrent_params* p = Common::getPointer<libtorrent::add_torrent_params>(ctx, 0);

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

duk_ret_t SessionWrapper::findTorrent(duk_context* ctx)
{
    std::string infoHash(duk_require_string(ctx, 0));
    libtorrent::session* sess = Common::getPointer<libtorrent::session>(ctx);

    libtorrent::sha1_hash hash;
    libtorrent::from_hex(infoHash.c_str(), infoHash.size(), (char*)&hash[0]);

    libtorrent::torrent_handle handle = sess->find_torrent(hash);
    BitTorrent::TorrentHandleWrapper::initialize(ctx, handle);

    return 1;
}

duk_ret_t SessionWrapper::getListenPort(duk_context* ctx)
{
    libtorrent::session* sess = Common::getPointer<libtorrent::session>(ctx);
    duk_push_int(ctx, sess->listen_port());
    return 1;
}

duk_ret_t SessionWrapper::getSslListenPort(duk_context* ctx)
{
    libtorrent::session* sess = Common::getPointer<libtorrent::session>(ctx);
    duk_push_int(ctx, sess->ssl_listen_port());
    return 1;
}

duk_ret_t SessionWrapper::getAlerts(duk_context* ctx)
{
    libtorrent::session* sess = Common::getPointer<libtorrent::session>(ctx);

    std::deque<libtorrent::alert*> alerts;
    sess->pop_alerts(&alerts);

    duk_idx_t arrIdx = duk_push_array(ctx);
    int i = 0;

    for (auto &alert : alerts)
    {
        AlertWrapper::construct(ctx, alert);
        duk_put_prop_index(ctx, arrIdx, i);

        ++i;
    }

    return 1;
}

duk_ret_t SessionWrapper::getSettings(duk_context* ctx)
{
    libtorrent::session* sess = Common::getPointer<libtorrent::session>(ctx);
    SessionSettingsWrapper::initialize(ctx, sess->settings());
    return 1;
}

#define DUK_SESSIONSTATUS_PROP(type, method, name) \
    duk_push_##type(ctx, method); \
    duk_put_prop_string(ctx, statusIndex, name);

duk_ret_t SessionWrapper::getStatus(duk_context* ctx)
{
    libtorrent::session* sess = Common::getPointer<libtorrent::session>(ctx);
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

duk_ret_t SessionWrapper::getTorrents(duk_context* ctx)
{
    libtorrent::session* sess = Common::getPointer<libtorrent::session>(ctx);

    int arrayIndex = duk_push_array(ctx);
    int i = 0;

    for (libtorrent::torrent_handle handle : sess->get_torrents())
    {
        BitTorrent::TorrentHandleWrapper::initialize(ctx, handle);
        duk_put_prop_index(ctx, arrayIndex, i);

        ++i;
    }

    return 1;
}

duk_ret_t SessionWrapper::isListening(duk_context* ctx)
{
    libtorrent::session* sess = Common::getPointer<libtorrent::session>(ctx);
    duk_push_boolean(ctx, sess->is_listening());
    return 1;
}

duk_ret_t SessionWrapper::isPaused(duk_context* ctx)
{
    libtorrent::session* sess = Common::getPointer<libtorrent::session>(ctx);
    duk_push_boolean(ctx, sess->is_paused());
    return 1;
}

duk_ret_t SessionWrapper::listenOn(duk_context* ctx)
{
    libtorrent::session* sess = Common::getPointer<libtorrent::session>(ctx);

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

duk_ret_t SessionWrapper::loadState(duk_context* ctx)
{
    libtorrent::lazy_entry* entry = Common::getPointer<libtorrent::lazy_entry>(ctx, 0);
    Common::getPointer<libtorrent::session>(ctx)->load_state(*entry);
    return 0;
}

duk_ret_t SessionWrapper::pause(duk_context* ctx)
{
    Common::getPointer<libtorrent::session>(ctx)->pause();
    return 0;
}

duk_ret_t SessionWrapper::removeTorrent(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = Common::getPointer<libtorrent::torrent_handle>(ctx, 0);
    duk_bool_t removeData = duk_require_boolean(ctx, 1);

    libtorrent::session* sess = Common::getPointer<libtorrent::session>(ctx);
    sess->remove_torrent(*handle, removeData ? libtorrent::session::options_t::delete_files : 0);

    return 0;
}

duk_ret_t SessionWrapper::resume(duk_context* ctx)
{
    Common::getPointer<libtorrent::session>(ctx)->resume();
    return 0;
}

duk_ret_t SessionWrapper::saveState(duk_context* ctx)
{
    libtorrent::session* sess = Common::getPointer<libtorrent::session>(ctx);

    libtorrent::entry entry;
    sess->save_state(entry);

    EntryWrapper::initialize(ctx, entry);
    return 1;
}

duk_ret_t SessionWrapper::startDht(duk_context* ctx)
{
    Common::getPointer<libtorrent::session>(ctx)->start_dht();
    return 0;
}

duk_ret_t SessionWrapper::waitForAlert(duk_context* ctx)
{
    uint64_t duration = duk_require_number(ctx, 0);

    libtorrent::session* sess = Common::getPointer<libtorrent::session>(ctx);
    libtorrent::alert const* alert = sess->wait_for_alert(libtorrent::milliseconds(duration));

    duk_push_boolean(ctx, alert != 0);
    return 1;
}
