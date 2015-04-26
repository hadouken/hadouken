#include <Hadouken/Scripting/Modules/BitTorrent/Events/StatsEvent.hpp>

#include <Hadouken/BitTorrent/TorrentHandle.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/TorrentHandleWrapper.hpp>

#include "../../common.hpp"
#include "../../../duktape.h"

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Scripting::Modules::BitTorrent::Events;

StatsEvent::StatsEvent(std::shared_ptr<TorrentHandle> handle, int interval, int (&transferred)[10])
    : TorrentEvent(handle)
{
    interval_ = interval;
    transferred_ = std::vector<int>(std::begin(transferred), std::end(transferred));
}

void StatsEvent::push(duk_context* ctx)
{
    duk_idx_t idx = duk_push_object(ctx);
    
    TorrentEvent::push(ctx);
    duk_put_prop_string(ctx, idx, "torrent");

    duk_push_int(ctx, interval_);
    duk_put_prop_string(ctx, idx, "interval");

    duk_push_int(ctx, transferred_[0]);
    duk_put_prop_string(ctx, idx, "payloadUploadRate");

    duk_push_int(ctx, transferred_[1]);
    duk_put_prop_string(ctx, idx, "protocolUploadRate");

    duk_push_int(ctx, transferred_[2]);
    duk_put_prop_string(ctx, idx, "payloadDownloadRate");

    duk_push_int(ctx, transferred_[3]);
    duk_put_prop_string(ctx, idx, "protocolDownloadRate");

    duk_push_int(ctx, transferred_[4]);
    duk_put_prop_string(ctx, idx, "ipProtocolUploadRate");

    duk_push_int(ctx, transferred_[5]);
    duk_put_prop_string(ctx, idx, "dhtUploadRate");

    duk_push_int(ctx, transferred_[6]);
    duk_put_prop_string(ctx, idx, "trackerUploadRate");

    duk_push_int(ctx, transferred_[7]);
    duk_put_prop_string(ctx, idx, "ipProtocolDownloadRate");

    duk_push_int(ctx, transferred_[8]);
    duk_put_prop_string(ctx, idx, "dhtDownloadRate");

    duk_push_int(ctx, transferred_[9]);
    duk_put_prop_string(ctx, idx, "trackerDownloadRate");
}

