#include <Hadouken/Scripting/Modules/BitTorrent/TorrentHandleWrapper.hpp>

#include <Hadouken/BitTorrent/TorrentSubsystem.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/AnnounceEntryWrapper.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/PeerInfoWrapper.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/TorrentInfoWrapper.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/TorrentStatusWrapper.hpp>
#include <libtorrent/peer_info.hpp>
#include <libtorrent/torrent_handle.hpp>
#include <Poco/Util/Application.h>

#include "../common.hpp"
#include "../../duktape.h"

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Scripting::Modules;
using namespace Hadouken::Scripting::Modules::BitTorrent;

void TorrentHandleWrapper::initialize(duk_context* ctx, const libtorrent::torrent_handle& handle)
{
    duk_function_list_entry functions[] =
    {
        { "clearError",      clearError,      0 },
        { "flushCache",      flushCache,      0 },
        { "forceRecheck",    forceRecheck,    0 },
        { "getFileProgress", getFileProgress, 0 },
        { "getPeers",        getPeers,        0 },
        { "getStatus",       getStatus,       0 },
        { "getTorrentInfo",  getTorrentInfo,  0 },
        { "getTrackers",     getTrackers,     0 },
        { "havePiece",       havePiece,       1 },
        { "metadata",        metadata,        DUK_VARARGS },
        { "moveStorage",     moveStorage,     1 },
        { "pause",           pause,           0 },
        { "queueBottom",     queueBottom,     0 },
        { "queueDown",       queueDown,       0 },
        { "queueTop",        queueTop,        0 },
        { "queueUp",         queueUp,         0 },
        { "readPiece",       readPiece,       1 },
        { "renameFile",      renameFile,      2 },
        { "resume",          resume,          0 },
        { "saveResumeData",  saveResumeData,  0 },
        { "setPriority",     setPriority,     1 },
        { NULL,              NULL,            0 }
    };

    duk_idx_t idx = duk_push_object(ctx);
    duk_put_function_list(ctx, idx, functions);

    Common::setPointer<libtorrent::torrent_handle>(ctx, idx, new libtorrent::torrent_handle(handle));

    // read-only properties
    DUK_READONLY_PROPERTY(ctx, idx, infoHash, getInfoHash);
    DUK_READONLY_PROPERTY(ctx, idx, isValid, isValid);
    DUK_READONLY_PROPERTY(ctx, idx, queuePosition, getQueuePosition);

    duk_push_c_function(ctx, finalize, 1);
    duk_set_finalizer(ctx, -2);

    // read+write properties
    DUK_READWRITE_PROPERTY(ctx, idx, maxConnections, MaxConnections);
    DUK_READWRITE_PROPERTY(ctx, idx, maxUploads, MaxUploads);
    DUK_READWRITE_PROPERTY(ctx, idx, resolveCountries, ResolveCountries);
    DUK_READWRITE_PROPERTY(ctx, idx, sequentialDownload, SequentialDownload);
    DUK_READWRITE_PROPERTY(ctx, idx, uploadMode, UploadMode);
    DUK_READWRITE_PROPERTY(ctx, idx, uploadLimit, UploadLimit);
}

duk_ret_t TorrentHandleWrapper::finalize(duk_context* ctx)
{
    Common::finalize<libtorrent::torrent_handle>(ctx);
    return 0;
}

duk_ret_t TorrentHandleWrapper::finalizeMetadata(duk_context* ctx)
{
    Common::finalize<std::string>(ctx);
    return 0;
}

duk_ret_t TorrentHandleWrapper::clearError(duk_context* ctx)
{
    Common::getPointer<libtorrent::torrent_handle>(ctx)->clear_error();
    return 0;
}

duk_ret_t TorrentHandleWrapper::flushCache(duk_context* ctx)
{
    Common::getPointer<libtorrent::torrent_handle>(ctx)->flush_cache();
    return 0;
}

duk_ret_t TorrentHandleWrapper::forceRecheck(duk_context* ctx)
{
    Common::getPointer<libtorrent::torrent_handle>(ctx)->force_recheck();
    return 0;
}

duk_ret_t TorrentHandleWrapper::getInfoHash(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = Common::getPointer<libtorrent::torrent_handle>(ctx);
    duk_push_string(ctx, libtorrent::to_hex(handle->info_hash().to_string()).c_str());
    return 1;
}

duk_ret_t TorrentHandleWrapper::isValid(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = Common::getPointer<libtorrent::torrent_handle>(ctx);
    duk_push_boolean(ctx, handle->is_valid());
    return 1;
}

