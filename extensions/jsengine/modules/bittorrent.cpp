#include "bittorrent.hpp"

#include <Hadouken/BitTorrent/AddTorrentParams.hpp>
#include <Hadouken/BitTorrent/Session.hpp>
#include <Hadouken/BitTorrent/TorrentHandle.hpp>
#include <Hadouken/BitTorrent/TorrentStatus.hpp>
#include <Hadouken/BitTorrent/TorrentSubsystem.hpp>
#include <Poco/Util/Application.h>

using namespace JsEngine::Modules;
using namespace Hadouken::BitTorrent;
using namespace Poco::Util;

duk_ret_t session_getLibtorrentVersion(duk_context* ctx)
{
    duk_push_string(ctx, "1.0.3.0");
    return 1;
}

const duk_function_list_entry BitTorrent::handle_functions_[] = {
    { "getStatus", BitTorrent::handleGetStatus, 0 },
    { "move",      BitTorrent::handleMove,      1 },
    { "pause",     BitTorrent::handlePause,     0 },
    { "resume",    BitTorrent::handleResume,    0 },
    { NULL, NULL, 0 }
};

const duk_function_list_entry BitTorrent::session_functions_[] = {
    { "addTorrentFile",  BitTorrent::sessionAddTorrentFile,  2 },
    { "getTorrents",     BitTorrent::sessionGetTorrents,     0 },
    { NULL, NULL, 0 }
};

const std::string BitTorrent::session_name_ = "\xff" "session";

duk_ret_t BitTorrent::init(duk_context* ctx)
{
    Application& app = Application::instance();
    Session& sess = app.getSubsystem<TorrentSubsystem>().getSession();

    duk_idx_t sessionIndex = duk_push_object(ctx);

    // Put internal session pointer
    duk_push_pointer(ctx, &sess);
    duk_put_prop_string(ctx, sessionIndex, session_name_.c_str());

    // Session functions
    duk_put_function_list(ctx, sessionIndex, session_functions_);
    
    // Session properties
    duk_push_string(ctx, "libtorrentVersion");
    duk_push_c_function(ctx, session_getLibtorrentVersion, 0);
    duk_def_prop(ctx, sessionIndex, DUK_DEFPROP_HAVE_GETTER);

    // Set properties and functions on exports
    duk_put_prop_string(ctx, 0, "session");

    return 0;
}

duk_ret_t BitTorrent::handleGetInfoHash(duk_context* ctx)
{
    TorrentHandle* handle = getTorrentHandleFromThis(ctx);
    duk_push_string(ctx, handle->getInfoHash().c_str());
    return 1;
}

duk_ret_t BitTorrent::handleGetQueuePosition(duk_context* ctx)
{
    TorrentHandle* handle = getTorrentHandleFromThis(ctx);
    duk_push_int(ctx, handle->getQueuePosition());
    return 1;
}

duk_ret_t BitTorrent::handleGetStatus(duk_context* ctx)
{
    TorrentHandle* handle = getTorrentHandleFromThis(ctx);
    setTorrentStatusObject(ctx, handle->getStatus());
    return 1;
}

duk_ret_t BitTorrent::handleFinalizer(duk_context* ctx)
{
    if (duk_get_prop_string(ctx, -1, "\xff" "handle"))
    {
        void* ptr = duk_get_pointer(ctx, -1);
        TorrentHandle* handle = static_cast<TorrentHandle*>(ptr);
        delete handle;
    }

    return 0;
}

duk_ret_t BitTorrent::handleMove(duk_context* ctx)
{
    std::string savePath(duk_require_string(ctx, 0));
    TorrentHandle* handle = getTorrentHandleFromThis(ctx);

    if (!handle)
    {
        duk_error(ctx, DUK_ERR_INTERNAL_ERROR, "Could not locate torrent handle.");
        return 0;
    }

    if (handle->isValid())
    {
        handle->moveStorage(savePath);
    }

    return 0;
}

