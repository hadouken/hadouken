#include <hadouken/scripting/modules/bittorrent/alert_wrapper.hpp>

#include <hadouken/scripting/modules/bittorrent/add_torrent_params_wrapper.hpp>
#include <hadouken/scripting/modules/bittorrent/entry_wrapper.hpp>
#include <hadouken/scripting/modules/bittorrent/error_code_wrapper.hpp>
#include <hadouken/scripting/modules/bittorrent/torrent_handle_wrapper.hpp>
#include <hadouken/scripting/modules/bittorrent/torrent_status_wrapper.hpp>
#include <libtorrent/alert_types.hpp>

#include "../common.hpp"
#include "../../duktape.h"

using namespace hadouken::scripting::modules;
using namespace hadouken::scripting::modules::bittorrent;

#define ALERT_CASE(type) \
    case type::alert_type: \
        alert_wrapper::initialize(ctx, alert_cast<type>(alert)); \
        break;

void alert_wrapper::construct(duk_context* ctx, libtorrent::alert* alert)
{
    using namespace libtorrent;

    switch (alert->type())
    {
        ALERT_CASE(torrent_added_alert)
        ALERT_CASE(torrent_removed_alert)
        ALERT_CASE(read_piece_alert)
        ALERT_CASE(file_completed_alert)
        ALERT_CASE(file_renamed_alert)
        ALERT_CASE(file_rename_failed_alert)
        ALERT_CASE(performance_alert)
        ALERT_CASE(state_changed_alert)
        ALERT_CASE(tracker_error_alert)
        ALERT_CASE(tracker_warning_alert)
        ALERT_CASE(scrape_reply_alert)
        ALERT_CASE(scrape_failed_alert)
        ALERT_CASE(tracker_reply_alert)
        ALERT_CASE(dht_reply_alert)
        ALERT_CASE(tracker_announce_alert)
        ALERT_CASE(hash_failed_alert)
        ALERT_CASE(peer_ban_alert)
        ALERT_CASE(peer_snubbed_alert)
        ALERT_CASE(peer_unsnubbed_alert)
        ALERT_CASE(peer_error_alert)
        ALERT_CASE(peer_connect_alert)
        ALERT_CASE(peer_disconnected_alert)
        ALERT_CASE(invalid_request_alert)
        ALERT_CASE(torrent_finished_alert)
        ALERT_CASE(piece_finished_alert)
        ALERT_CASE(request_dropped_alert)
        ALERT_CASE(block_timeout_alert)
        ALERT_CASE(block_finished_alert)
        ALERT_CASE(block_downloading_alert)
        ALERT_CASE(unwanted_block_alert)
        ALERT_CASE(storage_moved_alert)
        ALERT_CASE(storage_moved_failed_alert)
        ALERT_CASE(torrent_deleted_alert)
        ALERT_CASE(torrent_delete_failed_alert)
        ALERT_CASE(save_resume_data_alert)
        ALERT_CASE(save_resume_data_failed_alert)
        ALERT_CASE(torrent_paused_alert)
        ALERT_CASE(torrent_resumed_alert)
        ALERT_CASE(torrent_checked_alert)
        ALERT_CASE(url_seed_alert)
        ALERT_CASE(file_error_alert)
        ALERT_CASE(metadata_failed_alert)
        ALERT_CASE(metadata_received_alert)
        ALERT_CASE(udp_error_alert)
        ALERT_CASE(external_ip_alert)
        ALERT_CASE(listen_failed_alert)
        ALERT_CASE(listen_succeeded_alert)
        ALERT_CASE(portmap_error_alert)
        ALERT_CASE(portmap_alert)
        ALERT_CASE(portmap_log_alert)
        ALERT_CASE(fastresume_rejected_alert)
        ALERT_CASE(peer_blocked_alert)
        //ALERT_CASE(dht_announce_alert)
        ALERT_CASE(dht_get_peers_alert)
        ALERT_CASE(stats_alert)
        ALERT_CASE(cache_flushed_alert)
        //ALERT_CASE(anonymous_mode_alert)
        ALERT_CASE(lsd_peer_alert)
        ALERT_CASE(trackerid_alert)
        ALERT_CASE(dht_bootstrap_alert)
//      ALERT_CASE(rss_alert)
        ALERT_CASE(torrent_error_alert)
        ALERT_CASE(torrent_need_cert_alert)
        ALERT_CASE(add_torrent_alert)
        ALERT_CASE(state_update_alert)
        ALERT_CASE(torrent_update_alert)
        //ALERT_CASE(rss_item_alert)
        //ALERT_CASE(dht_error_alert)
        //ALERT_CASE(dht_immutable_item_alert)
        //ALERT_CASE(dht_mutable_item_alert)
        //ALERT_CASE(dht_put_alert)
        ALERT_CASE(i2p_alert)

    default:
        duk_push_null(ctx);
        break;
    }
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::alert* alert)
{
    duk_idx_t idx = duk_push_object(ctx);

    // Internal pointer. We take ownership of the one passed to us
    common::set_pointer<libtorrent::alert>(ctx, idx, alert);

    duk_push_string(ctx, alert->message().c_str());
    duk_put_prop_string(ctx, idx, "_message");

    duk_push_string(ctx, alert->what());
    duk_put_prop_string(ctx, idx, "name");

    duk_push_number(ctx, alert->type());
    duk_put_prop_string(ctx, idx, "type");

    // Finalizer
    duk_push_c_function(ctx, finalize, 1);
    duk_set_finalizer(ctx, -2);

    return idx;
}