duk_ret_t TorrentHandleWrapper::getFileProgress(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = Common::getPointer<libtorrent::torrent_handle>(ctx);
    
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

duk_ret_t TorrentHandleWrapper::getPeers(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = Common::getPointer<libtorrent::torrent_handle>(ctx);

    int arrayIndex = duk_push_array(ctx);
    int i = 0;

    std::vector<libtorrent::peer_info> peers;
    handle->get_peer_info(peers);

    for (libtorrent::peer_info peer : peers)
    {
        PeerInfoWrapper::initialize(ctx, peer);
        duk_put_prop_index(ctx, arrayIndex, i);

        ++i;
    }

    return 1;
}

duk_ret_t TorrentHandleWrapper::getQueuePosition(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = Common::getPointer<libtorrent::torrent_handle>(ctx);
    duk_push_int(ctx, handle->queue_position());
    return 1;
}

duk_ret_t TorrentHandleWrapper::getStatus(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = Common::getPointer<libtorrent::torrent_handle>(ctx);
    TorrentStatusWrapper::initialize(ctx, handle->status());
    return 1;
}

duk_ret_t TorrentHandleWrapper::getTorrentInfo(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = Common::getPointer<libtorrent::torrent_handle>(ctx);
    boost::intrusive_ptr<libtorrent::torrent_info const> info = handle->torrent_file();

    if (info)
    {
        TorrentInfoWrapper::initialize(ctx, *info);
    }
    else
    {
        duk_push_null(ctx);
    }

    return 1;
}

duk_ret_t TorrentHandleWrapper::getTrackers(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = Common::getPointer<libtorrent::torrent_handle>(ctx);

    int arrayIndex = duk_push_array(ctx);
    int i = 0;

    for (libtorrent::announce_entry entry : handle->trackers())
    {
        AnnounceEntryWrapper::initialize(ctx, entry);
        duk_put_prop_index(ctx, arrayIndex, i);

        ++i;
    }

    return 1;
}

duk_ret_t TorrentHandleWrapper::havePiece(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = Common::getPointer<libtorrent::torrent_handle>(ctx);
    duk_push_boolean(ctx, handle->have_piece(duk_require_int(ctx, 0)));
    return 1;
}

duk_ret_t TorrentHandleWrapper::moveStorage(duk_context* ctx)
{
    std::string path(duk_require_string(ctx, 0));

    libtorrent::torrent_handle* handle = Common::getPointer<libtorrent::torrent_handle>(ctx);
    handle->move_storage(path);

    return 0;
}

duk_ret_t TorrentHandleWrapper::pause(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = Common::getPointer<libtorrent::torrent_handle>(ctx);
    handle->auto_managed(false);
    handle->pause();

    return 0;
}

duk_ret_t TorrentHandleWrapper::queueBottom(duk_context* ctx)
{
    Common::getPointer<libtorrent::torrent_handle>(ctx)->queue_position_bottom();
    return 0;
}

duk_ret_t TorrentHandleWrapper::queueDown(duk_context* ctx)
{
    Common::getPointer<libtorrent::torrent_handle>(ctx)->queue_position_down();
    return 0;
}

duk_ret_t TorrentHandleWrapper::queueTop(duk_context* ctx)
{
    Common::getPointer<libtorrent::torrent_handle>(ctx)->queue_position_top();
    return 0;
}

duk_ret_t TorrentHandleWrapper::queueUp(duk_context* ctx)
{
    Common::getPointer<libtorrent::torrent_handle>(ctx)->queue_position_up();
    return 0;
}

duk_ret_t TorrentHandleWrapper::readPiece(duk_context* ctx)
{
    Common::getPointer<libtorrent::torrent_handle>(ctx)->read_piece(duk_require_int(ctx, 0));
    return 0;
}

duk_ret_t TorrentHandleWrapper::renameFile(duk_context* ctx)
{
    duk_int_t fileIndex = duk_require_int(ctx, 0);
    const char* name = duk_require_string(ctx, 1);

    Common::getPointer<libtorrent::torrent_handle>(ctx)->rename_file(fileIndex, std::string(name));

    return 0;
}

duk_ret_t TorrentHandleWrapper::resume(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = Common::getPointer<libtorrent::torrent_handle>(ctx);
    handle->auto_managed(true);
    handle->resume();

    return 0;
}

duk_ret_t TorrentHandleWrapper::saveResumeData(duk_context* ctx)
{
    Common::getPointer<libtorrent::torrent_handle>(ctx)->save_resume_data();
    return 0;
}

duk_ret_t TorrentHandleWrapper::setPriority(duk_context* ctx)
{
    Common::getPointer<libtorrent::torrent_handle>(ctx)->set_priority(duk_require_int(ctx, 0));
    return 0;
}

duk_ret_t TorrentHandleWrapper::metadata(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = Common::getPointer<libtorrent::torrent_handle>(ctx);
    return 0;
    /*Session& sess = Poco::Util::Application::instance().getSubsystem<TorrentSubsystem>().getSession();
    std::string hash = handle->getInfoHash();

    switch (duk_get_top(ctx))
    {
    case 0:
    {
        duk_idx_t metaIdx = duk_push_object(ctx);

        for (std::string key : sess.getTorrentMetadataKeys(hash))
        {
            std::string val = sess.getTorrentMetadata(hash, key);

            if (!val.empty())
            {
                duk_push_string(ctx, val.c_str());
                duk_json_decode(ctx, -1);

                duk_put_prop_string(ctx, metaIdx, key.c_str());
            }
        }

        return 1;
    }

    case 1:
    {
        std::string key(duk_require_string(ctx, 0));
        std::string val = sess.getTorrentMetadata(hash, key);

        if (val.empty())
        {
            duk_push_undefined(ctx);
        }
        else
        {
            duk_push_string(ctx, val.c_str());
            duk_json_decode(ctx, -1);
        }

        return 1;
    }

    case 2:
    {
        std::string key(duk_require_string(ctx, 0));
        std::string val;

        if (!duk_is_undefined(ctx, 1))
        {
            duk_json_encode(ctx, 1);
            std::string val(duk_require_string(ctx, 1));
        }
        
        sess.setTorrentMetadata(hash, key, val);
        break;
    }
    }

    return 0;*/
}

duk_ret_t TorrentHandleWrapper::getMaxConnections(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = Common::getPointer<libtorrent::torrent_handle>(ctx);
    duk_push_int(ctx, handle->max_connections());
    return 1;
}

duk_ret_t TorrentHandleWrapper::getMaxUploads(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = Common::getPointer<libtorrent::torrent_handle>(ctx);
    duk_push_int(ctx, handle->max_uploads());
    return 1;
}

duk_ret_t TorrentHandleWrapper::getResolveCountries(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = Common::getPointer<libtorrent::torrent_handle>(ctx);
    duk_push_boolean(ctx, handle->resolve_countries());
    return 1;
}

duk_ret_t TorrentHandleWrapper::getSequentialDownload(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = Common::getPointer<libtorrent::torrent_handle>(ctx);
    duk_push_boolean(ctx, handle->status().sequential_download);
    return 1;
}

duk_ret_t TorrentHandleWrapper::getUploadLimit(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = Common::getPointer<libtorrent::torrent_handle>(ctx);
    duk_push_int(ctx, handle->upload_limit());
    return 1;
}

duk_ret_t TorrentHandleWrapper::getUploadMode(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = Common::getPointer<libtorrent::torrent_handle>(ctx);
    duk_push_boolean(ctx, handle->status().upload_mode);
    return 1;
}

duk_ret_t TorrentHandleWrapper::setMaxConnections(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = Common::getPointer<libtorrent::torrent_handle>(ctx);
    handle->set_max_connections(duk_require_int(ctx, 0));
    return 0;
}

duk_ret_t TorrentHandleWrapper::setMaxUploads(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = Common::getPointer<libtorrent::torrent_handle>(ctx);
    handle->set_max_uploads(duk_require_int(ctx, 0));
    return 0;
}

duk_ret_t TorrentHandleWrapper::setResolveCountries(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = Common::getPointer<libtorrent::torrent_handle>(ctx);
    handle->resolve_countries(duk_require_boolean(ctx, 0) > 0 ? true : false);
    return 0;
}

duk_ret_t TorrentHandleWrapper::setSequentialDownload(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = Common::getPointer<libtorrent::torrent_handle>(ctx);
    handle->set_sequential_download(duk_require_boolean(ctx, 0) > 0 ? true : false);
    return 0;
}

duk_ret_t TorrentHandleWrapper::setUploadLimit(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = Common::getPointer<libtorrent::torrent_handle>(ctx);
    handle->set_upload_limit(duk_require_int(ctx, 0));
    return 0;
}

duk_ret_t TorrentHandleWrapper::setUploadMode(duk_context* ctx)
{
    libtorrent::torrent_handle* handle = Common::getPointer<libtorrent::torrent_handle>(ctx);
    handle->set_upload_mode(duk_require_boolean(ctx, 0) > 0 ? true : false);
    return 0;
}
