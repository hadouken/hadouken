#include <hadouken/scripting/modules/bittorrent/torrent_status_wrapper.hpp>

#include <hadouken/scripting/modules/bittorrent/torrent_handle_wrapper.hpp>
#include <libtorrent/torrent_handle.hpp>
#include "../common.hpp"
#include "../../duktape.h"

using namespace hadouken::scripting::modules;
using namespace hadouken::scripting::modules::bittorrent;

void torrent_status_wrapper::initialize(duk_context* ctx, const libtorrent::torrent_status& status)
{
    duk_idx_t statusIndex = duk_push_object(ctx);
    common::set_pointer<libtorrent::torrent_status>(ctx, statusIndex, new libtorrent::torrent_status(status));

    DUK_READONLY_PROPERTY(ctx, statusIndex, torrent, get_handle);
    DUK_READONLY_PROPERTY(ctx, statusIndex, autoManaged, get_auto_managed);
    DUK_READONLY_PROPERTY(ctx, statusIndex, error, get_error);
    DUK_READONLY_PROPERTY(ctx, statusIndex, name, get_name);
    DUK_READONLY_PROPERTY(ctx, statusIndex, eta, get_eta);
    DUK_READONLY_PROPERTY(ctx, statusIndex, addedTime, get_added_time);
    DUK_READONLY_PROPERTY(ctx, statusIndex, completedTime, get_completed_time);
    DUK_READONLY_PROPERTY(ctx, statusIndex, currentTracker, get_current_tracker);
    DUK_READONLY_PROPERTY(ctx, statusIndex, progress, get_progress);
    DUK_READONLY_PROPERTY(ctx, statusIndex, savePath, get_save_path);
    DUK_READONLY_PROPERTY(ctx, statusIndex, downloadRate, get_download_rate);
    DUK_READONLY_PROPERTY(ctx, statusIndex, uploadRate, get_upload_rate);
    DUK_READONLY_PROPERTY(ctx, statusIndex, downloadedBytes, get_downloaded_bytes);
    DUK_READONLY_PROPERTY(ctx, statusIndex, downloadedBytesTotal, get_downloaded_bytes_total);
    DUK_READONLY_PROPERTY(ctx, statusIndex, failedBytes, get_failed_bytes);
    DUK_READONLY_PROPERTY(ctx, statusIndex, redundantBytes, get_redundant_bytes);
    DUK_READONLY_PROPERTY(ctx, statusIndex, uploadedBytes, get_uploaded_bytes);
    DUK_READONLY_PROPERTY(ctx, statusIndex, uploadedBytesTotal, get_uploaded_bytes_total);
    DUK_READONLY_PROPERTY(ctx, statusIndex, numPeers, get_num_peers);
    DUK_READONLY_PROPERTY(ctx, statusIndex, numSeeds, get_num_seeds);
    DUK_READONLY_PROPERTY(ctx, statusIndex, numComplete, get_num_complete);
    DUK_READONLY_PROPERTY(ctx, statusIndex, numIncomplete, get_num_incomplete);
    DUK_READONLY_PROPERTY(ctx, statusIndex, ratio, get_ratio);
    DUK_READONLY_PROPERTY(ctx, statusIndex, state, get_state);
    DUK_READONLY_PROPERTY(ctx, statusIndex, isFinished, is_finished);
    DUK_READONLY_PROPERTY(ctx, statusIndex, isMovingStorage, is_moving_storage);
    DUK_READONLY_PROPERTY(ctx, statusIndex, isPaused, is_paused);
    DUK_READONLY_PROPERTY(ctx, statusIndex, isSeeding, is_seeding);
    DUK_READONLY_PROPERTY(ctx, statusIndex, isSequentialDownload, is_sequential_download);
    DUK_READONLY_PROPERTY(ctx, statusIndex, hasMetadata, has_metadata);
    DUK_READONLY_PROPERTY(ctx, statusIndex, needSaveResume, need_save_resume);
    DUK_READONLY_PROPERTY(ctx, statusIndex, listSeeds, get_list_seeds);
    DUK_READONLY_PROPERTY(ctx, statusIndex, listPeers, get_list_peers);
    DUK_READONLY_PROPERTY(ctx, statusIndex, distributedCopies, get_distributed_copies);
    DUK_READONLY_PROPERTY(ctx, statusIndex, wantedBytes, get_total_wanted);
    DUK_READONLY_PROPERTY(ctx, statusIndex, seedingTime, get_seeding_time);

    duk_push_c_function(ctx, finalize, 1);
    duk_set_finalizer(ctx, -2);
}

