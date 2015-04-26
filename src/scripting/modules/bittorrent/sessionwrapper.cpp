#include <Hadouken/Scripting/Modules/BitTorrent/SessionWrapper.hpp>

#include <Hadouken/BitTorrent/AddTorrentParams.hpp>
#include <Hadouken/BitTorrent/Session.hpp>
#include <Hadouken/BitTorrent/SessionStatus.hpp>
#include <Hadouken/BitTorrent/TorrentHandle.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/TorrentHandleWrapper.hpp>

#include "../common.hpp"
#include "../../duktape.h"

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Scripting::Modules;
using namespace Hadouken::Scripting::Modules::BitTorrent;

void SessionWrapper::initialize(duk_context* ctx, Session& session)
{
    // Create session property
    duk_idx_t sessionIndex = duk_push_object(ctx);

    Common::setPointer<Session>(ctx, sessionIndex, &session);

    duk_push_string(ctx, session.getLibtorrentVersion().c_str());
    duk_put_prop_string(ctx, sessionIndex, "LIBTORRENT_VERSION");

    DUK_READONLY_PROPERTY(ctx, sessionIndex, isListening, isListening);
    DUK_READONLY_PROPERTY(ctx, sessionIndex, isPaused, isPaused);
    DUK_READONLY_PROPERTY(ctx, sessionIndex, listenPort, getListenPort);
    DUK_READONLY_PROPERTY(ctx, sessionIndex, sslListenPort, getSslListenPort);

    // Session functions
    duk_function_list_entry functions[] =
    {
        { "addTorrent",     addTorrent,     2 },
        { "addTorrentFile", addTorrentFile, 2 },
        { "addTorrentUri",  addTorrentUri,  2 },
        { "findTorrent",    findTorrent,    1 },
        { "getStatus",      getStatus,      0 },
        { "getTorrents",    getTorrents,    0 },
        { "pause",          pause,          0 },
        { "removeTorrent",  removeTorrent,  2 },
        { "resume",         resume,         0 },
        { NULL, NULL, 0 }
    };

    duk_put_function_list(ctx, sessionIndex, functions);
}

duk_ret_t SessionWrapper::addTorrent(duk_context* ctx)
{
    Session* sess = Common::getPointer<Session>(ctx);

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

    std::string infoHash = sess->addTorrent(data, params);

    duk_push_string(ctx, infoHash.c_str());
    return 1;
}

duk_ret_t SessionWrapper::addTorrentFile(duk_context* ctx)
{
    Session* sess = Common::getPointer<Session>(ctx);

    AddTorrentParams params;

    if (duk_has_prop_string(ctx, 1, "savePath"))
    {
        duk_get_prop_string(ctx, 1, "savePath");
        params.savePath = std::string(duk_get_string(ctx, -1));
        duk_pop(ctx);
    }

    std::string infoHash = sess->addTorrentFile(duk_require_string(ctx, 0), params);

    duk_push_string(ctx, infoHash.c_str());
    return 1;
}

