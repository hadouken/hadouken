#include <Hadouken/Scripting/Modules/BitTorrent/Events/PeerEvent.hpp>

#include <Hadouken/BitTorrent/TorrentHandle.hpp>

#include "../../../duktape.h"

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Scripting::Modules::BitTorrent::Events;

PeerEvent::PeerEvent(std::shared_ptr<TorrentHandle> handle, std::string ip, int port)
    : TorrentEvent(handle),
    ip_(ip)
{
    port_ = port;
}

void PeerEvent::push(duk_context* ctx, duk_idx_t idx)
{
    TorrentEvent::push(ctx);
    duk_put_prop_string(ctx, idx, "torrent");

    duk_idx_t ipIdx = duk_push_object(ctx);

    duk_push_string(ctx, ip_.c_str());
    duk_put_prop_string(ctx, ipIdx, "address");

    duk_push_int(ctx, port_);
    duk_put_prop_string(ctx, ipIdx, "port");

    duk_put_prop_string(ctx, idx, "ip");
}
