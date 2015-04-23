#include <Hadouken/Scripting/Modules/BitTorrent/TorrentHandleWrapper.hpp>

#include <Hadouken/BitTorrent/AnnounceEntry.hpp>
#include <Hadouken/BitTorrent/PeerInfo.hpp>
#include <Hadouken/BitTorrent/TorrentInfo.hpp>
#include <Hadouken/BitTorrent/TorrentHandle.hpp>
#include <Hadouken/BitTorrent/TorrentStatus.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/AnnounceEntryWrapper.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/PeerInfoWrapper.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/TorrentInfoWrapper.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/TorrentStatusWrapper.hpp>

#include "../common.hpp"
#include "../../duktape.h"

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Scripting::Modules;
using namespace Hadouken::Scripting::Modules::BitTorrent;

void TorrentHandleWrapper::initialize(duk_context* ctx, std::shared_ptr<Hadouken::BitTorrent::TorrentHandle> handle)
{
    duk_function_list_entry functions[] =
    {
        { "clearError",     clearError,     0 },
        { "forceRecheck",   forceRecheck,   0 },
        { "getPeers",       getPeers,       0 },
        { "getStatus",      getStatus,      0 },
        { "getTorrentInfo", getTorrentInfo, 0 },
        { "getTrackers",    getTrackers,    0 },
        { "moveStorage",    moveStorage,    1 },
        { "pause",          pause,          0 },
        { "queueBottom",    queueBottom,    0 },
        { "queueDown",      queueDown,      0 },
        { "queueTop",       queueTop,       0 },
        { "queueUp",        queueUp,        0 },
        { "renameFile",     renameFile,     2 },
        { "resume",         resume,         0 },
        { NULL,             NULL,           0 }
    };

    duk_idx_t idx = duk_push_object(ctx);
    duk_put_function_list(ctx, idx, functions);

    Common::setPointer<TorrentHandle>(ctx, idx, new TorrentHandle(*handle));

    // read-only properties
    DUK_READONLY_PROPERTY(ctx, idx, infoHash, getInfoHash);
    DUK_READONLY_PROPERTY(ctx, idx, queuePosition, getQueuePosition);
    DUK_READONLY_PROPERTY(ctx, idx, tags, getTags);

    // read+write properties
    DUK_READWRITE_PROPERTY(ctx, idx, maxConnections, getMaxConnections, setMaxConnections);
    DUK_READWRITE_PROPERTY(ctx, idx, maxUploads, getMaxUploads, setMaxUploads);
    DUK_READWRITE_PROPERTY(ctx, idx, resolveCountries, getResolveCountries, setResolveCountries);
    DUK_READWRITE_PROPERTY(ctx, idx, sequentialDownload, getSequentialDownload, setSequentialDownload);
    DUK_READWRITE_PROPERTY(ctx, idx, uploadMode, getUploadMode, setUploadMode);
    DUK_READWRITE_PROPERTY(ctx, idx, uploadLimit, getUploadLimit, setUploadLimit);

    duk_push_c_function(ctx, finalize, 1);
    duk_set_finalizer(ctx, -2);
}

duk_ret_t TorrentHandleWrapper::finalize(duk_context* ctx)
{
    Common::finalize<TorrentHandle>(ctx);
    return 0;
}

duk_ret_t TorrentHandleWrapper::clearError(duk_context* ctx)
{
    Common::getPointer<TorrentHandle>(ctx)->clearError();
    return 0;
}

duk_ret_t TorrentHandleWrapper::forceRecheck(duk_context* ctx)
{
    Common::getPointer<TorrentHandle>(ctx)->forceRecheck();
    return 0;
}

duk_ret_t TorrentHandleWrapper::getInfoHash(duk_context* ctx)
{
    TorrentHandle* handle = Common::getPointer<TorrentHandle>(ctx);
    duk_push_string(ctx, handle->getInfoHash().c_str());
    return 1;
}