duk_ret_t alert_wrapper::finalize(duk_context* ctx)
{
    common::finalize<libtorrent::alert>(ctx);
    return 0;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::torrent_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::alert*>(alert));

    torrent_handle_wrapper::initialize(ctx, alert->handle);
    duk_put_prop_string(ctx, idx, "torrent");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::peer_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::torrent_alert*>(alert));

    duk_idx_t ipIdx = duk_push_object(ctx);
    duk_push_string(ctx, alert->ip.address().to_string().c_str());
    duk_put_prop_string(ctx, ipIdx, "address");
    duk_push_int(ctx, alert->ip.port());
    duk_put_prop_string(ctx, ipIdx, "port");

    duk_put_prop_string(ctx, idx, "ip");

    duk_push_string(ctx, libtorrent::to_hex(alert->pid.to_string()).c_str());
    duk_put_prop_string(ctx, idx, "peerId");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::tracker_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::torrent_alert*>(alert));

    duk_push_string(ctx, alert->tracker_url());
    duk_put_prop_string(ctx, idx, "url");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::save_resume_data_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::torrent_alert*>(alert));

    entry_wrapper::initialize(ctx, *alert->resume_data);
    duk_put_prop_string(ctx, idx, "resumeData");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::torrent_removed_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::torrent_alert*>(alert));

    duk_push_string(ctx, libtorrent::to_hex(alert->info_hash.to_string()).c_str());
    duk_put_prop_string(ctx, idx, "infoHash");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::read_piece_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::torrent_alert*>(alert));

    if (alert->ec)
    {
        error_code_wrapper::initialize(ctx, alert->ec);
        duk_put_prop_string(ctx, idx, "error");
    }

    duk_push_int(ctx, alert->piece);
    duk_put_prop_string(ctx, idx, "index");

    duk_push_int(ctx, alert->size);
    duk_put_prop_string(ctx, idx, "size");

    void* buf = duk_push_buffer(ctx, alert->size, false);
    std::strncpy(static_cast<char*>(buf), alert->buffer.get(), alert->size);
    duk_put_prop_string(ctx, idx, "buffer");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::file_completed_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::torrent_alert*>(alert));

    duk_push_int(ctx, alert->index);
    duk_put_prop_string(ctx, idx, "index");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::file_renamed_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::torrent_alert*>(alert));

    duk_push_int(ctx, alert->index);
    duk_put_prop_string(ctx, idx, "index");

    duk_push_string(ctx, alert->new_name());
    duk_put_prop_string(ctx, idx, "name");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::file_rename_failed_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::torrent_alert*>(alert));

    duk_push_int(ctx, alert->index);
    duk_put_prop_string(ctx, idx, "index");

    if (alert->error)
    {
        error_code_wrapper::initialize(ctx, alert->error);
        duk_put_prop_string(ctx, idx, "error");
    }

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::performance_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::torrent_alert*>(alert));

    duk_push_int(ctx, alert->warning_code);
    duk_put_prop_string(ctx, idx, "code");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::state_changed_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::torrent_alert*>(alert));

    duk_push_int(ctx, alert->state);
    duk_put_prop_string(ctx, idx, "state");

    duk_push_int(ctx, alert->prev_state);
    duk_put_prop_string(ctx, idx, "previousState");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::tracker_error_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::tracker_alert*>(alert));

    duk_push_int(ctx, alert->status_code);
    duk_put_prop_string(ctx, idx, "statusCode");

    duk_push_int(ctx, alert->times_in_row);
    duk_put_prop_string(ctx, idx, "times");

    duk_push_string(ctx, alert->error_message());
    duk_put_prop_string(ctx, idx, "message");

    if (alert->error)
    {
        error_code_wrapper::initialize(ctx, alert->error);
        duk_put_prop_string(ctx, idx, "error");
    }

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::tracker_warning_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::tracker_alert*>(alert));

    duk_push_string(ctx, alert->warning_message());
    duk_put_prop_string(ctx, idx, "message");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::scrape_reply_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::tracker_alert*>(alert));

    duk_push_int(ctx, alert->complete);
    duk_put_prop_string(ctx, idx, "complete");

    duk_push_int(ctx, alert->incomplete);
    duk_put_prop_string(ctx, idx, "incomplete");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::scrape_failed_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::tracker_alert*>(alert));

    duk_push_string(ctx, alert->error_message());
    duk_put_prop_string(ctx, idx, "message");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::tracker_reply_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::tracker_alert*>(alert));

    duk_push_int(ctx, alert->num_peers);
    duk_put_prop_string(ctx, idx, "numPeers");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::dht_reply_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::tracker_alert*>(alert));

    duk_push_int(ctx, alert->num_peers);
    duk_put_prop_string(ctx, idx, "numPeers");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::tracker_announce_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::tracker_alert*>(alert));

    duk_push_int(ctx, alert->event);
    duk_put_prop_string(ctx, idx, "event");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::hash_failed_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::torrent_alert*>(alert));

    duk_push_int(ctx, alert->piece_index);
    duk_put_prop_string(ctx, idx, "pieceIndex");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::peer_ban_alert* alert)
{
    return alert_wrapper::initialize(ctx, static_cast<libtorrent::peer_alert*>(alert));
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::peer_snubbed_alert* alert)
{
    return alert_wrapper::initialize(ctx, static_cast<libtorrent::peer_alert*>(alert));
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::peer_unsnubbed_alert* alert)
{
    return alert_wrapper::initialize(ctx, static_cast<libtorrent::peer_alert*>(alert));
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::peer_error_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::peer_alert*>(alert));

    if (alert->error)
    {
        error_code_wrapper::initialize(ctx, alert->error);
        duk_put_prop_string(ctx, idx, "error");
    }

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::peer_connect_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::peer_alert*>(alert));

    duk_push_int(ctx, alert->socket_type);
    duk_put_prop_string(ctx, idx, "socketType");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::peer_disconnected_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::peer_alert*>(alert));

    if (alert->error)
    {
        error_code_wrapper::initialize(ctx, alert->error);
        duk_put_prop_string(ctx, idx, "error");
    }

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::invalid_request_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::peer_alert*>(alert));

    duk_idx_t reqIdx = duk_push_object(ctx);
    duk_push_int(ctx, alert->request.length);
    duk_put_prop_string(ctx, reqIdx, "length");
    duk_push_int(ctx, alert->request.piece);
    duk_put_prop_string(ctx, reqIdx, "index");
    duk_push_int(ctx, alert->request.start);
    duk_put_prop_string(ctx, reqIdx, "start");

    duk_put_prop_string(ctx, idx, "request");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::torrent_finished_alert* alert)
{
    return alert_wrapper::initialize(ctx, static_cast<libtorrent::torrent_alert*>(alert));
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::piece_finished_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::torrent_alert*>(alert));

    duk_push_int(ctx, alert->piece_index);
    duk_put_prop_string(ctx, idx, "pieceIndex");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::request_dropped_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::peer_alert*>(alert));

    duk_push_int(ctx, alert->block_index);
    duk_put_prop_string(ctx, idx, "blockIndex");

    duk_push_int(ctx, alert->piece_index);
    duk_put_prop_string(ctx, idx, "pieceIndex");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::block_timeout_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::peer_alert*>(alert));

    duk_push_int(ctx, alert->block_index);
    duk_put_prop_string(ctx, idx, "blockIndex");

    duk_push_int(ctx, alert->piece_index);
    duk_put_prop_string(ctx, idx, "pieceIndex");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::block_finished_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::peer_alert*>(alert));

    duk_push_int(ctx, alert->block_index);
    duk_put_prop_string(ctx, idx, "blockIndex");

    duk_push_int(ctx, alert->piece_index);
    duk_put_prop_string(ctx, idx, "pieceIndex");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::block_downloading_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::peer_alert*>(alert));

    duk_push_int(ctx, alert->block_index);
    duk_put_prop_string(ctx, idx, "blockIndex");

    duk_push_int(ctx, alert->piece_index);
    duk_put_prop_string(ctx, idx, "pieceIndex");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::unwanted_block_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::peer_alert*>(alert));

    duk_push_int(ctx, alert->block_index);
    duk_put_prop_string(ctx, idx, "blockIndex");

    duk_push_int(ctx, alert->piece_index);
    duk_put_prop_string(ctx, idx, "pieceIndex");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::storage_moved_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::torrent_alert*>(alert));

    duk_push_string(ctx, alert->storage_path());
    duk_put_prop_string(ctx, idx, "path");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::storage_moved_failed_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::torrent_alert*>(alert));

    if (alert->error)
    {
        error_code_wrapper::initialize(ctx, alert->error);
        duk_put_prop_string(ctx, idx, "error");
    }

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::torrent_deleted_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::torrent_alert*>(alert));

    duk_push_string(ctx, libtorrent::to_hex(alert->info_hash.to_string()).c_str());
    duk_put_prop_string(ctx, idx, "infoHash");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::torrent_delete_failed_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::torrent_alert*>(alert));

    if (alert->error)
    {
        error_code_wrapper::initialize(ctx, alert->error);
        duk_put_prop_string(ctx, idx, "error");
    }

    duk_push_string(ctx, libtorrent::to_hex(alert->info_hash.to_string()).c_str());
    duk_put_prop_string(ctx, idx, "infoHash");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::save_resume_data_failed_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::torrent_alert*>(alert));

    if (alert->error)
    {
        error_code_wrapper::initialize(ctx, alert->error);
        duk_put_prop_string(ctx, idx, "error");
    }

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::torrent_paused_alert* alert)
{
    return alert_wrapper::initialize(ctx, static_cast<libtorrent::torrent_alert*>(alert));
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::torrent_resumed_alert* alert)
{
    return alert_wrapper::initialize(ctx, static_cast<libtorrent::torrent_alert*>(alert));
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::torrent_checked_alert* alert)
{
    return alert_wrapper::initialize(ctx, static_cast<libtorrent::torrent_alert*>(alert));
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::url_seed_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::torrent_alert*>(alert));

    duk_push_string(ctx, alert->server_url());
    duk_put_prop_string(ctx, idx, "url");

    duk_push_string(ctx, alert->error_message());
    duk_put_prop_string(ctx, idx, "message");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::file_error_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::torrent_alert*>(alert));
    
    if (alert->error)
    {
        error_code_wrapper::initialize(ctx, alert->error);
        duk_put_prop_string(ctx, idx, "error");
    }

    duk_push_string(ctx, alert->filename());
    duk_put_prop_string(ctx, idx, "file");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::metadata_failed_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::torrent_alert*>(alert));

    if (alert->error)
    {
        error_code_wrapper::initialize(ctx, alert->error);
        duk_put_prop_string(ctx, idx, "error");
    }

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::metadata_received_alert* alert)
{
    return alert_wrapper::initialize(ctx, static_cast<libtorrent::torrent_alert*>(alert));
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::udp_error_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::alert*>(alert));

    duk_idx_t ipIdx = duk_push_object(ctx);
    duk_push_string(ctx, alert->endpoint.address().to_string().c_str());
    duk_put_prop_string(ctx, ipIdx, "address");
    duk_push_int(ctx, alert->endpoint.port());
    duk_put_prop_string(ctx, ipIdx, "port");

    duk_put_prop_string(ctx, idx, "ip");

    if (alert->error)
    {
        error_code_wrapper::initialize(ctx, alert->error);
        duk_put_prop_string(ctx, idx, "error");
    }

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::external_ip_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::alert*>(alert));

    duk_push_string(ctx, alert->external_address.to_string().c_str());
    duk_put_prop_string(ctx, idx, "address");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::listen_failed_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::alert*>(alert));

    duk_push_string(ctx, alert->listen_interface());
    duk_put_prop_string(ctx, idx, "ip");

    if (alert->error)
    {
        error_code_wrapper::initialize(ctx, alert->error);
        duk_put_prop_string(ctx, idx, "error");
    }

    duk_push_int(ctx, alert->operation);
    duk_put_prop_string(ctx, idx, "operation");

    duk_push_int(ctx, alert->sock_type);
    duk_put_prop_string(ctx, idx, "socketType");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::listen_succeeded_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::alert*>(alert));

    duk_idx_t ipIdx = duk_push_object(ctx);
    duk_push_string(ctx, alert->endpoint.address().to_string().c_str());
    duk_put_prop_string(ctx, ipIdx, "address");
    duk_push_int(ctx, alert->endpoint.port());
    duk_put_prop_string(ctx, ipIdx, "port");

    duk_put_prop_string(ctx, idx, "ip");

    duk_push_int(ctx, alert->sock_type);
    duk_put_prop_string(ctx, idx, "socketType");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::portmap_error_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::alert*>(alert));

    if (alert->error)
    {
        error_code_wrapper::initialize(ctx, alert->error);
        duk_put_prop_string(ctx, idx, "error");
    }

    duk_push_int(ctx, alert->mapping);
    duk_put_prop_string(ctx, idx, "mapping");

    duk_push_int(ctx, alert->map_type);
    duk_put_prop_string(ctx, idx, "type");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::portmap_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::alert*>(alert));

    duk_push_int(ctx, alert->mapping);
    duk_put_prop_string(ctx, idx, "mapping");

    duk_push_int(ctx, alert->map_type);
    duk_put_prop_string(ctx, idx, "type");

    duk_push_int(ctx, alert->external_port);
    duk_put_prop_string(ctx, idx, "externalPort");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::portmap_log_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::alert*>(alert));

    duk_push_int(ctx, alert->map_type);
    duk_put_prop_string(ctx, idx, "type");

    duk_push_string(ctx, alert->log_message());
    duk_put_prop_string(ctx, idx, "message");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::fastresume_rejected_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::torrent_alert*>(alert));

    if (alert->error)
    {
        error_code_wrapper::initialize(ctx, alert->error);
        duk_put_prop_string(ctx, idx, "error");
    }

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::peer_blocked_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::torrent_alert*>(alert));

    duk_push_string(ctx, alert->ip.to_string().c_str());
    duk_put_prop_string(ctx, idx, "address");

    duk_push_int(ctx, alert->reason);
    duk_put_prop_string(ctx, idx, "reason");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::dht_get_peers_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::alert*>(alert));

    duk_push_string(ctx, libtorrent::to_hex(alert->info_hash.to_string()).c_str());
    duk_put_prop_string(ctx, idx, "infoHash");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::stats_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::torrent_alert*>(alert));

    duk_push_int(ctx, alert->transferred[libtorrent::stats_alert::stats_channel::download_ip_protocol]);
    duk_put_prop_string(ctx, idx, "downloadIpProtocol");

    duk_push_int(ctx, alert->transferred[libtorrent::stats_alert::stats_channel::download_payload]);
    duk_put_prop_string(ctx, idx, "downloadPayload");

    duk_push_int(ctx, alert->transferred[libtorrent::stats_alert::stats_channel::download_protocol]);
    duk_put_prop_string(ctx, idx, "downloadProtocol");

    duk_push_int(ctx, alert->transferred[libtorrent::stats_alert::stats_channel::upload_ip_protocol]);
    duk_put_prop_string(ctx, idx, "uploadIpProtocol");

    duk_push_int(ctx, alert->transferred[libtorrent::stats_alert::stats_channel::upload_payload]);
    duk_put_prop_string(ctx, idx, "uploadPayload");

    duk_push_int(ctx, alert->transferred[libtorrent::stats_alert::stats_channel::upload_protocol]);
    duk_put_prop_string(ctx, idx, "uploadProtocol");

    duk_push_int(ctx, alert->interval);
    duk_put_prop_string(ctx, idx, "interval");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::cache_flushed_alert* alert)
{
    return alert_wrapper::initialize(ctx, static_cast<libtorrent::torrent_alert*>(alert));
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::lsd_peer_alert* alert)
{
    return alert_wrapper::initialize(ctx, static_cast<libtorrent::peer_alert*>(alert));
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::trackerid_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::torrent_alert*>(alert));

    duk_push_string(ctx, alert->tracker_id());
    duk_put_prop_string(ctx, idx, "trackerId");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::dht_bootstrap_alert* alert)
{
    return alert_wrapper::initialize(ctx, static_cast<libtorrent::alert*>(alert));
}