duk_ret_t SessionWrapper::addTorrentUri(duk_context* ctx)
{
    Session* sess = Common::getPointer<Session>(ctx);
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

    Session* sess = Common::getPointer<Session>(ctx);
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

duk_ret_t SessionWrapper::getListenPort(duk_context* ctx)
{
    Session* sess = Common::getPointer<Session>(ctx);
    duk_push_int(ctx, sess->getListenPort());
    return 1;
}

duk_ret_t SessionWrapper::getSslListenPort(duk_context* ctx)
{
    Session* sess = Common::getPointer<Session>(ctx);
    duk_push_int(ctx, sess->getSslListenPort());
    return 1;
}

#define DUK_SESSIONSTATUS_PROP(type, method, name) \
    duk_push_##type(ctx, method); \
    duk_put_prop_string(ctx, statusIndex, name);

duk_ret_t SessionWrapper::getStatus(duk_context* ctx)
{
    Session* sess = Common::getPointer<Session>(ctx);
    SessionStatus status = sess->getStatus();

    duk_idx_t statusIndex = duk_push_object(ctx);

    DUK_SESSIONSTATUS_PROP(boolean, status.hasIncomingConnections(), "hasIncomingConnections");
    DUK_SESSIONSTATUS_PROP(int, status.getUploadRate(), "totalUploadRate");
    DUK_SESSIONSTATUS_PROP(int, status.getDownloadRate(), "totalDownloadRate");
    DUK_SESSIONSTATUS_PROP(number, static_cast<duk_double_t>(status.getTotalDownload()), "totalDownloadedBytes");
    DUK_SESSIONSTATUS_PROP(number, static_cast<duk_double_t>(status.getTotalUpload()), "totalUploadedBytes");
    DUK_SESSIONSTATUS_PROP(int, status.getPayloadDownloadRate(), "payloadDownloadRate");
    DUK_SESSIONSTATUS_PROP(int, status.getPayloadUploadRate(), "payloadUploadRate");
    DUK_SESSIONSTATUS_PROP(number, static_cast<duk_double_t>(status.getTotalPayloadDownload()), "payloadDownloadedBytes");
    DUK_SESSIONSTATUS_PROP(number, static_cast<duk_double_t>(status.getTotalPayloadUpload()), "payloadUploadedBytes");
    DUK_SESSIONSTATUS_PROP(int, status.getIpOverheadDownloadRate(), "ipOverheadDownloadRate");
    DUK_SESSIONSTATUS_PROP(int, status.getIpOverheadUploadRate(), "ipOverheadUploadRate");
    DUK_SESSIONSTATUS_PROP(number, static_cast<duk_double_t>(status.getTotalIpOverheadDownload()), "ipOverheadDownloadedBytes");
    DUK_SESSIONSTATUS_PROP(number, static_cast<duk_double_t>(status.getTotalIpOverheadUpload()), "ipOverheadUploadedBytes");
    DUK_SESSIONSTATUS_PROP(int, status.getDhtDownloadRate(), "dhtDownloadRate");
    DUK_SESSIONSTATUS_PROP(int, status.getDhtUploadRate(), "dhtUploadRate");
    DUK_SESSIONSTATUS_PROP(number, static_cast<duk_double_t>(status.getTotalDhtDownload()), "dhtDownloadedBytes");
    DUK_SESSIONSTATUS_PROP(number, static_cast<duk_double_t>(status.getTotalDhtUpload()), "dhtUploadedBytes");
    DUK_SESSIONSTATUS_PROP(int, status.getTrackerDownloadRate(), "trackerDownloadRate");
    DUK_SESSIONSTATUS_PROP(int, status.getTrackerUploadRate(), "trackerUploadRate");
    DUK_SESSIONSTATUS_PROP(number, static_cast<duk_double_t>(status.getTotalTrackerDownload()), "trackerDownloadedBytes");
    DUK_SESSIONSTATUS_PROP(number, static_cast<duk_double_t>(status.getTotalTrackerUpload()), "trackerUploadedBytes");
    DUK_SESSIONSTATUS_PROP(number, static_cast<duk_double_t>(status.getTotalFailedBytes()), "totalFailedBytes");
    DUK_SESSIONSTATUS_PROP(number, static_cast<duk_double_t>(status.getTotalRedundantBytes()), "totalRedundantBytes");
    DUK_SESSIONSTATUS_PROP(int, status.getNumPeers(), "numPeers");
    DUK_SESSIONSTATUS_PROP(int, status.getNumUnchoked(), "numUnchoked");
    DUK_SESSIONSTATUS_PROP(int, status.getAllowedUploadSlots(), "allowedUploadSlots");
    DUK_SESSIONSTATUS_PROP(int, status.getDownBandwidthQueue(), "downBandwidthQueue");
    DUK_SESSIONSTATUS_PROP(int, status.getUpBandwidthQueue(), "upBandwidthQueue");
    DUK_SESSIONSTATUS_PROP(int, status.getDownBandwidthBytesQueue(), "downBandwidthBytesQueue");
    DUK_SESSIONSTATUS_PROP(int, status.getUpBandwidthBytesQueue(), "upBandwidthBytesQueue");
    DUK_SESSIONSTATUS_PROP(int, status.getOptimisticUnchokeCounter(), "optimisticUnchokeCounter");
    DUK_SESSIONSTATUS_PROP(int, status.getUnchokeCounter(), "unchokeCounter");
    DUK_SESSIONSTATUS_PROP(int, status.getDiskReadQueue(), "diskReadQueue");
    DUK_SESSIONSTATUS_PROP(int, status.getDiskWriteQueue(), "diskWriteQueue");
    DUK_SESSIONSTATUS_PROP(int, status.getDhtNodes(), "dhtNodes");
    DUK_SESSIONSTATUS_PROP(int, status.getDhtNodeCache(), "dhtNodeCache");
    DUK_SESSIONSTATUS_PROP(int, status.getDhtTorrents(), "dhtTorrents");
    DUK_SESSIONSTATUS_PROP(number, static_cast<duk_double_t>(status.getDhtGlobalNodes()), "dhtGlobalNodes");
    DUK_SESSIONSTATUS_PROP(int, status.getDhtTotalAllocations(), "dhtTotalAllocations");

    return 1;
}

duk_ret_t SessionWrapper::getTorrents(duk_context* ctx)
{
    Session* sess = Common::getPointer<Session>(ctx);

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

duk_ret_t SessionWrapper::isListening(duk_context* ctx)
{
    Session* sess = Common::getPointer<Session>(ctx);
    duk_push_boolean(ctx, sess->isListening());
    return 1;
}

duk_ret_t SessionWrapper::isPaused(duk_context* ctx)
{
    Session* sess = Common::getPointer<Session>(ctx);
    duk_push_boolean(ctx, sess->isPaused());
    return 1;
}

duk_ret_t SessionWrapper::pause(duk_context* ctx)
{
    Common::getPointer<Session>(ctx)->pause();
    return 0;
}

duk_ret_t SessionWrapper::removeTorrent(duk_context* ctx)
{
    std::string infoHash(duk_require_string(ctx, 0));
    duk_bool_t removeData = duk_require_boolean(ctx, 1);

    Session* sess = Common::getPointer<Session>(ctx);
    std::shared_ptr<TorrentHandle> handle = sess->findTorrent(infoHash);

    if (handle)
    {
        sess->removeTorrent(handle, removeData);
        duk_push_true(ctx);
    }
    else
    {
        duk_push_false(ctx);
    }

    return 1;
}

duk_ret_t SessionWrapper::resume(duk_context* ctx)
{
    Common::getPointer<Session>(ctx)->resume();
    return 0;
}