duk_ret_t TorrentHandleWrapper::getPeers(duk_context* ctx)
{
    TorrentHandle* handle = Common::getPointer<TorrentHandle>(ctx);

    int arrayIndex = duk_push_array(ctx);
    int i = 0;

    for (PeerInfo peer : handle->getPeers())
    {
        PeerInfoWrapper::initialize(ctx, peer);
        duk_put_prop_index(ctx, arrayIndex, i);

        ++i;
    }

    return 1;
}

duk_ret_t TorrentHandleWrapper::getQueuePosition(duk_context* ctx)
{
    TorrentHandle* handle = Common::getPointer<TorrentHandle>(ctx);
    duk_push_int(ctx, handle->getQueuePosition());
    return 1;
}

duk_ret_t TorrentHandleWrapper::getStatus(duk_context* ctx)
{
    TorrentHandle* handle = Common::getPointer<TorrentHandle>(ctx);
    TorrentStatusWrapper::initialize(ctx, handle->getStatus());
    return 1;
}

duk_ret_t TorrentHandleWrapper::getTags(duk_context* ctx)
{
    TorrentHandle* handle = Common::getPointer<TorrentHandle>(ctx);

    int arrayIndex = duk_push_array(ctx);
    int i = 0;

    for (std::string tag : handle->getTags())
    {
        duk_push_string(ctx, tag.c_str());
        duk_put_prop_index(ctx, arrayIndex, i);

        ++i;
    }

    return 1;
}

duk_ret_t TorrentHandleWrapper::getTorrentInfo(duk_context* ctx)
{
    TorrentHandle* handle = Common::getPointer<TorrentHandle>(ctx);
    std::unique_ptr<TorrentInfo> info = handle->getTorrentFile();

    if (info)
    {
        TorrentInfoWrapper::initialize(ctx, *handle, std::move(info));
    }
    else
    {
        duk_push_null(ctx);
    }

    return 1;
}

duk_ret_t TorrentHandleWrapper::getTrackers(duk_context* ctx)
{
    TorrentHandle* handle = Common::getPointer<TorrentHandle>(ctx);

    int arrayIndex = duk_push_array(ctx);
    int i = 0;

    for (AnnounceEntry entry : handle->getTrackers())
    {
        AnnounceEntryWrapper::initialize(ctx, entry);
        duk_put_prop_index(ctx, arrayIndex, i);

        ++i;
    }

    return 1;
}

duk_ret_t TorrentHandleWrapper::moveStorage(duk_context* ctx)
{
    std::string path(duk_require_string(ctx, 0));

    TorrentHandle* handle = Common::getPointer<TorrentHandle>(ctx);
    handle->moveStorage(path);

    return 0;
}

duk_ret_t TorrentHandleWrapper::pause(duk_context* ctx)
{
    TorrentHandle* handle = Common::getPointer<TorrentHandle>(ctx);
    handle->pause();
    return 0;
}

duk_ret_t TorrentHandleWrapper::queueBottom(duk_context* ctx)
{
    TorrentHandle* handle = Common::getPointer<TorrentHandle>(ctx);
    handle->queueBottom();
    return 0;
}

duk_ret_t TorrentHandleWrapper::queueDown(duk_context* ctx)
{
    TorrentHandle* handle = Common::getPointer<TorrentHandle>(ctx);
    handle->queueDown();
    return 0;
}

duk_ret_t TorrentHandleWrapper::queueTop(duk_context* ctx)
{
    TorrentHandle* handle = Common::getPointer<TorrentHandle>(ctx);
    handle->queueTop();
    return 0;
}

duk_ret_t TorrentHandleWrapper::queueUp(duk_context* ctx)
{
    TorrentHandle* handle = Common::getPointer<TorrentHandle>(ctx);
    handle->queueUp();
    return 0;
}

