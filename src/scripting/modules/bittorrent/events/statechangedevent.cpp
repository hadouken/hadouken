#include <Hadouken/Scripting/Modules/BitTorrent/Events/StateChangedEvent.hpp>

#include <Hadouken/BitTorrent/TorrentHandle.hpp>
#include "../../../duktape.h"

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Scripting::Modules::BitTorrent::Events;

StateChangedEvent::StateChangedEvent(std::shared_ptr<TorrentHandle> handle, int state, int previousState)
    : TorrentEvent(handle)
{
    state_ = state;
    previousState_ = previousState;
}

void StateChangedEvent::push(duk_context* ctx)
{
    duk_idx_t idx = duk_push_object(ctx);
    
    TorrentEvent::push(ctx);
    duk_put_prop_string(ctx, idx, "torrent");

    duk_push_int(ctx, state_);
    duk_put_prop_string(ctx, idx, "state");

    duk_push_int(ctx, previousState_);
    duk_put_prop_string(ctx, idx, "previousState");
}
