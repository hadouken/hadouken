#include <Hadouken/Scripting/Modules/BitTorrent/Events/PerformanceEvent.hpp>

#include <Hadouken/BitTorrent/TorrentHandle.hpp>
#include "../../../duktape.h"

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Scripting::Modules::BitTorrent::Events;

PerformanceEvent::PerformanceEvent(std::shared_ptr<TorrentHandle> handle, int code)
    : TorrentEvent(handle)
{
    code_ = code;
}

void PerformanceEvent::push(duk_context* ctx)
{
    duk_idx_t idx = duk_push_object(ctx);
    
    TorrentEvent::push(ctx);
    duk_put_prop_string(ctx, idx, "torrent");

    duk_push_int(ctx, code_);
    duk_put_prop_string(ctx, idx, "code");
}