duk_ret_t BitTorrent::handlePause(duk_context* ctx)
{
    TorrentHandle* handle = getTorrentHandleFromThis(ctx);

    if (!handle)
    {
        duk_error(ctx, DUK_ERR_INTERNAL_ERROR, "Could not locate torrent handle.");
        return 0;
    }

    if (handle->isValid())
    {
        handle->pause();
    }

    return 0;
}

duk_ret_t BitTorrent::handleResume(duk_context* ctx)
{
    TorrentHandle* handle = getTorrentHandleFromThis(ctx);

    if (!handle)
    {
        duk_error(ctx, DUK_ERR_INTERNAL_ERROR, "Could not locate torrent handle.");
        return 0;
    }

    if (handle->isValid())
    {
        handle->resume();
    }

    return 0;
}

duk_ret_t BitTorrent::sessionAddTorrentFile(duk_context* ctx)
{
    Session* sess = getSessionFromThis(ctx);

    if (!sess)
    {
        duk_error(ctx, DUK_ERR_INTERNAL_ERROR, "Could not locate session.");
        return 0;
    }

    const char* rawFilePath = duk_require_string(ctx, 0);
    std::string filePath(rawFilePath);

    // TODO: investigate better way to parse second parameter
    duk_get_prop_string(ctx, 1, "savePath");
    std::string savePath(duk_to_string(ctx, -1));
    duk_pop(ctx);

    AddTorrentParams p;
    p.savePath = savePath;
    
    Poco::Path fp(filePath);
    std::string hash = sess->addTorrentFile(fp, p);

    if (!hash.empty())
    {
        duk_push_string(ctx, hash.c_str());
        return 1;
    }

    return 0;
}

duk_ret_t BitTorrent::sessionGetTorrents(duk_context* ctx)
{
    Session* sess = getSessionFromThis(ctx);

    if (!sess)
    {
        duk_error(ctx, DUK_ERR_INTERNAL_ERROR, "Could not find session.");
        return 0;
    }

    std::vector<TorrentHandle> handles = sess->getTorrents();
    int arrayIndex = duk_push_array(ctx);
    int i = 0;

    for (TorrentHandle handle : handles)
    {
        setTorrentHandleObject(ctx, handle);
        duk_put_prop_index(ctx, arrayIndex, i);

        ++i;
    }

    return 1;
}

duk_ret_t BitTorrent::statusFinalizer(duk_context* ctx)
{
    if (duk_get_prop_string(ctx, -1, "\xff" "status"))
    {
        void* ptr = duk_get_pointer(ctx, -1);
        TorrentStatus* status = static_cast<TorrentStatus*>(ptr);
        delete status;
    }

    return 0;
}

duk_ret_t BitTorrent::statusGetActiveTime(duk_context* ctx)
{
    TorrentStatus* status = getTorrentStatusFromThis(ctx);
    duk_push_int(ctx, status->getActiveTime());
    return 1;
}

duk_ret_t BitTorrent::statusGetDownloadRate(duk_context* ctx)
{
    TorrentStatus* status = getTorrentStatusFromThis(ctx);
    duk_push_number(ctx, status->getDownloadRate());
    return 1;
}

duk_ret_t BitTorrent::statusGetError(duk_context* ctx)
{
    TorrentStatus* status = getTorrentStatusFromThis(ctx);
    duk_push_string(ctx, status->getError().c_str());
    return 1;
}

duk_ret_t BitTorrent::statusGetName(duk_context* ctx)
{
    TorrentStatus* status = getTorrentStatusFromThis(ctx);
    duk_push_string(ctx, status->getName().c_str());
    return 1;
}

duk_ret_t BitTorrent::statusGetProgress(duk_context* ctx)
{
    TorrentStatus* status = getTorrentStatusFromThis(ctx);
    duk_push_number(ctx, status->getProgress());
    return 1;
}

duk_ret_t BitTorrent::statusGetSavePath(duk_context* ctx)
{
    TorrentStatus* status = getTorrentStatusFromThis(ctx);
    duk_push_string(ctx, status->getSavePath().c_str());
    return 1;
}

