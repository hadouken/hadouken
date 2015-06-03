#include <Hadouken/Scripting/Modules/BitTorrent/TorrentStatusWrapper.hpp>

#include <libtorrent/torrent_handle.hpp>
#include "../common.hpp"
#include "../../duktape.h"

using namespace Hadouken::Scripting::Modules;
using namespace Hadouken::Scripting::Modules::BitTorrent;

void TorrentStatusWrapper::initialize(duk_context* ctx, const libtorrent::torrent_status& status)
{
    duk_idx_t statusIndex = duk_push_object(ctx);
    Common::setPointer<libtorrent::torrent_status>(ctx, statusIndex, new libtorrent::torrent_status(status));

    DUK_READONLY_PROPERTY(ctx, statusIndex, name, getName);
    DUK_READONLY_PROPERTY(ctx, statusIndex, progress, getProgress);
    DUK_READONLY_PROPERTY(ctx, statusIndex, savePath, getSavePath);
    DUK_READONLY_PROPERTY(ctx, statusIndex, downloadRate, getDownloadRate);
    DUK_READONLY_PROPERTY(ctx, statusIndex, uploadRate, getUploadRate);
    DUK_READONLY_PROPERTY(ctx, statusIndex, downloadedBytes, getDownloadedBytes);
    DUK_READONLY_PROPERTY(ctx, statusIndex, downloadedBytesTotal, getDownloadedBytesTotal);
    DUK_READONLY_PROPERTY(ctx, statusIndex, uploadedBytes, getUploadedBytes);
    DUK_READONLY_PROPERTY(ctx, statusIndex, uploadedBytesTotal, getUploadedBytesTotal);
    DUK_READONLY_PROPERTY(ctx, statusIndex, numPeers, getNumPeers);
    DUK_READONLY_PROPERTY(ctx, statusIndex, numSeeds, getNumSeeds);
    DUK_READONLY_PROPERTY(ctx, statusIndex, state, getState);
    DUK_READONLY_PROPERTY(ctx, statusIndex, isFinished, isFinished);
    DUK_READONLY_PROPERTY(ctx, statusIndex, isMovingStorage, isMovingStorage);
    DUK_READONLY_PROPERTY(ctx, statusIndex, isPaused, isPaused);
    DUK_READONLY_PROPERTY(ctx, statusIndex, isSeeding, isSeeding);
    DUK_READONLY_PROPERTY(ctx, statusIndex, isSequentialDownload, isSequentialDownload);
    DUK_READONLY_PROPERTY(ctx, statusIndex, hasMetadata, hasMetadata);
    DUK_READONLY_PROPERTY(ctx, statusIndex, needSaveResume, needSaveResume);

    duk_push_c_function(ctx, TorrentStatusWrapper::finalize, 1);
    duk_set_finalizer(ctx, -2);
}

duk_ret_t TorrentStatusWrapper::finalize(duk_context* ctx)
{
    Common::finalize<libtorrent::torrent_status>(ctx);
    return 0;
}

duk_ret_t TorrentStatusWrapper::getName(duk_context* ctx)
{
    libtorrent::torrent_status* status = Common::getPointer<libtorrent::torrent_status>(ctx);
    duk_push_string(ctx, status->name.c_str());
    return 1;
}

duk_ret_t TorrentStatusWrapper::getProgress(duk_context* ctx)
{
    libtorrent::torrent_status* status = Common::getPointer<libtorrent::torrent_status>(ctx);
    duk_push_number(ctx, status->progress);
    return 1;
}

duk_ret_t TorrentStatusWrapper::getSavePath(duk_context* ctx)
{
    libtorrent::torrent_status* status = Common::getPointer<libtorrent::torrent_status>(ctx);
    duk_push_string(ctx, status->save_path.c_str());
    return 1;
}

duk_ret_t TorrentStatusWrapper::getDownloadRate(duk_context* ctx)
{
    libtorrent::torrent_status* status = Common::getPointer<libtorrent::torrent_status>(ctx);
    duk_push_int(ctx, status->download_rate);
    return 1;
}

