#include <hadouken/scripting/modules/bittorrent/torrent_handle_wrapper.hpp>

#include <hadouken/scripting/modules/bittorrent/announce_entry_wrapper.hpp>
#include <hadouken/scripting/modules/bittorrent/peer_info_wrapper.hpp>
#include <hadouken/scripting/modules/bittorrent/torrent_info_wrapper.hpp>
#include <hadouken/scripting/modules/bittorrent/torrent_status_wrapper.hpp>
#include <libtorrent/peer_info.hpp>
#include <libtorrent/torrent_handle.hpp>

#include "../common.hpp"
#include "../../duktape.h"

using namespace hadouken::scripting::modules;
using namespace hadouken::scripting::modules::bittorrent;

void torrent_handle_wrapper::initialize(duk_context* ctx, const libtorrent::torrent_handle& handle)
{
    duk_function_list_entry functions[] =
    {
        { "clearError",        clear_error,         0 },
        { "flushCache",        flush_cache,         0 },
        { "forceRecheck",      force_recheck,       0 },
        { "getFilePriorities", get_file_priorities, 0 },
        { "getFileProgress",   get_file_progress,   0 },
        { "getPeers",          get_peers,           0 },
        { "getStatus",         get_status,          0 },
        { "getTorrentInfo",    get_torrent_info,    0 },
        { "getTrackers",       get_trackers,        0 },
        { "havePiece",         have_piece,          1 },
        { "moveStorage",       move_storage,        1 },
        { "pause",             pause,               0 },
        { "queueBottom",       queue_bottom,        0 },
        { "queueDown",         queue_down,          0 },
        { "queueTop",          queue_top,           0 },
        { "queueUp",           queue_up,            0 },
        { "readPiece",         read_piece,          1 },
        { "renameFile",        rename_file,         2 },
        { "resume",            resume,              0 },
        { "saveResumeData",    save_resume_data,    0 },
        { "setPriority",       set_priority,        1 },
        { NULL,                NULL,                0 }
    };

    duk_idx_t idx = duk_push_object(ctx);
    duk_put_function_list(ctx, idx, functions);

    common::set_pointer<libtorrent::torrent_handle>(ctx, idx, new libtorrent::torrent_handle(handle));

    // read-only properties
    DUK_READONLY_PROPERTY(ctx, idx, infoHash, get_info_hash);
    DUK_READONLY_PROPERTY(ctx, idx, isValid, is_valid);
    DUK_READONLY_PROPERTY(ctx, idx, queuePosition, get_queue_position);

    duk_push_c_function(ctx, finalize, 1);
    duk_set_finalizer(ctx, -2);

    // read+write properties
    DUK_READWRITE_PROPERTY(ctx, idx, maxConnections, max_connections);
    DUK_READWRITE_PROPERTY(ctx, idx, maxUploads, max_uploads);
    DUK_READWRITE_PROPERTY(ctx, idx, resolveCountries, resolve_countries);
    DUK_READWRITE_PROPERTY(ctx, idx, sequentialDownload, sequential_download);
    DUK_READWRITE_PROPERTY(ctx, idx, uploadMode, upload_mode);
    DUK_READWRITE_PROPERTY(ctx, idx, uploadLimit, upload_limit);
}

duk_ret_t torrent_handle_wrapper::finalize(duk_context* ctx)
{
    common::finalize<libtorrent::torrent_handle>(ctx);
    return 0;
}

duk_ret_t torrent_handle_wrapper::clear_error(duk_context* ctx)
{
    common::get_pointer<libtorrent::torrent_handle>(ctx)->clear_error();
    return 0;
}

duk_ret_t torrent_handle_wrapper::flush_cache(duk_context* ctx)
{
    common::get_pointer<libtorrent::torrent_handle>(ctx)->flush_cache();
    return 0;
}

duk_ret_t torrent_handle_wrapper::force_recheck(duk_context* ctx)
{
    common::get_pointer<libtorrent::torrent_handle>(ctx)->force_recheck();
    return 0;
}

duk_ret_t torrent_handle_wrapper::get_info_hash(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = common::get_pointer<libtorrent::torrent_handle>(ctx);
    duk_push_string(ctx, libtorrent::to_hex(handle->info_hash().to_string()).c_str());
    return 1;
}

