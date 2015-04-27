#include <Hadouken/Scripting/Modules/BitTorrent/Events/PeerErrorEvent.hpp>

#include <Hadouken/BitTorrent/TorrentHandle.hpp>

#include "../../../duktape.h"

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Scripting::Modules::BitTorrent::Events;

PeerErrorEvent::PeerErrorEvent(std::shared_ptr<TorrentHandle> handle, std::string ip, int port, Error error)
    : PeerEvent(handle, ip, port),
    error_(error)
{
}

void PeerErrorEvent::push(duk_context* ctx)
{
    duk_idx_t idx = duk_push_object(ctx);
    PeerEvent::push(ctx, idx);

    // ----- error
    duk_idx_t errIdx = duk_push_object(ctx);

    duk_put_prop_string(ctx, idx, "error");
}