duk_ret_t TorrentStatusWrapper::getUploadRate(duk_context* ctx)
{
    libtorrent::torrent_status* status = Common::getPointer<libtorrent::torrent_status>(ctx);
    duk_push_int(ctx, status->upload_rate);
    return 1;
}

duk_ret_t TorrentStatusWrapper::getDownloadedBytes(duk_context* ctx)
{
    libtorrent::torrent_status* status = Common::getPointer<libtorrent::torrent_status>(ctx);
    duk_push_number(ctx, static_cast<duk_double_t>(status->total_download));
    return 1;
}

duk_ret_t TorrentStatusWrapper::getDownloadedBytesTotal(duk_context* ctx)
{
    libtorrent::torrent_status* status = Common::getPointer<libtorrent::torrent_status>(ctx);
    duk_push_number(ctx, static_cast<duk_double_t>(status->all_time_download));
    return 1;
}

duk_ret_t TorrentStatusWrapper::getUploadedBytes(duk_context* ctx)
{
    libtorrent::torrent_status* status = Common::getPointer<libtorrent::torrent_status>(ctx);
    duk_push_number(ctx, static_cast<duk_double_t>(status->total_upload));
    return 1;
}

duk_ret_t TorrentStatusWrapper::getUploadedBytesTotal(duk_context* ctx)
{
    libtorrent::torrent_status* status = Common::getPointer<libtorrent::torrent_status>(ctx);
    duk_push_number(ctx, static_cast<duk_double_t>(status->all_time_upload));
    return 1;
}

duk_ret_t TorrentStatusWrapper::getNumPeers(duk_context* ctx)
{
    libtorrent::torrent_status* status = Common::getPointer<libtorrent::torrent_status>(ctx);
    duk_push_int(ctx, status->num_peers);
    return 1;
}

duk_ret_t TorrentStatusWrapper::getNumSeeds(duk_context* ctx)
{
    libtorrent::torrent_status* status = Common::getPointer<libtorrent::torrent_status>(ctx);
    duk_push_int(ctx, status->num_seeds);
    return 1;
}

duk_ret_t TorrentStatusWrapper::getState(duk_context* ctx)
{
    libtorrent::torrent_status* status = Common::getPointer<libtorrent::torrent_status>(ctx);
    duk_push_int(ctx, status->state);
    return 1;
}

duk_ret_t TorrentStatusWrapper::hasMetadata(duk_context* ctx)
{
    libtorrent::torrent_status* status = Common::getPointer<libtorrent::torrent_status>(ctx);
    duk_push_boolean(ctx, status->has_metadata);
    return 1;
}

duk_ret_t TorrentStatusWrapper::isFinished(duk_context* ctx)
{
    libtorrent::torrent_status* status = Common::getPointer<libtorrent::torrent_status>(ctx);
    duk_push_boolean(ctx, status->is_finished);
    return 1;
}

duk_ret_t TorrentStatusWrapper::isMovingStorage(duk_context* ctx)
{
    libtorrent::torrent_status* status = Common::getPointer<libtorrent::torrent_status>(ctx);
    duk_push_boolean(ctx, status->moving_storage);
    return 1;
}

duk_ret_t TorrentStatusWrapper::isPaused(duk_context* ctx)
{
    libtorrent::torrent_status* status = Common::getPointer<libtorrent::torrent_status>(ctx);
    duk_push_boolean(ctx, status->paused);
    return 1;
}

duk_ret_t TorrentStatusWrapper::isSeeding(duk_context* ctx)
{
    libtorrent::torrent_status* status = Common::getPointer<libtorrent::torrent_status>(ctx);
    duk_push_boolean(ctx, status->is_seeding);
    return 1;
}

duk_ret_t TorrentStatusWrapper::isSequentialDownload(duk_context* ctx)
{
    libtorrent::torrent_status* status = Common::getPointer<libtorrent::torrent_status>(ctx);
    duk_push_boolean(ctx, status->sequential_download);
    return 1;
}

duk_ret_t TorrentStatusWrapper::needSaveResume(duk_context* ctx)
{
    libtorrent::torrent_status* status = Common::getPointer<libtorrent::torrent_status>(ctx);
    duk_push_boolean(ctx, status->need_save_resume);
    return 1;
}
