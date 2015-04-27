#include <Hadouken/Scripting/Modules/BitTorrent/Events/BlockEvent.hpp>

#include <Hadouken/BitTorrent/TorrentHandle.hpp>

#include "../../../duktape.h"

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Scripting::Modules::BitTorrent::Events;

BlockEvent::BlockEvent(std::shared_ptr<TorrentHandle> handle, std::string ip, int port, int pieceIndex, int blockIndex)
    : PeerEvent(handle, ip, port)
{
    pieceIndex_ = pieceIndex;
    blockIndex_ = blockIndex;
}

void BlockEvent::push(duk_context* ctx)
{
    duk_idx_t idx = duk_push_object(ctx);
    PeerEvent::push(ctx, idx);

    duk_push_int(ctx, pieceIndex_);
    duk_put_prop_string(ctx, idx, "pieceIndex");

    duk_push_int(ctx, blockIndex_);
    duk_put_prop_string(ctx, idx, "blockIndex");
}
