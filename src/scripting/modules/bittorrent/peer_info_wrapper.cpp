#include <hadouken/scripting/modules/bittorrent/peer_info_wrapper.hpp>

#include <libtorrent/peer_info.hpp>

#include "../common.hpp"
#include "../../duktape.h"

using namespace hadouken::scripting::modules;
using namespace hadouken::scripting::modules::bittorrent;

void peer_info_wrapper::initialize(duk_context* ctx, libtorrent::peer_info& peer)
{
    duk_function_list_entry functions[] =
    {
        { NULL, NULL, 0 }
    };

    duk_idx_t peerIndex = duk_push_object(ctx);
    duk_put_function_list(ctx, peerIndex, functions);

    common::set_pointer<libtorrent::peer_info>(ctx, peerIndex, new libtorrent::peer_info(peer));

    DUK_READONLY_PROPERTY(ctx, peerIndex, id, get_peer_id);
    DUK_READONLY_PROPERTY(ctx, peerIndex, flags, get_flags);
    DUK_READONLY_PROPERTY(ctx, peerIndex, country, get_country);
    DUK_READONLY_PROPERTY(ctx, peerIndex, ip, get_ip);
    DUK_READONLY_PROPERTY(ctx, peerIndex, port, get_port);
    DUK_READONLY_PROPERTY(ctx, peerIndex, connectionType, get_connection_type);
    DUK_READONLY_PROPERTY(ctx, peerIndex, client, get_client);
    DUK_READONLY_PROPERTY(ctx, peerIndex, progress, get_progress);
    DUK_READONLY_PROPERTY(ctx, peerIndex, downloadRate, get_download_rate);
    DUK_READONLY_PROPERTY(ctx, peerIndex, downloadRateRemote, get_download_rate_remote);
    DUK_READONLY_PROPERTY(ctx, peerIndex, uploadRate, get_upload_rate);
    DUK_READONLY_PROPERTY(ctx, peerIndex, downloadedBytes, get_downloaded_bytes);
    DUK_READONLY_PROPERTY(ctx, peerIndex, uploadedBytes, get_uploaded_bytes);
    DUK_READONLY_PROPERTY(ctx, peerIndex, lastActive, get_last_active);
    DUK_READONLY_PROPERTY(ctx, peerIndex, lastRequest, get_last_request);
    DUK_READONLY_PROPERTY(ctx, peerIndex, downloadQueueLength, get_download_queue_length);
    DUK_READONLY_PROPERTY(ctx, peerIndex, uploadQueueLength, get_upload_queue_length);
    DUK_READONLY_PROPERTY(ctx, peerIndex, numHashFails, get_num_hashfails);
    DUK_READONLY_PROPERTY(ctx, peerIndex, source, get_source);

    duk_push_c_function(ctx, peer_info_wrapper::finalize, 1);
    duk_set_finalizer(ctx, -2);
}

duk_ret_t peer_info_wrapper::finalize(duk_context* ctx)
{
    common::finalize<libtorrent::peer_info>(ctx);
    return 0;
}

duk_ret_t peer_info_wrapper::get_peer_id(duk_context* ctx)
{
    libtorrent::peer_info* info = common::get_pointer<libtorrent::peer_info>(ctx);
    duk_push_string(ctx, info->pid.to_string().c_str());
    return 1;
}

duk_ret_t peer_info_wrapper::get_flags(duk_context* ctx)
{
    libtorrent::peer_info* info = common::get_pointer<libtorrent::peer_info>(ctx);
    duk_push_number(ctx, info->flags);
    return 1;
}

duk_ret_t peer_info_wrapper::get_country(duk_context* ctx)
{
    libtorrent::peer_info* info = common::get_pointer<libtorrent::peer_info>(ctx);
    duk_push_string(ctx, std::string(info->country, 2).c_str());
    return 1;
}

duk_ret_t peer_info_wrapper::get_ip(duk_context* ctx)
{
    libtorrent::peer_info* info = common::get_pointer<libtorrent::peer_info>(ctx);
    duk_push_string(ctx, info->ip.address().to_string().c_str());
    return 1;
}

