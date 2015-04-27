#include <Hadouken/Scripting/Modules/BitTorrent/Events/TrackerReplyEvent.hpp>

#include <Hadouken/BitTorrent/TorrentHandle.hpp>
#include "../../../duktape.h"

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Scripting::Modules::BitTorrent::Events;

TrackerReplyEvent::TrackerReplyEvent(std::shared_ptr<TorrentHandle> handle, std::string url, int numPeers)
    : TrackerEvent(handle, url)
{
    numPeers_ = numPeers;
}

void TrackerReplyEvent::push(duk_context* ctx)
{
    duk_idx_t idx = duk_push_object(ctx);
    TrackerEvent::push(ctx, idx);
    
    duk_push_int(ctx, numPeers_);
    duk_put_prop_string(ctx, idx, "numPeers");
}
