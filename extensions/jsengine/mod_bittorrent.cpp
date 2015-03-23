#include "mod_bittorrent.hpp"

#include <Hadouken/BitTorrent/Session.hpp>
#include <Hadouken/BitTorrent/TorrentHandle.hpp>
#include <Hadouken/BitTorrent/TorrentStatus.hpp>
#include <Hadouken/BitTorrent/TorrentSubsystem.hpp>
#include <Poco/Util/Application.h>

using namespace Hadouken::BitTorrent;
using namespace Poco::Util;

void push_torrent_handle(TorrentHandle& handle, duk_context* ctx);

duk_ret_t session_getTorrents(duk_context* ctx)
{
    std::vector<TorrentHandle> handles;

    duk_push_this(ctx);

    if (duk_get_prop_string(ctx, -1, "\xff" "session"))
    {
        void* ptr = duk_get_pointer(ctx, -1);
        Session* sess = static_cast<Session*>(ptr);

        handles = sess->getTorrents();
    }

    duk_pop(ctx);

    int arrayIndex = duk_push_array(ctx);
    int i = 0;

    for (TorrentHandle handle : handles)
    {
        push_torrent_handle(handle, ctx);
        duk_put_prop_index(ctx, arrayIndex, i);

        ++i;
    }

    return 1;
}

duk_ret_t handle_move(duk_context* ctx)
{
    std::string savePath(duk_get_string(ctx, 1));

    duk_push_this(ctx);

    if (duk_get_prop_string(ctx, -1, "\xff" "handle"))
    {
        void* ptr = duk_get_pointer(ctx, -1);
        TorrentHandle* handle = static_cast<TorrentHandle*>(ptr);

        if (handle->isValid())
        {
            handle->moveStorage(savePath);
        }
    }

    duk_pop(ctx);

    return 0;
}

duk_ret_t handle_pause(duk_context* ctx)
{
    duk_push_this(ctx);

    if (duk_get_prop_string(ctx, -1, "\xff" "handle"))
    {
        void* ptr = duk_get_pointer(ctx, -1);

        TorrentHandle* handle = static_cast<TorrentHandle*>(ptr);

        if (handle->isValid())
        {
            handle->pause();
        }
    }

    duk_pop(ctx);

    return 0;
}

duk_ret_t handle_resume(duk_context* ctx)
{
    duk_push_this(ctx);

    if (duk_get_prop_string(ctx, -1, "\xff" "handle"))
    {
        void* ptr = duk_get_pointer(ctx, -1);

        TorrentHandle* handle = static_cast<TorrentHandle*>(ptr);
        handle->resume();
    }

    duk_pop(ctx);

    return 0;
}

duk_ret_t handle_finalizer(duk_context* ctx)
{
    if (duk_get_prop_string(ctx, -1, "\xff" "handle"))
    {
        void* ptr = duk_get_pointer(ctx, -1);
        TorrentHandle* handle = static_cast<TorrentHandle*>(ptr);
        delete handle;
    }

    return 0;
}

static const duk_function_list_entry session_funcs[] = {
    { "getTorrents", session_getTorrents, 0 },
    { NULL, NULL, 0 }
};

static const duk_function_list_entry handle_funcs[] = {
    { "move", handle_move, 1 },
    { "pause", handle_pause, 0 },
    { "resume", handle_resume, 0 },
    { NULL, NULL, 0 }
};

void push_torrent_handle(TorrentHandle& handle, duk_context* ctx)
{
    TorrentStatus status = handle.getStatus();

    duk_idx_t handleIndex = duk_push_object(ctx);
    duk_put_function_list(ctx, handleIndex, handle_funcs);

    duk_push_pointer(ctx, new TorrentHandle(handle));
    duk_put_prop_string(ctx, handleIndex, "\xff" "handle");

    duk_push_string(ctx, handle.getInfoHash().c_str());
    duk_put_prop_string(ctx, handleIndex, "infoHash");

    duk_push_string(ctx, status.getName().c_str());
    duk_put_prop_string(ctx, handleIndex, "name");

    duk_push_number(ctx, status.getProgress());
    duk_put_prop_string(ctx, handleIndex, "progress");

    duk_push_string(ctx, status.getSavePath().c_str());
    duk_put_prop_string(ctx, handleIndex, "savePath");

    duk_push_int(ctx, status.getDownloadRate());
    duk_put_prop_string(ctx, handleIndex, "downloadRate");

    duk_push_int(ctx, status.getUploadRate());
    duk_put_prop_string(ctx, handleIndex, "uploadRate");

    duk_push_number(ctx, status.getTotalDownload());
    duk_put_prop_string(ctx, handleIndex, "downloadedBytes");

    duk_push_number(ctx, status.getTotalUpload());
    duk_put_prop_string(ctx, handleIndex, "uploadedBytes");

    duk_push_c_function(ctx, handle_finalizer, 1);
    duk_set_finalizer(ctx, -2);
}

namespace JsEngine
{
    duk_ret_t dukopen_bittorrent(duk_context* ctx)
    {
        Application& app = Application::instance();
        Session& sess = app.getSubsystem<TorrentSubsystem>().getSession();

        duk_idx_t sessionIndex = duk_push_object(ctx);

        // Put internal session pointer
        duk_push_pointer(ctx, &sess);
        duk_put_prop_string(ctx, sessionIndex, "\xff" "session");

        // Session functions
        duk_put_function_list(ctx, sessionIndex, session_funcs);
        
        // Session properties
        duk_push_string(ctx, sess.getLibtorrentVersion().c_str());
        duk_put_prop_string(ctx, sessionIndex, "libtorrentVersion");

        // Set properties and functions on exports
        duk_put_prop_string(ctx, 0, "session");

        return 0;
    }
}
