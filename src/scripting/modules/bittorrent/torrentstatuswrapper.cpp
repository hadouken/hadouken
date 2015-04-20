#include <Hadouken/Scripting/Modules/BitTorrent/TorrentStatusWrapper.hpp>

#include <Hadouken/BitTorrent/TorrentStatus.hpp>

#include "../common.hpp"
#include "../../duktape.h"

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Scripting::Modules;
using namespace Hadouken::Scripting::Modules::BitTorrent;

const char* TorrentStatusWrapper::field = "\xff" "TorrentStatus";

void TorrentStatusWrapper::initialize(duk_context* ctx, TorrentStatus& status)
{
    duk_idx_t statusIndex = duk_push_object(ctx);

    // Internal pointer
    duk_push_pointer(ctx, new TorrentStatus(status));
    duk_put_prop_string(ctx, statusIndex, field);

    DUK_READONLY_PROPERTY(ctx, statusIndex, name, TorrentStatusWrapper::getName);
    DUK_READONLY_PROPERTY(ctx, statusIndex, progress, TorrentStatusWrapper::getProgress);
    DUK_READONLY_PROPERTY(ctx, statusIndex, savePath, TorrentStatusWrapper::getSavePath);
    DUK_READONLY_PROPERTY(ctx, statusIndex, downloadRate, TorrentStatusWrapper::getDownloadRate);
    DUK_READONLY_PROPERTY(ctx, statusIndex, uploadRate, TorrentStatusWrapper::getUploadRate);
    DUK_READONLY_PROPERTY(ctx, statusIndex, downloadedBytes, TorrentStatusWrapper::getDownloadedBytes);
    DUK_READONLY_PROPERTY(ctx, statusIndex, downloadedBytesTotal, TorrentStatusWrapper::getDownloadedBytesTotal);
    DUK_READONLY_PROPERTY(ctx, statusIndex, uploadedBytes, TorrentStatusWrapper::getUploadedBytes);
    DUK_READONLY_PROPERTY(ctx, statusIndex, uploadedBytesTotal, TorrentStatusWrapper::getUploadedBytesTotal);
    DUK_READONLY_PROPERTY(ctx, statusIndex, numPeers, TorrentStatusWrapper::getNumPeers);
    DUK_READONLY_PROPERTY(ctx, statusIndex, numSeeds, TorrentStatusWrapper::getNumSeeds);
    DUK_READONLY_PROPERTY(ctx, statusIndex, state, TorrentStatusWrapper::getState);
    DUK_READONLY_PROPERTY(ctx, statusIndex, isFinished, TorrentStatusWrapper::isFinished);
    DUK_READONLY_PROPERTY(ctx, statusIndex, isMovingStorage, TorrentStatusWrapper::isMovingStorage);
    DUK_READONLY_PROPERTY(ctx, statusIndex, isPaused, TorrentStatusWrapper::isPaused);
    DUK_READONLY_PROPERTY(ctx, statusIndex, isSeeding, TorrentStatusWrapper::isSeeding);
    DUK_READONLY_PROPERTY(ctx, statusIndex, isSequentialDownload, TorrentStatusWrapper::isSequentialDownload);

    duk_push_c_function(ctx, TorrentStatusWrapper::finalize, 1);
    duk_set_finalizer(ctx, -2);
}

duk_ret_t TorrentStatusWrapper::finalize(duk_context* ctx)
{
    if (duk_get_prop_string(ctx, -1, field))
    {
        void* ptr = duk_get_pointer(ctx, -1);
        TorrentStatus* status = static_cast<TorrentStatus*>(ptr);
        delete status;
    }

    return 0;
}

duk_ret_t TorrentStatusWrapper::getName(duk_context* ctx)
{
    TorrentStatus* status = Common::getPointer<TorrentStatus>(ctx, field);
    duk_push_string(ctx, status->getName().c_str());
    return 1;
}

duk_ret_t TorrentStatusWrapper::getProgress(duk_context* ctx)
{
    TorrentStatus* status = Common::getPointer<TorrentStatus>(ctx, field);
    duk_push_number(ctx, status->getProgress());
    return 1;
}

