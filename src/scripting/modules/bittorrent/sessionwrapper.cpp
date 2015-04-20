#include <Hadouken/Scripting/Modules/BitTorrent/SessionWrapper.hpp>

#include <Hadouken/BitTorrent/AddTorrentParams.hpp>
#include <Hadouken/BitTorrent/Session.hpp>
#include <Hadouken/BitTorrent/TorrentHandle.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/TorrentHandleWrapper.hpp>

#include "../common.hpp"
#include "../../duktape.h"

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Scripting::Modules;
using namespace Hadouken::Scripting::Modules::BitTorrent;

const char* SessionWrapper::field = "\xff" "Session";

void SessionWrapper::initialize(duk_context* ctx, Session& session)
{
    // Create session property
    duk_idx_t sessionIndex = duk_push_object(ctx);

    duk_push_pointer(ctx, &session);
    duk_put_prop_string(ctx, sessionIndex, field);

    duk_push_string(ctx, session.getLibtorrentVersion().c_str());
    duk_put_prop_string(ctx, sessionIndex, "LIBTORRENT_VERSION");

    // Session functions
    duk_function_list_entry functions[] =
    {
        { "addTorrentFile", SessionWrapper::addTorrentFile, 2 },
        { "addTorrentUri", SessionWrapper::addTorrentUri, 2 },
        { "findTorrent", SessionWrapper::findTorrent, 1 },
        { "getTorrents", SessionWrapper::getTorrents, 0 },
        { "removeTorrent", SessionWrapper::removeTorrent, 2 },
        { NULL, NULL, 0 }
    };

    duk_put_function_list(ctx, sessionIndex, functions);
}

duk_ret_t SessionWrapper::addTorrentFile(duk_context* ctx)
{
    Session* sess = Common::getPointer<Session>(ctx, field);

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

duk_ret_t SessionWrapper::addTorrentUri(duk_context* ctx)
{
    Session* sess = Common::getPointer<Session>(ctx, field);
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

duk_ret_t SessionWrapper::findTorrent(duk_context* ctx)
{
    std::string infoHash(duk_require_string(ctx, 0));

    Session* sess = Common::getPointer<Session>(ctx, field);
    std::shared_ptr<TorrentHandle> handle = sess->findTorrent(infoHash);

    if (handle)
    {
        BitTorrent::TorrentHandleWrapper::initialize(ctx, handle);
    }
    else
    {
        duk_push_null(ctx);
    }

    return 1;
}

duk_ret_t SessionWrapper::getTorrents(duk_context* ctx)
{
    Session* sess = Common::getPointer<Session>(ctx, field);

    int arrayIndex = duk_push_array(ctx);
    int i = 0;

    for (std::shared_ptr<TorrentHandle> handle : sess->getTorrents())
    {
        BitTorrent::TorrentHandleWrapper::initialize(ctx, handle);
        duk_put_prop_index(ctx, arrayIndex, i);

        ++i;
    }

    return 1;
}

duk_ret_t SessionWrapper::removeTorrent(duk_context* ctx)
{
    std::string infoHash(duk_require_string(ctx, 0));
    bool removeData = duk_require_boolean(ctx, 1);

    Session* sess = Common::getPointer<Session>(ctx, field);
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
