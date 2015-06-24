#include <hadouken/scripting/modules/bittorrent/torrent_status_wrapper.hpp>

#include <libtorrent/torrent_handle.hpp>
#include "../common.hpp"
#include "../../duktape.h"

using namespace hadouken::scripting::modules;
using namespace hadouken::scripting::modules::bittorrent;

void torrent_status_wrapper::initialize(duk_context* ctx, const libtorrent::torrent_status& status)
{
    duk_idx_t statusIndex = duk_push_object(ctx);
    common::set_pointer<libtorrent::torrent_status>(ctx, statusIndex, new libtorrent::torrent_status(status));

    DUK_READONLY_PROPERTY(ctx, statusIndex, name, get_name);
    DUK_READONLY_PROPERTY(ctx, statusIndex, progress, get_progress);
    DUK_READONLY_PROPERTY(ctx, statusIndex, savePath, get_save_path);
    DUK_READONLY_PROPERTY(ctx, statusIndex, downloadRate, get_download_rate);
    DUK_READONLY_PROPERTY(ctx, statusIndex, uploadRate, get_upload_rate);
    DUK_READONLY_PROPERTY(ctx, statusIndex, downloadedBytes, get_downloaded_bytes);
    DUK_READONLY_PROPERTY(ctx, statusIndex, downloadedBytesTotal, get_downloaded_bytes_total);
    DUK_READONLY_PROPERTY(ctx, statusIndex, uploadedBytes, get_uploaded_bytes);
    DUK_READONLY_PROPERTY(ctx, statusIndex, uploadedBytesTotal, get_uploaded_bytes_total);
    DUK_READONLY_PROPERTY(ctx, statusIndex, numPeers, get_num_peers);
    DUK_READONLY_PROPERTY(ctx, statusIndex, numSeeds, get_num_seeds);
    DUK_READONLY_PROPERTY(ctx, statusIndex, state, get_state);
    DUK_READONLY_PROPERTY(ctx, statusIndex, isFinished, is_finished);
    DUK_READONLY_PROPERTY(ctx, statusIndex, isMovingStorage, is_moving_storage);
    DUK_READONLY_PROPERTY(ctx, statusIndex, isPaused, is_paused);
    DUK_READONLY_PROPERTY(ctx, statusIndex, isSeeding, is_seeding);
    DUK_READONLY_PROPERTY(ctx, statusIndex, isSequentialDownload, is_sequential_download);
    DUK_READONLY_PROPERTY(ctx, statusIndex, hasMetadata, has_metadata);
    DUK_READONLY_PROPERTY(ctx, statusIndex, needSaveResume, need_save_resume);

    duk_push_c_function(ctx, finalize, 1);
    duk_set_finalizer(ctx, -2);
}

duk_ret_t torrent_status_wrapper::finalize(duk_context* ctx)
{
    common::finalize<libtorrent::torrent_status>(ctx);
    return 0;
}

duk_ret_t torrent_status_wrapper::get_name(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    duk_push_string(ctx, status->name.c_str());
    return 1;
}

duk_ret_t torrent_status_wrapper::get_progress(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    duk_push_number(ctx, status->progress);
    return 1;
}

duk_ret_t torrent_status_wrapper::get_save_path(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    duk_push_string(ctx, status->save_path.c_str());
    return 1;
}

duk_ret_t torrent_status_wrapper::get_download_rate(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    duk_push_int(ctx, status->download_rate);
    return 1;
}

duk_ret_t torrent_status_wrapper::get_upload_rate(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    duk_push_int(ctx, status->upload_rate);
    return 1;
}

duk_ret_t torrent_status_wrapper::get_downloaded_bytes(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    duk_push_number(ctx, static_cast<duk_double_t>(status->total_download));
    return 1;
}

duk_ret_t torrent_status_wrapper::get_downloaded_bytes_total(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    duk_push_number(ctx, static_cast<duk_double_t>(status->all_time_download));
    return 1;
}

duk_ret_t torrent_status_wrapper::get_uploaded_bytes(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    duk_push_number(ctx, static_cast<duk_double_t>(status->total_upload));
    return 1;
}

duk_ret_t torrent_status_wrapper::get_uploaded_bytes_total(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    duk_push_number(ctx, static_cast<duk_double_t>(status->all_time_upload));
    return 1;
}

duk_ret_t torrent_status_wrapper::get_num_peers(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    duk_push_int(ctx, status->num_peers);
    return 1;
}

duk_ret_t torrent_status_wrapper::get_num_seeds(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    duk_push_int(ctx, status->num_seeds);
    return 1;
}

duk_ret_t torrent_status_wrapper::get_state(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    duk_push_int(ctx, status->state);
    return 1;
}

duk_ret_t torrent_status_wrapper::has_metadata(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    duk_push_boolean(ctx, status->has_metadata);
    return 1;
}

duk_ret_t torrent_status_wrapper::is_finished(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    duk_push_boolean(ctx, status->is_finished);
    return 1;
}

duk_ret_t torrent_status_wrapper::is_moving_storage(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    duk_push_boolean(ctx, status->moving_storage);
    return 1;
}

duk_ret_t torrent_status_wrapper::is_paused(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    duk_push_boolean(ctx, status->paused);
    return 1;
}

duk_ret_t torrent_status_wrapper::is_seeding(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    duk_push_boolean(ctx, status->is_seeding);
    return 1;
}

duk_ret_t torrent_status_wrapper::is_sequential_download(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    duk_push_boolean(ctx, status->sequential_download);
    return 1;
}

duk_ret_t torrent_status_wrapper::need_save_resume(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    duk_push_boolean(ctx, status->need_save_resume);
    return 1;
}