duk_ret_t torrent_status_wrapper::finalize(duk_context* ctx)
{
    common::finalize<libtorrent::torrent_status>(ctx);
    return 0;
}

duk_ret_t torrent_status_wrapper::get_handle(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    torrent_handle_wrapper::initialize(ctx, status->handle);
    return 1;
}

duk_ret_t torrent_status_wrapper::get_auto_managed(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    duk_push_boolean(ctx, status->auto_managed ? 1 : 0);
    return 1;
}

duk_ret_t torrent_status_wrapper::get_error(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    duk_push_string(ctx, status->error.c_str());
    return 1;
}

duk_ret_t torrent_status_wrapper::get_name(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    duk_push_string(ctx, status->name.c_str());
    return 1;
}

duk_ret_t torrent_status_wrapper::get_eta(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    int64_t remaining = status->total_wanted - status->total_wanted_done;

    float eta = -1;
    
    if (remaining > 0 && status->download_rate > 0)
    {
        eta = remaining / status->download_rate;
    }
    
    duk_push_number(ctx, eta);
    return 1;
}
duk_ret_t torrent_status_wrapper::get_added_time(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    duk_push_number(ctx, status->added_time);
    return 1;
}

duk_ret_t torrent_status_wrapper::get_completed_time(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    duk_push_number(ctx, status->completed_time);
    return 1;
}

duk_ret_t torrent_status_wrapper::get_current_tracker(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    duk_push_string(ctx, status->current_tracker.c_str());
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

duk_ret_t torrent_status_wrapper::get_failed_bytes(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    duk_push_number(ctx, static_cast<duk_double_t>(status->total_failed_bytes));
    return 1;
}

duk_ret_t torrent_status_wrapper::get_redundant_bytes(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    duk_push_number(ctx, static_cast<duk_double_t>(status->total_redundant_bytes));
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

duk_ret_t torrent_status_wrapper::get_num_complete(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    duk_push_int(ctx, status->num_complete);
    return 1;
}

duk_ret_t torrent_status_wrapper::get_num_incomplete(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    duk_push_int(ctx, status->num_incomplete);
    return 1;
}

duk_ret_t torrent_status_wrapper::get_ratio(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    
    int64_t dl = status->total_done;
    int64_t ul = status->all_time_upload;
    float ratio = -1;
    
    if (dl > 0)
    {
        ratio = (float)ul / (float)dl;
    }

    duk_push_number(ctx, ratio);
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

duk_ret_t torrent_status_wrapper::get_list_peers(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    duk_push_int(ctx, status->list_peers);
    return 1;
}

duk_ret_t torrent_status_wrapper::get_list_seeds(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    duk_push_int(ctx, status->list_seeds);
    return 1;
}

duk_ret_t torrent_status_wrapper::get_distributed_copies(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    duk_push_number(ctx, status->distributed_copies);
    return 1;
}

duk_ret_t torrent_status_wrapper::get_total_wanted(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    duk_push_number(ctx, status->total_wanted);
    return 1;
}

duk_ret_t torrent_status_wrapper::get_seeding_time(duk_context* ctx)
{
    libtorrent::torrent_status* status = common::get_pointer<libtorrent::torrent_status>(ctx);
    duk_push_int(ctx, status->seeding_time);
    return 1;
}