/*
duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::rss_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::alert*>(alert));

    duk_push_string(ctx, alert->url.c_str());
    duk_put_prop_string(ctx, idx, "url");

    duk_push_int(ctx, alert->state);
    duk_put_prop_string(ctx, idx, "state");

    feed_handle_wrapper::initialize(ctx, alert->handle);
    duk_put_prop_string(ctx, idx, "feed");

    return idx;
}
*/

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::torrent_error_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::torrent_alert*>(alert));

    if (alert->error)
    {
        error_code_wrapper::initialize(ctx, alert->error);
        duk_put_prop_string(ctx, idx, "error");
    }

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::torrent_need_cert_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::torrent_alert*>(alert));

    if (alert->error)
    {
        error_code_wrapper::initialize(ctx, alert->error);
        duk_put_prop_string(ctx, idx, "error");
    }

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::incoming_connection_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::alert*>(alert));

    duk_idx_t ipIdx = duk_push_object(ctx);
    duk_push_string(ctx, alert->ip.address().to_string().c_str());
    duk_put_prop_string(ctx, ipIdx, "address");
    duk_push_int(ctx, alert->ip.port());
    duk_put_prop_string(ctx, ipIdx, "port");

    duk_put_prop_string(ctx, idx, "ip");

    duk_push_int(ctx, alert->socket_type);
    duk_put_prop_string(ctx, idx, "socketType");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::add_torrent_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::torrent_alert*>(alert));

    add_torrent_params_wrapper::initialize(ctx, alert->params);
    duk_put_prop_string(ctx, idx, "params");

    if (alert->error)
    {
        error_code_wrapper::initialize(ctx, alert->error);
        duk_put_prop_string(ctx, idx, "error");
    }

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::state_update_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::alert*>(alert));

    duk_idx_t arrIdx = duk_push_array(ctx);
    int i = 0;

    for (libtorrent::torrent_status status : alert->status)
    {
        torrent_status_wrapper::initialize(ctx, status);
        duk_put_prop_index(ctx, arrIdx, i);

        ++i;
    }

    duk_put_prop_string(ctx, idx, "status");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::torrent_update_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::torrent_alert*>(alert));

    duk_push_string(ctx, libtorrent::to_hex(alert->old_ih.to_string()).c_str());
    duk_put_prop_string(ctx, idx, "oldInfoHash");

    duk_push_string(ctx, libtorrent::to_hex(alert->new_ih.to_string()).c_str());
    duk_put_prop_string(ctx, idx, "infoHash");

    return idx;
}

duk_idx_t alert_wrapper::initialize(duk_context* ctx, libtorrent::i2p_alert* alert)
{
    duk_idx_t idx = alert_wrapper::initialize(ctx, static_cast<libtorrent::alert*>(alert));

    if (alert->error)
    {
        error_code_wrapper::initialize(ctx, alert->error);
        duk_put_prop_string(ctx, idx, "error");
    }

    return idx;
}
