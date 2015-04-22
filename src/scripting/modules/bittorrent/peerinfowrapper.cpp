#include <Hadouken/Scripting/Modules/BitTorrent/PeerInfoWrapper.hpp>

#include <Hadouken/BitTorrent/PeerInfo.hpp>

#include "../common.hpp"
#include "../../duktape.h"

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Scripting::Modules;
using namespace Hadouken::Scripting::Modules::BitTorrent;

void PeerInfoWrapper::initialize(duk_context* ctx, PeerInfo& peerInfo)
{
    duk_function_list_entry functions[] =
    {
        { NULL, NULL, 0 }
    };

    duk_idx_t peerIndex = duk_push_object(ctx);
    duk_put_function_list(ctx, peerIndex, functions);

    Common::setPointer<PeerInfo>(ctx, peerIndex, new PeerInfo(peerInfo));

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
    Common::finalize<PeerInfo>(ctx);
    return 0;
}

duk_ret_t PeerInfoWrapper::getCountry(duk_context* ctx)
{
    PeerInfo* info = Common::getPointer<PeerInfo>(ctx);
    duk_push_string(ctx, info->getCountry().c_str());
    return 1;
}

duk_ret_t PeerInfoWrapper::getIp(duk_context* ctx)
{
    PeerInfo* info = Common::getPointer<PeerInfo>(ctx);
    duk_push_string(ctx, info->getRemoteAddress().host().toString().c_str());
    return 1;
}

duk_ret_t PeerInfoWrapper::getPort(duk_context* ctx)
{
    PeerInfo* info = Common::getPointer<PeerInfo>(ctx);
    duk_push_int(ctx, info->getRemoteAddress().port());
    return 1;
}

duk_ret_t PeerInfoWrapper::getConnectionType(duk_context* ctx)
{
    PeerInfo* info = Common::getPointer<PeerInfo>(ctx);
    duk_push_int(ctx, info->getConnectionType());
    return 1;
}

duk_ret_t PeerInfoWrapper::getClient(duk_context* ctx)
{
    PeerInfo* info = Common::getPointer<PeerInfo>(ctx);
    duk_push_string(ctx, info->getClient().c_str());
    return 1;
}

duk_ret_t PeerInfoWrapper::getProgress(duk_context* ctx)
{
    PeerInfo* info = Common::getPointer<PeerInfo>(ctx);
    duk_push_number(ctx, info->getProgress());
    return 1;
}

duk_ret_t PeerInfoWrapper::getDownloadRate(duk_context* ctx)
{
    PeerInfo* info = Common::getPointer<PeerInfo>(ctx);
    duk_push_int(ctx, info->getDownSpeed());
    return 1;
}

duk_ret_t PeerInfoWrapper::getUploadRate(duk_context* ctx)
{
    PeerInfo* info = Common::getPointer<PeerInfo>(ctx);
    duk_push_int(ctx, info->getUpSpeed());
    return 1;
}

duk_ret_t PeerInfoWrapper::getDownloadedBytes(duk_context* ctx)
{
    PeerInfo* info = Common::getPointer<PeerInfo>(ctx);
    duk_push_number(ctx, static_cast<duk_double_t>(info->getDownloadedBytes()));
    return 1;
}

duk_ret_t PeerInfoWrapper::getUploadedBytes(duk_context* ctx)
{
    PeerInfo* info = Common::getPointer<PeerInfo>(ctx);
    duk_push_number(ctx, static_cast<duk_double_t>(info->getUploadedBytes()));
    return 1;
}
