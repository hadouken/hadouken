#include <Hadouken/Scripting/Modules/BitTorrent/Events/HashFailedEvent.hpp>

#include <Hadouken/BitTorrent/TorrentHandle.hpp>
#include "../../../duktape.h"

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Scripting::Modules::BitTorrent::Events;

HashFailedEvent::HashFailedEvent(std::shared_ptr<TorrentHandle> handle, int pieceIndex)
    : TorrentEvent(handle)
{
    pieceIndex_ = pieceIndex;
}

void HashFailedEvent::push(duk_context* ctx)
{
    duk_idx_t idx = duk_push_object(ctx);
    
    TorrentEvent::push(ctx);
    duk_put_prop_string(ctx, idx, "torrent");

    duk_push_int(ctx, pieceIndex_);
    duk_put_prop_string(ctx, idx, "pieceIndex");
}