duk_ret_t torrent_handle_wrapper::is_valid(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = common::get_pointer<libtorrent::torrent_handle>(ctx);
    duk_push_boolean(ctx, handle->is_valid());
    return 1;
}

duk_ret_t torrent_handle_wrapper::get_file_priorities(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = common::get_pointer<libtorrent::torrent_handle>(ctx);

    duk_idx_t arrIdx = duk_push_array(ctx);
    int i = 0;

    for (int prio : handle->file_priorities())
    {
        duk_push_int(ctx, prio);
        duk_put_prop_index(ctx, arrIdx, i);

        ++i;
    }

    return 1;
}

duk_ret_t torrent_handle_wrapper::get_file_progress(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = common::get_pointer<libtorrent::torrent_handle>(ctx);
    
    std::vector<libtorrent::size_type> progress;
    handle->file_progress(progress);

    duk_idx_t arrIdx = duk_push_array(ctx);
    int i = 0;

    for (libtorrent::size_type size : progress)
    {
        duk_push_number(ctx, size);
        duk_put_prop_index(ctx, arrIdx, i);

        ++i;
    }

    return 1;
}

duk_ret_t torrent_handle_wrapper::get_peers(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = common::get_pointer<libtorrent::torrent_handle>(ctx);

    int arrayIndex = duk_push_array(ctx);
    int i = 0;

    std::vector<libtorrent::peer_info> peers;
    handle->get_peer_info(peers);

    for (libtorrent::peer_info peer : peers)
    {
        peer_info_wrapper::initialize(ctx, peer);
        duk_put_prop_index(ctx, arrayIndex, i);

        ++i;
    }

    return 1;
}

duk_ret_t torrent_handle_wrapper::get_queue_position(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = common::get_pointer<libtorrent::torrent_handle>(ctx);
    duk_push_int(ctx, handle->queue_position());
    return 1;
}

duk_ret_t torrent_handle_wrapper::get_status(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = common::get_pointer<libtorrent::torrent_handle>(ctx);
    torrent_status_wrapper::initialize(ctx, handle->status());
    return 1;
}

duk_ret_t torrent_handle_wrapper::get_torrent_info(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = common::get_pointer<libtorrent::torrent_handle>(ctx);
    boost::intrusive_ptr<libtorrent::torrent_info const> info = handle->torrent_file();

    if (info)
    {
        torrent_info_wrapper::initialize(ctx, *info);
    }
    else
    {
        duk_push_null(ctx);
    }

    return 1;
}

duk_ret_t torrent_handle_wrapper::get_trackers(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = common::get_pointer<libtorrent::torrent_handle>(ctx);

    int arrayIndex = duk_push_array(ctx);
    int i = 0;

    for (libtorrent::announce_entry entry : handle->trackers())
    {
        announce_entry_wrapper::initialize(ctx, entry);
        duk_put_prop_index(ctx, arrayIndex, i);

        ++i;
    }

    return 1;
}

duk_ret_t torrent_handle_wrapper::have_piece(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = common::get_pointer<libtorrent::torrent_handle>(ctx);
    duk_push_boolean(ctx, handle->have_piece(duk_require_int(ctx, 0)));
    return 1;
}

duk_ret_t torrent_handle_wrapper::move_storage(duk_context* ctx)
{
    std::string path(duk_require_string(ctx, 0));

    libtorrent::torrent_handle* handle = common::get_pointer<libtorrent::torrent_handle>(ctx);
    handle->move_storage(path);

    return 0;
}

duk_ret_t torrent_handle_wrapper::pause(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = common::get_pointer<libtorrent::torrent_handle>(ctx);
    handle->auto_managed(false);
    handle->pause();

    return 0;
}

duk_ret_t torrent_handle_wrapper::queue_bottom(duk_context* ctx)
{
    common::get_pointer<libtorrent::torrent_handle>(ctx)->queue_position_bottom();
    return 0;
}

duk_ret_t torrent_handle_wrapper::queue_down(duk_context* ctx)
{
    common::get_pointer<libtorrent::torrent_handle>(ctx)->queue_position_down();
    return 0;
}

duk_ret_t torrent_handle_wrapper::queue_top(duk_context* ctx)
{
    common::get_pointer<libtorrent::torrent_handle>(ctx)->queue_position_top();
    return 0;
}