duk_ret_t peer_info_wrapper::get_port(duk_context* ctx)
{
    libtorrent::peer_info* info = common::get_pointer<libtorrent::peer_info>(ctx);
    duk_push_int(ctx, info->ip.port());
    return 1;
}

duk_ret_t peer_info_wrapper::get_connection_type(duk_context* ctx)
{
    libtorrent::peer_info* info = common::get_pointer<libtorrent::peer_info>(ctx);
    duk_push_int(ctx, info->connection_type);
    return 1;
}

duk_ret_t peer_info_wrapper::get_client(duk_context* ctx)
{
    libtorrent::peer_info* info = common::get_pointer<libtorrent::peer_info>(ctx);
    duk_push_string(ctx, info->client.c_str());
    return 1;
}

duk_ret_t peer_info_wrapper::get_progress(duk_context* ctx)
{
    libtorrent::peer_info* info = common::get_pointer<libtorrent::peer_info>(ctx);
    duk_push_number(ctx, info->progress);
    return 1;
}

duk_ret_t peer_info_wrapper::get_download_rate(duk_context* ctx)
{
    libtorrent::peer_info* info = common::get_pointer<libtorrent::peer_info>(ctx);
    duk_push_int(ctx, info->down_speed);
    return 1;
}

duk_ret_t peer_info_wrapper::get_download_rate_remote(duk_context* ctx)
{
    libtorrent::peer_info* info = common::get_pointer<libtorrent::peer_info>(ctx);
    duk_push_number(ctx, info->remote_dl_rate);
    return 1;
}

duk_ret_t peer_info_wrapper::get_upload_rate(duk_context* ctx)
{
    libtorrent::peer_info* info = common::get_pointer<libtorrent::peer_info>(ctx);
    duk_push_int(ctx, info->up_speed);
    return 1;
}

duk_ret_t peer_info_wrapper::get_downloaded_bytes(duk_context* ctx)
{
    libtorrent::peer_info* info = common::get_pointer<libtorrent::peer_info>(ctx);
    duk_push_number(ctx, static_cast<duk_double_t>(info->total_download));
    return 1;
}

duk_ret_t peer_info_wrapper::get_uploaded_bytes(duk_context* ctx)
{
    libtorrent::peer_info* info = common::get_pointer<libtorrent::peer_info>(ctx);
    duk_push_number(ctx, static_cast<duk_double_t>(info->total_upload));
    return 1;
}

duk_ret_t peer_info_wrapper::get_last_active(duk_context* ctx)
{
    libtorrent::peer_info* info = common::get_pointer<libtorrent::peer_info>(ctx);
    duk_push_number(ctx, libtorrent::total_seconds(info->last_active));
    return 1;
}

duk_ret_t peer_info_wrapper::get_last_request(duk_context* ctx)
{
    libtorrent::peer_info* info = common::get_pointer<libtorrent::peer_info>(ctx);
    duk_push_number(ctx, libtorrent::total_seconds(info->last_request));
    return 1;
}

duk_ret_t peer_info_wrapper::get_download_queue_length(duk_context* ctx)
{
    libtorrent::peer_info* info = common::get_pointer<libtorrent::peer_info>(ctx);
    duk_push_int(ctx, info->download_queue_length);
    return 1;
}

duk_ret_t peer_info_wrapper::get_upload_queue_length(duk_context* ctx)
{
    libtorrent::peer_info* info = common::get_pointer<libtorrent::peer_info>(ctx);
    duk_push_int(ctx, info->upload_queue_length);
    return 1;
}

duk_ret_t peer_info_wrapper::get_num_hashfails(duk_context* ctx)
{
    libtorrent::peer_info* info = common::get_pointer<libtorrent::peer_info>(ctx);
    duk_push_int(ctx, info->num_hashfails);
    return 1;
}

duk_ret_t peer_info_wrapper::get_source(duk_context* ctx)
{
    libtorrent::peer_info* info = common::get_pointer<libtorrent::peer_info>(ctx);
    duk_push_int(ctx, info->source);
    return 1;
}
