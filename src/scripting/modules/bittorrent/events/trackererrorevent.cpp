#include <Hadouken/Scripting/Modules/BitTorrent/Events/TrackerErrorEvent.hpp>

#include <Hadouken/BitTorrent/TorrentHandle.hpp>
#include "../../../duktape.h"

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Scripting::Modules::BitTorrent::Events;

TrackerErrorEvent::TrackerErrorEvent(std::shared_ptr<TorrentHandle> handle, Error error, std::string url, int times, int statusCode, std::string message)
    : TrackerEvent(handle, url),
    error_(error),
    message_(message)
{
    times_ = times;
    statusCode_ = statusCode;
}

void TrackerErrorEvent::push(duk_context* ctx)
{
    duk_idx_t idx = duk_push_object(ctx);
    TrackerEvent::push(ctx, idx);

    duk_push_int(ctx, times_);
    duk_put_prop_string(ctx, idx, "times");

    duk_push_int(ctx, statusCode_);
    duk_put_prop_string(ctx, idx, "statusCode");

    duk_push_string(ctx, message_.c_str());
    duk_put_prop_string(ctx, idx, "message");

    // ---- error
    duk_idx_t errIdx = duk_push_object(ctx);
    
    duk_push_int(ctx, error_.code);
    duk_put_prop_string(ctx, errIdx, "code");

    duk_push_string(ctx, error_.message.c_str());
    duk_put_prop_string(ctx, errIdx, "message");

    duk_put_prop_string(ctx, idx, "error");
}