duk_ret_t torrent_handle_wrapper::queue_up(duk_context* ctx)
{
    common::get_pointer<libtorrent::torrent_handle>(ctx)->queue_position_up();
    return 0;
}

duk_ret_t torrent_handle_wrapper::read_piece(duk_context* ctx)
{
    common::get_pointer<libtorrent::torrent_handle>(ctx)->read_piece(duk_require_int(ctx, 0));
    return 0;
}

duk_ret_t torrent_handle_wrapper::rename_file(duk_context* ctx)
{
    duk_int_t fileIndex = duk_require_int(ctx, 0);
    const char* name = duk_require_string(ctx, 1);

    common::get_pointer<libtorrent::torrent_handle>(ctx)->rename_file(fileIndex, std::string(name));

    return 0;
}

duk_ret_t torrent_handle_wrapper::resume(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = common::get_pointer<libtorrent::torrent_handle>(ctx);
    handle->auto_managed(true);
    handle->resume();

    return 0;
}

duk_ret_t torrent_handle_wrapper::save_resume_data(duk_context* ctx)
{
    common::get_pointer<libtorrent::torrent_handle>(ctx)->save_resume_data();
    return 0;
}

duk_ret_t torrent_handle_wrapper::set_priority(duk_context* ctx)
{
    common::get_pointer<libtorrent::torrent_handle>(ctx)->set_priority(duk_require_int(ctx, 0));
    return 0;
}

duk_ret_t torrent_handle_wrapper::get_max_connections(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = common::get_pointer<libtorrent::torrent_handle>(ctx);
    duk_push_int(ctx, handle->max_connections());
    return 1;
}

duk_ret_t torrent_handle_wrapper::get_max_uploads(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = common::get_pointer<libtorrent::torrent_handle>(ctx);
    duk_push_int(ctx, handle->max_uploads());
    return 1;
}

duk_ret_t torrent_handle_wrapper::get_resolve_countries(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = common::get_pointer<libtorrent::torrent_handle>(ctx);
    duk_push_boolean(ctx, handle->resolve_countries());
    return 1;
}

duk_ret_t torrent_handle_wrapper::get_sequential_download(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = common::get_pointer<libtorrent::torrent_handle>(ctx);
    duk_push_boolean(ctx, handle->status().sequential_download);
    return 1;
}

duk_ret_t torrent_handle_wrapper::get_upload_limit(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = common::get_pointer<libtorrent::torrent_handle>(ctx);
    duk_push_int(ctx, handle->upload_limit());
    return 1;
}

duk_ret_t torrent_handle_wrapper::get_upload_mode(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = common::get_pointer<libtorrent::torrent_handle>(ctx);
    duk_push_boolean(ctx, handle->status().upload_mode);
    return 1;
}

duk_ret_t torrent_handle_wrapper::set_max_connections(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = common::get_pointer<libtorrent::torrent_handle>(ctx);
    handle->set_max_connections(duk_require_int(ctx, 0));
    return 0;
}

duk_ret_t torrent_handle_wrapper::set_max_uploads(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = common::get_pointer<libtorrent::torrent_handle>(ctx);
    handle->set_max_uploads(duk_require_int(ctx, 0));
    return 0;
}

duk_ret_t torrent_handle_wrapper::set_resolve_countries(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = common::get_pointer<libtorrent::torrent_handle>(ctx);
    handle->resolve_countries(duk_require_boolean(ctx, 0) > 0 ? true : false);
    return 0;
}

duk_ret_t torrent_handle_wrapper::set_sequential_download(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = common::get_pointer<libtorrent::torrent_handle>(ctx);
    handle->set_sequential_download(duk_require_boolean(ctx, 0) > 0 ? true : false);
    return 0;
}

duk_ret_t torrent_handle_wrapper::set_upload_limit(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = common::get_pointer<libtorrent::torrent_handle>(ctx);
    handle->set_upload_limit(duk_require_int(ctx, 0));
    return 0;
}

duk_ret_t torrent_handle_wrapper::set_upload_mode(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = common::get_pointer<libtorrent::torrent_handle>(ctx);
    handle->set_upload_mode(duk_require_boolean(ctx, 0) > 0 ? true : false);
    return 0;
}
