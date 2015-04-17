#include <Hadouken/Scripting/Modules/BitTorrentModule.hpp>

#include <Hadouken/BitTorrent/AddTorrentParams.hpp>
#include <Hadouken/BitTorrent/FileEntry.hpp>
#include <Hadouken/BitTorrent/FileStorage.hpp>
#include <Hadouken/BitTorrent/PeerInfo.hpp>
#include <Hadouken/BitTorrent/Session.hpp>
#include <Hadouken/BitTorrent/TorrentInfo.hpp>
#include <Hadouken/BitTorrent/TorrentHandle.hpp>
#include <Hadouken/BitTorrent/TorrentStatus.hpp>
#include <Hadouken/BitTorrent/TorrentSubsystem.hpp>
#include <Poco/Util/Application.h>

#include <vector>

#include "../duktape.h"

#define DUK_READONLY_PROPERTY(ctx, index, name, func) \
    duk_push_string(ctx, #name); \
    duk_push_c_function(ctx, func, 0); \
    duk_def_prop(ctx, index, DUK_DEFPROP_HAVE_GETTER | DUK_DEFPROP_HAVE_ENUMERABLE | DUK_DEFPROP_ENUMERABLE);

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Scripting::Modules;
using namespace Poco::Util;

const char* handleField = "\xff" "handle";
const char* infoField = "\xff" "info";
const char* peerField = "\xff" "peer";
const char* sessionField = "\xff" "session";
const char* statusField = "\xff" "status";

duk_ret_t BitTorrentModule::initialize(duk_context* ctx)
{
    Application& app = Application::instance();
    Session& sess = app.getSubsystem<TorrentSubsystem>().getSession();

    // Create session property
    duk_idx_t sessionIndex = duk_push_object(ctx);
    
    duk_push_pointer(ctx, &sess);
    duk_put_prop_string(ctx, sessionIndex, sessionField);

    duk_push_string(ctx, sess.getLibtorrentVersion().c_str());
    duk_put_prop_string(ctx, sessionIndex, "LIBTORRENT_VERSION");

    // Session functions
    duk_function_list_entry sessionFunctions[] = 
    {
        { "addTorrentFile", BitTorrentModule::session_addTorrentFile, 2 },
        { "addTorrentUri",  BitTorrentModule::session_addTorrentUri,  2 },
        { "findTorrent",    BitTorrentModule::session_findTorrent,    1 },
        { "getTorrents",    BitTorrentModule::session_getTorrents,    0 },
        { "removeTorrent",  BitTorrentModule::session_removeTorrent,  2 },
        { NULL, NULL, 0 }
    };

    duk_put_function_list(ctx, sessionIndex, sessionFunctions);

    // Set properties and functions on exports
    duk_put_prop_string(ctx, 0, "session");

    return 0;
}

void BitTorrentModule::info_initialize(duk_context* ctx, TorrentHandle& handle, std::unique_ptr<TorrentInfo> info)
{
    duk_function_list_entry infoFunctions[] =
    {
        { "getFiles", BitTorrentModule::info_getFiles, 0 },
        { NULL, NULL, 0 }
    };

    duk_idx_t infoIndex = duk_push_object(ctx);
    duk_put_function_list(ctx, infoIndex, infoFunctions);

    // Set handle pointer
    duk_push_pointer(ctx, new TorrentHandle(handle));
    duk_put_prop_string(ctx, infoIndex, handleField);

    // Set info pointer
    duk_push_pointer(ctx, info.release());
    duk_put_prop_string(ctx, infoIndex, infoField);

    DUK_READONLY_PROPERTY(ctx, infoIndex, totalSize, BitTorrentModule::info_getTotalSize);

    duk_push_c_function(ctx, BitTorrentModule::info_finalize, 1);
    duk_set_finalizer(ctx, -2);
}

duk_ret_t BitTorrentModule::info_finalize(duk_context* ctx)
{
    if (duk_get_prop_string(ctx, -1, handleField))
    {
        void* ptr = duk_get_pointer(ctx, -1);
        TorrentHandle* handle = static_cast<TorrentHandle*>(ptr);
        delete handle;

        duk_pop(ctx);
    }

    if (duk_get_prop_string(ctx, -1, infoField))
    {
        void* ptr = duk_get_pointer(ctx, -1);
        TorrentInfo* info = static_cast<TorrentInfo*>(ptr);
        delete info;

        duk_pop(ctx);
    }

    return 0;
}

duk_ret_t BitTorrentModule::info_getFiles(duk_context* ctx)
{
    TorrentHandle* handle = getPointerFromThis<TorrentHandle>(ctx, handleField);
    TorrentInfo* info = getPointerFromThis<TorrentInfo>(ctx, infoField);

    int arrayIndex = duk_push_array(ctx);
    int i = 0;

    FileStorage fileStorage = info->getFiles();
    std::vector<int64_t> progress;
    handle->getFileProgress(progress);

    for (int i = 0; i < fileStorage.getNumFiles(); i++)
    {
        FileEntry entry = fileStorage.getEntryAt(i);

        duk_idx_t entryIndex = duk_push_object(ctx);
        duk_push_string(ctx, entry.getPath().c_str());
        duk_put_prop_string(ctx, entryIndex, "path");

        duk_push_number(ctx, progress[i]);
        duk_put_prop_string(ctx, entryIndex, "progress");

        duk_push_number(ctx, entry.getSize());
        duk_put_prop_string(ctx, entryIndex, "size");

        // Put entry object
        duk_put_prop_index(ctx, arrayIndex, i);
    }

    return 1;
}

duk_ret_t BitTorrentModule::info_getTotalSize(duk_context* ctx)
{
    TorrentInfo* info = getPointerFromThis<TorrentInfo>(ctx, infoField);
    duk_push_number(ctx, info->getTotalSize());
    return 1;
}

void BitTorrentModule::handle_initialize(duk_context* ctx, std::shared_ptr<TorrentHandle> handle)
{
    duk_function_list_entry handleFunctions[] =
    {
        { "getPeers",       BitTorrentModule::handle_getPeers,       0 },
        { "getStatus",      BitTorrentModule::handle_getStatus,      0 },
        { "getTorrentInfo", BitTorrentModule::handle_getTorrentInfo, 0 },
        { "moveStorage",    BitTorrentModule::handle_moveStorage,    1 },
        { "pause",          BitTorrentModule::handle_pause,          0 },
        { "resume",         BitTorrentModule::handle_resume,         0 },
        { NULL, NULL, 0}
    };

    duk_idx_t handleIndex = duk_push_object(ctx);
    duk_put_function_list(ctx, handleIndex, handleFunctions);

    duk_push_pointer(ctx, new TorrentHandle(*handle));
    duk_put_prop_string(ctx, handleIndex, handleField);

    DUK_READONLY_PROPERTY(ctx, handleIndex, infoHash, BitTorrentModule::handle_getInfoHash);
    DUK_READONLY_PROPERTY(ctx, handleIndex, queuePosition, BitTorrentModule::handle_getQueuePosition);
    DUK_READONLY_PROPERTY(ctx, handleIndex, tags, BitTorrentModule::handle_getTags);

    duk_push_c_function(ctx, BitTorrentModule::handle_finalize, 1);
    duk_set_finalizer(ctx, -2);
}

duk_ret_t BitTorrentModule::handle_finalize(duk_context* ctx)
{
    if (duk_get_prop_string(ctx, -1, handleField))
    {
        void* ptr = duk_get_pointer(ctx, -1);
        TorrentHandle* handle = static_cast<TorrentHandle*>(ptr);
        delete handle;
    }

    return 0;
}

duk_ret_t BitTorrentModule::handle_getInfoHash(duk_context* ctx)
{
    TorrentHandle* handle = getPointerFromThis<TorrentHandle>(ctx, handleField);
    duk_push_string(ctx, handle->getInfoHash().c_str());
    return 1;
}

duk_ret_t BitTorrentModule::handle_getPeers(duk_context* ctx)
{
    TorrentHandle* handle = getPointerFromThis<TorrentHandle>(ctx, handleField);

    int arrayIndex = duk_push_array(ctx);
    int i = 0;

    for (PeerInfo peer : handle->getPeers())
    {
        peer_initialize(ctx, peer);
        duk_put_prop_index(ctx, arrayIndex, i);

        ++i;
    }

    return 1;
}

duk_ret_t BitTorrentModule::handle_getQueuePosition(duk_context* ctx)
{
    TorrentHandle* handle = getPointerFromThis<TorrentHandle>(ctx, handleField);
    duk_push_int(ctx, handle->getQueuePosition());
    return 1;
}

duk_ret_t BitTorrentModule::handle_getStatus(duk_context* ctx)
{
    TorrentHandle* handle = getPointerFromThis<TorrentHandle>(ctx, handleField);
    status_initialize(ctx, handle->getStatus());
    return 1;
}

duk_ret_t BitTorrentModule::handle_getTags(duk_context* ctx)
{
    TorrentHandle* handle = getPointerFromThis<TorrentHandle>(ctx, handleField);
    
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

duk_ret_t BitTorrentModule::handle_getTorrentInfo(duk_context* ctx)
{
    TorrentHandle* handle = getPointerFromThis<TorrentHandle>(ctx, handleField);
    std::unique_ptr<TorrentInfo> info = handle->getTorrentFile();

    if (info)
    {
        info_initialize(ctx, *handle, std::move(info));
    }
    else
    {
        duk_push_null(ctx);
    }

    return 1;
}

duk_ret_t BitTorrentModule::handle_moveStorage(duk_context* ctx)
{
    std::string path(duk_require_string(ctx, 0));
    
    TorrentHandle* handle = getPointerFromThis<TorrentHandle>(ctx, handleField);
    handle->moveStorage(path);

    return 0;
}

duk_ret_t BitTorrentModule::handle_pause(duk_context* ctx)
{
    TorrentHandle* handle = getPointerFromThis<TorrentHandle>(ctx, handleField);
    handle->pause();
    return 0;
}

duk_ret_t BitTorrentModule::handle_resume(duk_context* ctx)
{
    TorrentHandle* handle = getPointerFromThis<TorrentHandle>(ctx, handleField);
    handle->resume();
    return 0;
}

void BitTorrentModule::peer_initialize(duk_context* ctx, PeerInfo& peer)
{
    duk_function_list_entry peerFunctions[] =
    {
        { NULL, NULL, 0 }
    };

    duk_idx_t peerIndex = duk_push_object(ctx);
    duk_put_function_list(ctx, peerIndex, peerFunctions);

    duk_push_pointer(ctx, new PeerInfo(peer));
    duk_put_prop_string(ctx, peerIndex, peerField);

    DUK_READONLY_PROPERTY(ctx, peerIndex, country, BitTorrentModule::peer_getCountry);
    DUK_READONLY_PROPERTY(ctx, peerIndex, ip, BitTorrentModule::peer_getIp);
    DUK_READONLY_PROPERTY(ctx, peerIndex, port, BitTorrentModule::peer_getPort);
    DUK_READONLY_PROPERTY(ctx, peerIndex, connectionType, BitTorrentModule::peer_getConnectionType);
    DUK_READONLY_PROPERTY(ctx, peerIndex, client, BitTorrentModule::peer_getClient);
    DUK_READONLY_PROPERTY(ctx, peerIndex, progress, BitTorrentModule::peer_getProgress);
    DUK_READONLY_PROPERTY(ctx, peerIndex, downloadRate, BitTorrentModule::peer_getDownloadRate);
    DUK_READONLY_PROPERTY(ctx, peerIndex, uploadRate, BitTorrentModule::peer_getUploadRate);
    DUK_READONLY_PROPERTY(ctx, peerIndex, downloadedBytes, BitTorrentModule::peer_getDownloadedBytes);
    DUK_READONLY_PROPERTY(ctx, peerIndex, uploadedBytes, BitTorrentModule::peer_getUploadedBytes);

    duk_push_c_function(ctx, BitTorrentModule::peer_finalize, 1);
    duk_set_finalizer(ctx, -2);
}

duk_ret_t BitTorrentModule::peer_finalize(duk_context* ctx)
{
    if (duk_get_prop_string(ctx, -1, peerField))
    {
        void* ptr = duk_get_pointer(ctx, -1);
        PeerInfo* info = static_cast<PeerInfo*>(ptr);
        delete info;
    }

    return 0;
}

duk_ret_t BitTorrentModule::peer_getConnectionType(duk_context* ctx)
{
    PeerInfo* info = getPointerFromThis<PeerInfo>(ctx, peerField);
    duk_push_int(ctx, info->getConnectionType());
    return 1;
}

duk_ret_t BitTorrentModule::peer_getCountry(duk_context* ctx)
{
    PeerInfo* info = getPointerFromThis<PeerInfo>(ctx, peerField);
    duk_push_string(ctx, info->getCountry().c_str());
    return 1;
}

duk_ret_t BitTorrentModule::peer_getIp(duk_context* ctx)
{
    PeerInfo* info = getPointerFromThis<PeerInfo>(ctx, peerField);
    duk_push_string(ctx, info->getRemoteAddress().host().toString().c_str());
    return 1;
}

duk_ret_t BitTorrentModule::peer_getPort(duk_context* ctx)
{
    PeerInfo* info = getPointerFromThis<PeerInfo>(ctx, peerField);
    duk_push_int(ctx, info->getRemoteAddress().port());
    return 1;
}

duk_ret_t BitTorrentModule::peer_getClient(duk_context* ctx)
{
    PeerInfo* info = getPointerFromThis<PeerInfo>(ctx, peerField);
    duk_push_string(ctx, info->getClient().c_str());
    return 1;
}

duk_ret_t BitTorrentModule::peer_getProgress(duk_context* ctx)
{
    PeerInfo* info = getPointerFromThis<PeerInfo>(ctx, peerField);
    duk_push_number(ctx, info->getProgress());
    return 1;
}

duk_ret_t BitTorrentModule::peer_getDownloadRate(duk_context* ctx)
{
    PeerInfo* info = getPointerFromThis<PeerInfo>(ctx, peerField);
    duk_push_int(ctx, info->getDownSpeed());
    return 1;
}

duk_ret_t BitTorrentModule::peer_getUploadRate(duk_context* ctx)
{
    PeerInfo* info = getPointerFromThis<PeerInfo>(ctx, peerField);
    duk_push_int(ctx, info->getUpSpeed());
    return 1;
}

duk_ret_t BitTorrentModule::peer_getDownloadedBytes(duk_context* ctx)
{
    PeerInfo* info = getPointerFromThis<PeerInfo>(ctx, peerField);
    duk_push_number(ctx, info->getDownloadedBytes());
    return 1;
}

duk_ret_t BitTorrentModule::peer_getUploadedBytes(duk_context* ctx)
{
    PeerInfo* info = getPointerFromThis<PeerInfo>(ctx, peerField);
    duk_push_number(ctx, info->getUploadedBytes());
    return 1;
}

duk_ret_t BitTorrentModule::session_addTorrentFile(duk_context* ctx)
{
    Session* sess = getPointerFromThis<Session>(ctx, sessionField);

    duk_size_t size;
    const char* buffer = static_cast<const char*>(duk_require_buffer(ctx, 0, &size));

    std::vector<char> data(buffer, buffer + size);

    AddTorrentParams params;

    if (duk_has_prop_string(ctx, 1, "savePath"))
    {
        duk_get_prop_string(ctx, 1, "savePath");
        params.savePath = std::string(duk_get_string(ctx, -1));
        duk_pop(ctx);
    }

    std::string infoHash = sess->addTorrentFile(data, params);
    
    duk_push_string(ctx, infoHash.c_str());
    return 1;
}

duk_ret_t BitTorrentModule::session_addTorrentUri(duk_context* ctx)
{
    Session* sess = getPointerFromThis<Session>(ctx, sessionField);
    std::string uri(duk_require_string(ctx, 0));

    AddTorrentParams params;

    if (duk_has_prop_string(ctx, 1, "savePath"))
    {
        duk_get_prop_string(ctx, 1, "savePath");
        params.savePath = std::string(duk_get_string(ctx, -1));
        duk_pop(ctx);
    }

    sess->addTorrentUri(uri, params);
    
    duk_push_true(ctx);
    return 1;
}

duk_ret_t BitTorrentModule::session_findTorrent(duk_context* ctx)
{
    std::string infoHash(duk_require_string(ctx, 0));
    
    Session* sess = getPointerFromThis<Session>(ctx, sessionField);
    std::shared_ptr<TorrentHandle> handle = sess->findTorrent(infoHash);

    if (handle)
    {
        handle_initialize(ctx, handle);
    }
    else
    {
        duk_push_null(ctx);
    }

    return 1;
}

duk_ret_t BitTorrentModule::session_getTorrents(duk_context* ctx)
{
    Session* sess = getPointerFromThis<Session>(ctx, sessionField);

    int arrayIndex = duk_push_array(ctx);
    int i = 0;

    for (std::shared_ptr<TorrentHandle> handle : sess->getTorrents())
    {
        handle_initialize(ctx, handle);
        duk_put_prop_index(ctx, arrayIndex, i);

        ++i;
    }

    return 1;
}

duk_ret_t BitTorrentModule::session_removeTorrent(duk_context* ctx)
{
    std::string infoHash(duk_require_string(ctx, 0));
    bool removeData = duk_require_boolean(ctx, 1);

    Session* sess = getPointerFromThis<Session>(ctx, sessionField);
    std::shared_ptr<TorrentHandle> handle = sess->findTorrent(infoHash);

    if (handle)
    {
        sess->removeTorrent(handle, removeData ? 1 : 0);
        duk_push_true(ctx);
    }
    else
    {
        duk_push_false(ctx);
    }

    return 1;
}

void BitTorrentModule::status_initialize(duk_context* ctx, TorrentStatus& status)
{
    duk_idx_t statusIndex = duk_push_object(ctx);

    // Internal pointer
    duk_push_pointer(ctx, new TorrentStatus(status));
    duk_put_prop_string(ctx, statusIndex, statusField);

    DUK_READONLY_PROPERTY(ctx, statusIndex, name, BitTorrentModule::status_getName);
    DUK_READONLY_PROPERTY(ctx, statusIndex, progress, BitTorrentModule::status_getProgress);
    DUK_READONLY_PROPERTY(ctx, statusIndex, savePath, BitTorrentModule::status_getSavePath);
    DUK_READONLY_PROPERTY(ctx, statusIndex, downloadRate, BitTorrentModule::status_getDownloadRate);
    DUK_READONLY_PROPERTY(ctx, statusIndex, uploadRate, BitTorrentModule::status_getUploadRate);
    DUK_READONLY_PROPERTY(ctx, statusIndex, downloadedBytes, BitTorrentModule::status_getDownloadedBytes);
    DUK_READONLY_PROPERTY(ctx, statusIndex, downloadedBytesTotal, BitTorrentModule::status_getDownloadedBytesTotal);
    DUK_READONLY_PROPERTY(ctx, statusIndex, uploadedBytes, BitTorrentModule::status_getUploadedBytes);
    DUK_READONLY_PROPERTY(ctx, statusIndex, uploadedBytesTotal, BitTorrentModule::status_getUploadedBytesTotal);
    DUK_READONLY_PROPERTY(ctx, statusIndex, numPeers, BitTorrentModule::status_getNumPeers);
    DUK_READONLY_PROPERTY(ctx, statusIndex, numSeeds, BitTorrentModule::status_getNumSeeds);
    DUK_READONLY_PROPERTY(ctx, statusIndex, state, BitTorrentModule::status_getState);
    DUK_READONLY_PROPERTY(ctx, statusIndex, isFinished, BitTorrentModule::status_isFinished);
    DUK_READONLY_PROPERTY(ctx, statusIndex, isMovingStorage, BitTorrentModule::status_isMovingStorage);
    DUK_READONLY_PROPERTY(ctx, statusIndex, isPaused, BitTorrentModule::status_isPaused);
    DUK_READONLY_PROPERTY(ctx, statusIndex, isSeeding, BitTorrentModule::status_isSeeding);
    DUK_READONLY_PROPERTY(ctx, statusIndex, isSequentialDownload, BitTorrentModule::status_isSequentialDownload);

    duk_push_c_function(ctx, BitTorrentModule::status_finalize, 1);
    duk_set_finalizer(ctx, -2);
}

duk_ret_t BitTorrentModule::status_finalize(duk_context* ctx)
{
    if (duk_get_prop_string(ctx, -1, statusField))
    {
        void* ptr = duk_get_pointer(ctx, -1);
        TorrentStatus* status = static_cast<TorrentStatus*>(ptr);
        delete status;
    }

    return 0;
}

duk_ret_t BitTorrentModule::status_getDownloadedBytes(duk_context* ctx)
{
    TorrentStatus* status = getPointerFromThis<TorrentStatus>(ctx, statusField);
    duk_push_number(ctx, status->getTotalDownload());
    return 1;
}

duk_ret_t BitTorrentModule::status_getDownloadedBytesTotal(duk_context* ctx)
{
    TorrentStatus* status = getPointerFromThis<TorrentStatus>(ctx, statusField);
    duk_push_number(ctx, status->getAllTimeDownload());
    return 1;
}

duk_ret_t BitTorrentModule::status_getDownloadRate(duk_context* ctx)
{
    TorrentStatus* status = getPointerFromThis<TorrentStatus>(ctx, statusField);
    duk_push_int(ctx, status->getDownloadRate());
    return 1;
}

duk_ret_t BitTorrentModule::status_getName(duk_context* ctx)
{
    TorrentStatus* status = getPointerFromThis<TorrentStatus>(ctx, statusField);
    duk_push_string(ctx, status->getName().c_str());
    return 1;
}

duk_ret_t BitTorrentModule::status_getNumPeers(duk_context* ctx)
{
    TorrentStatus* status = getPointerFromThis<TorrentStatus>(ctx, statusField);
    duk_push_int(ctx, status->getNumPeers());
    return 1;
}

duk_ret_t BitTorrentModule::status_getNumSeeds(duk_context* ctx)
{
    TorrentStatus* status = getPointerFromThis<TorrentStatus>(ctx, statusField);
    duk_push_int(ctx, status->getNumSeeds());
    return 1;
}

duk_ret_t BitTorrentModule::status_getProgress(duk_context* ctx)
{
    TorrentStatus* status = getPointerFromThis<TorrentStatus>(ctx, statusField);
    duk_push_number(ctx, status->getProgress());
    return 1;
}

duk_ret_t BitTorrentModule::status_getSavePath(duk_context* ctx)
{
    TorrentStatus* status = getPointerFromThis<TorrentStatus>(ctx, statusField);
    duk_push_string(ctx, status->getSavePath().c_str());
    return 1;
}

duk_ret_t BitTorrentModule::status_getState(duk_context* ctx)
{
    TorrentStatus* status = getPointerFromThis<TorrentStatus>(ctx, statusField);
    duk_push_int(ctx, status->getState());
    return 1;
}

duk_ret_t BitTorrentModule::status_getUploadedBytes(duk_context* ctx)
{
    TorrentStatus* status = getPointerFromThis<TorrentStatus>(ctx, statusField);
    duk_push_number(ctx, status->getTotalUpload());
    return 1;
}

duk_ret_t BitTorrentModule::status_getUploadedBytesTotal(duk_context* ctx)
{
    TorrentStatus* status = getPointerFromThis<TorrentStatus>(ctx, statusField);
    duk_push_number(ctx, status->getAllTimeUpload());
    return 1;
}

duk_ret_t BitTorrentModule::status_getUploadRate(duk_context* ctx)
{
    TorrentStatus* status = getPointerFromThis<TorrentStatus>(ctx, statusField);
    duk_push_int(ctx, status->getUploadRate());
    return 1;
}

duk_ret_t BitTorrentModule::status_isFinished(duk_context* ctx)
{
    TorrentStatus* status = getPointerFromThis<TorrentStatus>(ctx, statusField);
    duk_push_boolean(ctx, status->isFinished());
    return 1;
}

duk_ret_t BitTorrentModule::status_isMovingStorage(duk_context* ctx)
{
    TorrentStatus* status = getPointerFromThis<TorrentStatus>(ctx, statusField);
    duk_push_boolean(ctx, status->isMovingStorage());
    return 1;
}

duk_ret_t BitTorrentModule::status_isPaused(duk_context* ctx)
{
    TorrentStatus* status = getPointerFromThis<TorrentStatus>(ctx, statusField);
    duk_push_boolean(ctx, status->isPaused());
    return 1;
}

duk_ret_t BitTorrentModule::status_isSeeding(duk_context* ctx)
{
    TorrentStatus* status = getPointerFromThis<TorrentStatus>(ctx, statusField);
    duk_push_boolean(ctx, status->isSeeding());
    return 1;
}

duk_ret_t BitTorrentModule::status_isSequentialDownload(duk_context* ctx)
{
    TorrentStatus* status = getPointerFromThis<TorrentStatus>(ctx, statusField);
    duk_push_boolean(ctx, status->isSequentialDownload());
    return 1;
}

template<class T>
T* BitTorrentModule::getPointerFromThis(duk_context* ctx, const char* fieldName)
{
    duk_push_this(ctx);

    T* res = 0;

    if (duk_get_prop_string(ctx, -1, fieldName))
    {
        res = static_cast<T*>(duk_get_pointer(ctx, -1));
        duk_pop(ctx);
    }

    duk_pop(ctx);
    return res;
}