duk_ret_t TorrentStatusWrapper::getSavePath(duk_context* ctx)
{
    TorrentStatus* status = Common::getPointer<TorrentStatus>(ctx, field);
    duk_push_string(ctx, status->getSavePath().c_str());
    return 1;
}

duk_ret_t TorrentStatusWrapper::getDownloadRate(duk_context* ctx)
{
    TorrentStatus* status = Common::getPointer<TorrentStatus>(ctx, field);
    duk_push_int(ctx, status->getDownloadRate());
    return 1;
}

duk_ret_t TorrentStatusWrapper::getUploadRate(duk_context* ctx)
{
    TorrentStatus* status = Common::getPointer<TorrentStatus>(ctx, field);
    duk_push_int(ctx, status->getUploadRate());
    return 1;
}

duk_ret_t TorrentStatusWrapper::getDownloadedBytes(duk_context* ctx)
{
    TorrentStatus* status = Common::getPointer<TorrentStatus>(ctx, field);
    duk_push_number(ctx, status->getTotalDownload());
    return 1;
}

duk_ret_t TorrentStatusWrapper::getDownloadedBytesTotal(duk_context* ctx)
{
    TorrentStatus* status = Common::getPointer<TorrentStatus>(ctx, field);
    duk_push_number(ctx, status->getAllTimeDownload());
    return 1;
}

duk_ret_t TorrentStatusWrapper::getUploadedBytes(duk_context* ctx)
{
    TorrentStatus* status = Common::getPointer<TorrentStatus>(ctx, field);
    duk_push_number(ctx, status->getTotalUpload());
    return 1;
}

duk_ret_t TorrentStatusWrapper::getUploadedBytesTotal(duk_context* ctx)
{
    TorrentStatus* status = Common::getPointer<TorrentStatus>(ctx, field);
    duk_push_number(ctx, status->getAllTimeUpload());
    return 1;
}

duk_ret_t TorrentStatusWrapper::getNumPeers(duk_context* ctx)
{
    TorrentStatus* status = Common::getPointer<TorrentStatus>(ctx, field);
    duk_push_int(ctx, status->getNumPeers());
    return 1;
}

duk_ret_t TorrentStatusWrapper::getNumSeeds(duk_context* ctx)
{
    TorrentStatus* status = Common::getPointer<TorrentStatus>(ctx, field);
    duk_push_int(ctx, status->getNumSeeds());
    return 1;
}

duk_ret_t TorrentStatusWrapper::getState(duk_context* ctx)
{
    TorrentStatus* status = Common::getPointer<TorrentStatus>(ctx, field);
    duk_push_int(ctx, status->getState());
    return 1;
}

duk_ret_t TorrentStatusWrapper::isFinished(duk_context* ctx)
{
    TorrentStatus* status = Common::getPointer<TorrentStatus>(ctx, field);
    duk_push_boolean(ctx, status->isFinished());
    return 1;
}

duk_ret_t TorrentStatusWrapper::isMovingStorage(duk_context* ctx)
{
    TorrentStatus* status = Common::getPointer<TorrentStatus>(ctx, field);
    duk_push_boolean(ctx, status->isMovingStorage());
    return 1;
}

duk_ret_t TorrentStatusWrapper::isPaused(duk_context* ctx)
{
    TorrentStatus* status = Common::getPointer<TorrentStatus>(ctx, field);
    duk_push_boolean(ctx, status->isPaused());
    return 1;
}

duk_ret_t TorrentStatusWrapper::isSeeding(duk_context* ctx)
{
    TorrentStatus* status = Common::getPointer<TorrentStatus>(ctx, field);
    duk_push_boolean(ctx, status->isSeeding());
    return 1;
}

duk_ret_t TorrentStatusWrapper::isSequentialDownload(duk_context* ctx)
{
    TorrentStatus* status = Common::getPointer<TorrentStatus>(ctx, field);
    duk_push_boolean(ctx, status->isSequentialDownload());
    return 1;
}