duk_ret_t BitTorrent::statusGetState(duk_context* ctx)
{
    TorrentStatus* status = getTorrentStatusFromThis(ctx);
    duk_push_int(ctx, (int)status->getState());
    return 1;
}

duk_ret_t BitTorrent::statusGetUploadRate(duk_context* ctx)
{
    TorrentStatus* status = getTorrentStatusFromThis(ctx);
    duk_push_number(ctx, status->getUploadRate());
    return 1;
}

Session* BitTorrent::getSessionFromThis(duk_context* ctx)
{
    duk_push_this(ctx);

    if (duk_get_prop_string(ctx, -1, session_name_.c_str()))
    {
        void* ptr = duk_get_pointer(ctx, -1);
        duk_pop(ctx);

        return static_cast<Session*>(ptr);
    }

    duk_pop(ctx);
    return 0;
}

TorrentHandle* BitTorrent::getTorrentHandleFromThis(duk_context* ctx)
{
    duk_push_this(ctx);

    if (duk_get_prop_string(ctx, -1, "\xff" "handle"))
    {
        void* ptr = duk_get_pointer(ctx, -1);
        duk_pop(ctx);

        return static_cast<TorrentHandle*>(ptr);
    }

    duk_pop(ctx);
    return 0;
}

TorrentStatus* BitTorrent::getTorrentStatusFromThis(duk_context* ctx)
{
    duk_push_this(ctx);

    if (duk_get_prop_string(ctx, -1, "\xff" "status"))
    {
        void* ptr = duk_get_pointer(ctx, -1);
        duk_pop(ctx);

        return static_cast<TorrentStatus*>(ptr);
    }

    duk_pop(ctx);
    return 0;
}

#define DUK_READONLY_PROPERTY(ctx, index, name, func) \
    duk_push_string(ctx, ##name); \
    duk_push_c_function(ctx, func, 0); \
    duk_def_prop(ctx, index, DUK_DEFPROP_HAVE_GETTER);

void BitTorrent::setTorrentHandleObject(duk_context* ctx, TorrentHandle& handle)
{
    duk_idx_t handleIndex = duk_push_object(ctx);
    duk_put_function_list(ctx, handleIndex, handle_functions_);

    duk_push_pointer(ctx, new TorrentHandle(handle));
    duk_put_prop_string(ctx, handleIndex, "\xff" "handle");

    DUK_READONLY_PROPERTY(ctx, handleIndex, "infoHash", BitTorrent::handleGetInfoHash);
    DUK_READONLY_PROPERTY(ctx, handleIndex, "queuePosition", BitTorrent::handleGetQueuePosition);

    duk_push_c_function(ctx, BitTorrent::handleFinalizer, 1);
    duk_set_finalizer(ctx, -2);
}

void BitTorrent::setTorrentStatusObject(duk_context* ctx, TorrentStatus& status)
{
    duk_idx_t statusIndex = duk_push_object(ctx);
    // duk_put_function_list(ctx, statusIndex, status_functions_);

    duk_push_pointer(ctx, new TorrentStatus(status));
    duk_put_prop_string(ctx, statusIndex, "\xff" "status");

    DUK_READONLY_PROPERTY(ctx, statusIndex, "activeTime", BitTorrent::statusGetActiveTime);
    DUK_READONLY_PROPERTY(ctx, statusIndex, "downloadRate", BitTorrent::statusGetDownloadRate);
    DUK_READONLY_PROPERTY(ctx, statusIndex, "error", BitTorrent::statusGetError);
    DUK_READONLY_PROPERTY(ctx, statusIndex, "name", BitTorrent::statusGetName);
    DUK_READONLY_PROPERTY(ctx, statusIndex, "progress", BitTorrent::statusGetProgress);
    DUK_READONLY_PROPERTY(ctx, statusIndex, "savePath", BitTorrent::statusGetSavePath);
    DUK_READONLY_PROPERTY(ctx, statusIndex, "uploadRate", BitTorrent::statusGetUploadRate);

    duk_push_c_function(ctx, BitTorrent::statusFinalizer, 1);
    duk_set_finalizer(ctx, -2);
}

