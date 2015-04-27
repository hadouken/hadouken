#include <Hadouken/Scripting/Modules/BitTorrent/Events/EmptyPeerEvent.hpp>

#include <Hadouken/BitTorrent/TorrentHandle.hpp>

#include "../../../duktape.h"

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Scripting::Modules::BitTorrent::Events;

EmptyPeerEvent::EmptyPeerEvent(std::shared_ptr<TorrentHandle> handle, std::string ip, int port)
    : PeerEvent(handle, ip, port)
{
}

void EmptyPeerEvent::push(duk_context* ctx)
{
    duk_idx_t idx = duk_push_object(ctx);
    PeerEvent::push(ctx, idx);
}
