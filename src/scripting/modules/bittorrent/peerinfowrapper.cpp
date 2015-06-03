#include <Hadouken/Scripting/Modules/BitTorrent/PeerInfoWrapper.hpp>

#include <libtorrent/peer_info.hpp>

#include "../common.hpp"
#include "../../duktape.h"

using namespace Hadouken::Scripting::Modules;
using namespace Hadouken::Scripting::Modules::BitTorrent;

void PeerInfoWrapper::initialize(duk_context* ctx, libtorrent::peer_info& peer)
{
    duk_function_list_entry functions[] =
    {
        { NULL, NULL, 0 }
    };

    duk_idx_t peerIndex = duk_push_object(ctx);
    duk_put_function_list(ctx, peerIndex, functions);

    Common::setPointer<libtorrent::peer_info>(ctx, peerIndex, new libtorrent::peer_info(peer));

    DUK_READONLY_PROPERTY(ctx, peerIndex, country, PeerInfoWrapper::getCountry);
    DUK_READONLY_PROPERTY(ctx, peerIndex, ip, PeerInfoWrapper::getIp);
    DUK_READONLY_PROPERTY(ctx, peerIndex, port, PeerInfoWrapper::getPort);
    DUK_READONLY_PROPERTY(ctx, peerIndex, connectionType, PeerInfoWrapper::getConnectionType);
    DUK_READONLY_PROPERTY(ctx, peerIndex, client, PeerInfoWrapper::getClient);
    DUK_READONLY_PROPERTY(ctx, peerIndex, progress, PeerInfoWrapper::getProgress);
    DUK_READONLY_PROPERTY(ctx, peerIndex, downloadRate, PeerInfoWrapper::getDownloadRate);
    DUK_READONLY_PROPERTY(ctx, peerIndex, uploadRate, PeerInfoWrapper::getUploadRate);
    DUK_READONLY_PROPERTY(ctx, peerIndex, downloadedBytes, PeerInfoWrapper::getDownloadedBytes);
    DUK_READONLY_PROPERTY(ctx, peerIndex, uploadedBytes, PeerInfoWrapper::getUploadedBytes);

    duk_push_c_function(ctx, PeerInfoWrapper::finalize, 1);
    duk_set_finalizer(ctx, -2);
}

duk_ret_t PeerInfoWrapper::finalize(duk_context* ctx)
{
    Common::finalize<libtorrent::peer_info>(ctx);
    return 0;
}

duk_ret_t PeerInfoWrapper::getCountry(duk_context* ctx)
{
    libtorrent::peer_info* info = Common::getPointer<libtorrent::peer_info>(ctx);
    duk_push_string(ctx, info->country);
    return 1;
}

duk_ret_t PeerInfoWrapper::getIp(duk_context* ctx)
{
    libtorrent::peer_info* info = Common::getPointer<libtorrent::peer_info>(ctx);
    duk_push_string(ctx, info->ip.address().to_string().c_str());
    return 1;
}

duk_ret_t PeerInfoWrapper::getPort(duk_context* ctx)
{
    libtorrent::peer_info* info = Common::getPointer<libtorrent::peer_info>(ctx);
    duk_push_int(ctx, info->ip.port());
    return 1;
}

duk_ret_t PeerInfoWrapper::getConnectionType(duk_context* ctx)
{
    libtorrent::peer_info* info = Common::getPointer<libtorrent::peer_info>(ctx);
    duk_push_int(ctx, info->connection_type);
    return 1;
}

duk_ret_t PeerInfoWrapper::getClient(duk_context* ctx)
{
    libtorrent::peer_info* info = Common::getPointer<libtorrent::peer_info>(ctx);
    duk_push_string(ctx, info->client.c_str());
    return 1;
}

duk_ret_t PeerInfoWrapper::getProgress(duk_context* ctx)
{
    libtorrent::peer_info* info = Common::getPointer<libtorrent::peer_info>(ctx);
    duk_push_number(ctx, info->progress);
    return 1;
}

duk_ret_t PeerInfoWrapper::getDownloadRate(duk_context* ctx)
{
    libtorrent::peer_info* info = Common::getPointer<libtorrent::peer_info>(ctx);
    duk_push_int(ctx, info->down_speed);
    return 1;
}

duk_ret_t PeerInfoWrapper::getUploadRate(duk_context* ctx)
{
    libtorrent::peer_info* info = Common::getPointer<libtorrent::peer_info>(ctx);
    duk_push_int(ctx, info->up_speed);
    return 1;
}

duk_ret_t PeerInfoWrapper::getDownloadedBytes(duk_context* ctx)
{
    libtorrent::peer_info* info = Common::getPointer<libtorrent::peer_info>(ctx);
    duk_push_number(ctx, static_cast<duk_double_t>(info->total_download));
    return 1;
}

duk_ret_t PeerInfoWrapper::getUploadedBytes(duk_context* ctx)
{
    libtorrent::peer_info* info = Common::getPointer<libtorrent::peer_info>(ctx);
    duk_push_number(ctx, static_cast<duk_double_t>(info->total_upload));
    return 1;
}