duk_ret_t TorrentHandleWrapper::renameFile(duk_context* ctx)
{
    duk_int_t fileIndex = duk_require_int(ctx, 0);
    const char* name = duk_require_string(ctx, 1);

    Common::getPointer<TorrentHandle>(ctx)->renameFile(fileIndex, std::string(name));

    return 0;
}

duk_ret_t TorrentHandleWrapper::resume(duk_context* ctx)
{
    TorrentHandle* handle = Common::getPointer<TorrentHandle>(ctx);
    handle->resume();
    return 0;
}

duk_ret_t TorrentHandleWrapper::getMaxConnections(duk_context* ctx)
{
    TorrentHandle* handle = Common::getPointer<TorrentHandle>(ctx);
    duk_push_int(ctx, handle->getMaxConnections());
    return 1;
}

duk_ret_t TorrentHandleWrapper::getMaxUploads(duk_context* ctx)
{
    TorrentHandle* handle = Common::getPointer<TorrentHandle>(ctx);
    duk_push_int(ctx, handle->getMaxUploads());
    return 1;
}

duk_ret_t TorrentHandleWrapper::getResolveCountries(duk_context* ctx)
{
    TorrentHandle* handle = Common::getPointer<TorrentHandle>(ctx);
    duk_push_boolean(ctx, handle->getResolveCountries());
    return 1;
}

duk_ret_t TorrentHandleWrapper::getSequentialDownload(duk_context* ctx)
{
    TorrentHandle* handle = Common::getPointer<TorrentHandle>(ctx);
    duk_push_boolean(ctx, handle->getStatus().isSequentialDownload());
    return 1;
}

duk_ret_t TorrentHandleWrapper::getUploadLimit(duk_context* ctx)
{
    TorrentHandle* handle = Common::getPointer<TorrentHandle>(ctx);
    duk_push_int(ctx, handle->getUploadLimit());
    return 1;
}

duk_ret_t TorrentHandleWrapper::getUploadMode(duk_context* ctx)
{
    TorrentHandle* handle = Common::getPointer<TorrentHandle>(ctx);
    duk_push_boolean(ctx, handle->getUploadMode());
    return 1;
}

duk_ret_t TorrentHandleWrapper::setMaxConnections(duk_context* ctx)
{
    TorrentHandle* handle = Common::getPointer<TorrentHandle>(ctx);
    handle->setMaxConnections(duk_require_int(ctx, 0));
    return 0;
}

duk_ret_t TorrentHandleWrapper::setMaxUploads(duk_context* ctx)
{
    TorrentHandle* handle = Common::getPointer<TorrentHandle>(ctx);
    handle->setMaxUploads(duk_require_int(ctx, 0));
    return 0;
}

duk_ret_t TorrentHandleWrapper::setResolveCountries(duk_context* ctx)
{
    TorrentHandle* handle = Common::getPointer<TorrentHandle>(ctx);
    handle->setResolveCountries(duk_require_boolean(ctx, 0) > 0 ? true : false);
    return 0;
}

duk_ret_t TorrentHandleWrapper::setSequentialDownload(duk_context* ctx)
{
    TorrentHandle* handle = Common::getPointer<TorrentHandle>(ctx);
    handle->setSequentialDownload(duk_require_boolean(ctx, 0) > 0 ? true : false);
    return 0;
}

duk_ret_t TorrentHandleWrapper::setUploadLimit(duk_context* ctx)
{
    TorrentHandle* handle = Common::getPointer<TorrentHandle>(ctx);
    handle->setUploadLimit(duk_require_int(ctx, 0));

    return 0;
}

duk_ret_t TorrentHandleWrapper::setUploadMode(duk_context* ctx)
{
    duk_bool_t uploadMode = duk_require_boolean(ctx, 0);
    
    TorrentHandle* handle = Common::getPointer<TorrentHandle>(ctx);
    handle->setUploadMode(uploadMode > 0 ? true : false);

    return 0;
}
